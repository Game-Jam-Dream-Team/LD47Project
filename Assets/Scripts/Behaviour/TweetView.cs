using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

using Game.Common;
using Game.State;

using JetBrains.Annotations;
using TMPro;

namespace Game.Behaviour {
	public sealed class TweetView : MonoBehaviour {
		const string NameTemplate = "<b>{0}</b><color=#aaaaaa>{1}</color>";

		public Image       Avatar;
		public TMP_Text    NameText;
		public TMP_Text    MessageText;
		public Button      CommentButton;
		public TMP_Text    CommentsText;
		public Image       LikeIcon;
		public PressButton LikeButton;
		public TMP_Text    LikesText;
		public Image       RetweetIcon;
		public PressButton RetweetButton;
		public TMP_Text    RetweetsText;
		public GameObject  TweetImageRoot;
		public Image       TweetImage;
		public Button      TweetImageButton;
		public GameObject  AgeRestrictionRoot;
		[Space]
		public HorizontalLayoutGroup LayoutGroup;
		public RectTransform         RightAreaTransform;
		[Space]
		public GameObject        TweetRoot;
		public GameObject        ReplyRoot;
		public PlayerCommentView PlayerCommentView;

		TweetsFeedView2 _feedView;

		TweetsController _tweetsController;
		AgeController    _ageController;
		QuestController  _questController;

		bool                   _isCommonInit;
		SenderCollection       _senderCollection;
		TweetSpritesCollection _tweetSpritesCollection;

		public Tweet Tweet { get; private set; }

		public bool ReplyShown { get; set; }

		void OnDestroy() {
			LikeButton.OnPressed     -= OnLikesPressed;
			LikeButton.OnReleased    -= OnLikesReleased;
			RetweetButton.OnPressed  -= OnRetweetsPressed;
			RetweetButton.OnReleased -= OnRetweetsReleased;
			if ( Tweet != null ) {
				TryReleaseFakeLike();
				TryReleaseFakeRetweet();
				Tweet.OnCommentsCountChanged -= OnCommentsCountChanged;
				Tweet.OnLikesCountChanged    -= UpdateLikesCount;
				Tweet.OnRetweetsCountChanged -= UpdateRetweetsCount;
				Tweet.OnMessageChanged       -= OnTweetMessageChanged;
				Tweet.OnPlayerLikeChanged    -= OnPlayerLikeChanged;
				Tweet.OnPlayerRetweetChanged -= OnPlayerRetweetChanged;
			}
		}

		void OnEnable() {
			TryCommonInit();
		}

		[UsedImplicitly]
		public void ScrollCellIndex(int index) {
			if ( Tweet != null ) {
				DeinitTweet();
				PlayerCommentView.DeinitTweet();
			}
			var tc = GameState.Instance.TweetsController;
			var ac = GameState.Instance.AgeController;
			var qc = GameState.Instance.QuestController;
			var totalTweets = qc.CurrentTweets.Length * 2; // with fake tweets for replies
			foreach ( var tmpTweet in qc.CurrentTweets ) {
				totalTweets += tmpTweet.CommentIds.Count;
			}
			while ( index < 0 ) {
				index += totalTweets;
			}
			index %= totalTweets;
			Tweet tweet = null;
			var accIndex = 0;
			var isRoot  = true;
			var isTweet = true;
			for ( var i = 0; i < qc.CurrentTweets.Length; ++i ) {
				var curTweet = qc.CurrentTweets[i];
				if ( accIndex == index ) {
					tweet = curTweet;
					break;
				}
				if ( accIndex + curTweet.CommentIds.Count + 1 == index ) {
					// TODO: init reply
					tweet = curTweet;
					InitReply(tweet, null);
					isTweet = false;
					break;
				}
				if ( curTweet.CommentIds.Count == 0 ) {
					accIndex += 2;
					continue;
				}
				if ( accIndex + curTweet.CommentIds.Count < index ) {
					accIndex += curTweet.CommentIds.Count + 2;
					continue;
				}
				var childIndex = index - accIndex - 1;
				tweet  = tc.GetTweetById(curTweet.CommentIds[childIndex]);
				isRoot = false;
				break;
			}
			if ( tweet == null ) {
				Debug.LogError("Tweet is null");
				return;
			}
			if ( isTweet ) {
				TweetRoot.SetActive(true);
				ReplyRoot.SetActive(false);
				InitTweet(null, tc, ac, qc, tweet, isRoot);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
		}

		public void InitReply(Tweet tweet, TweetsFeedView2 feedView) {
			TweetRoot.SetActive(false);
			ReplyRoot.SetActive(true);
			PlayerCommentView.InitTweet(tweet, feedView);
		}

		public void TryCommonInit() {
			if ( _isCommonInit ) {
				return;
			}

			_senderCollection       = Resources.Load<SenderCollection>("SenderCollection");
			_tweetSpritesCollection = Resources.Load<TweetSpritesCollection>("TweetSpritesCollection");

			CommentButton.onClick.AddListener(OnCommentsClick);
			LikeButton.OnPressed  += OnLikesPressed;
			LikeButton.OnReleased += OnLikesReleased;
			LikeButton.onClick.AddListener(OnLikesClick);
			RetweetButton.OnPressed  += OnRetweetsPressed;
			RetweetButton.OnReleased += OnRetweetsReleased;
			RetweetButton.onClick.AddListener(OnRetweetsClick);
			TweetImageButton.onClick.AddListener(OnImageClick);

			_isCommonInit = true;
		}

		public void InitTweet(TweetsFeedView2 feedView, TweetsController tweetsController, AgeController ageController,
			QuestController questController, Tweet tweet, bool isRoot = true) {
			_feedView         = feedView;
			_tweetsController = tweetsController;
			_ageController    = ageController;
			_questController  = questController;
			Tweet             = tweet;

			_questController.OnSenderAvatarChanged += OnSenderAvatarChanged;
			_questController.OnTweetImageChanged   += OnTweetImageChanged;
			Tweet.OnMessageChanged                += OnTweetMessageChanged;

			var senderInfo = _senderCollection.GetSenderInfo(Tweet.SenderId);
			Avatar.sprite = senderInfo.OverrideAvatar ? senderInfo.OverrideAvatar : senderInfo.Avatar;
			InitSender(senderInfo.DisplayName);
			MessageText.text = Tweet.Message;
			CommentsText.text = Tweet.CommentsCount.ToString();
			UpdateLikesCount(Tweet.LikesCount);
			UpdateRetweetsCount(Tweet.RetweetsCount);

			AgeRestrictionRoot.SetActive(false);
			if ( Tweet.ImageId == -1 ) {
				TweetImageRoot.SetActive(false);
			} else {
				TweetImageRoot.SetActive(true);
				TweetImage.sprite = _tweetSpritesCollection.GetTweetSprite(Tweet.Id, Tweet.ImageId);
				if ( _tweetSpritesCollection.IsAgeRestricted(Tweet.ImageId) ) {
					AgeRestrictionRoot.SetActive(!_ageController.IsAdult);
					_ageController.OnIsAdultChanged += OnIsAdultChanged;
				}
			}

			LayoutGroup.padding.left     = isRoot ? 0 : 100;
			RightAreaTransform.sizeDelta = new Vector2(isRoot ? 490 : 390, RightAreaTransform.sizeDelta.y);

			CommentButton.gameObject.SetActive(isRoot);

			if ( tweet.Type == TweetType.Temporary ) {
				StartCoroutine(TempDisappearCoro());
			} else {
				if ( isRoot ) {
					Tweet.OnCommentsCountChanged += OnCommentsCountChanged;
				}
				Tweet.OnLikesCountChanged    += UpdateLikesCount;
				Tweet.OnRetweetsCountChanged += UpdateRetweetsCount;
				Tweet.OnPlayerLikeChanged    += OnPlayerLikeChanged;
				Tweet.OnPlayerRetweetChanged += OnPlayerRetweetChanged;
				OnPlayerLikeChanged(Tweet.PlayerLike);
				OnPlayerRetweetChanged(Tweet.PlayerRetweet);
			}
		}

		void OnSenderAvatarChanged(int senderId, Sprite newAvatar) {
			if ( Tweet.SenderId != senderId ) {
				return;
			}
			Avatar.sprite = newAvatar;
		}

		void OnTweetImageChanged(int tweetId, Sprite newImage) {
			if ( Tweet.Id != tweetId ) {
				return;
			}
			TweetImage.sprite = newImage;
		}

		void OnIsAdultChanged(bool isAdult) {
			AgeRestrictionRoot.SetActive(!isAdult);
		}

		void OnTweetMessageChanged(string newMessage) {
			MessageText.text = newMessage;
		}

		IEnumerator TempDisappearCoro() {
			yield return new WaitForSeconds(3f);
			if ( Tweet.Type != TweetType.Temporary ) {
				yield break;
			}
			_tweetsController.RemoveTweet(Tweet);
			_feedView.OnTweetViewRemoved(this);
		}

		public void DeinitTweet() {
			_feedView   = null;
			ReplyShown = false;
			if ( Tweet != null ) {
				if ( Tweet.Type == TweetType.Temporary ) {
					StopAllCoroutines();
					_tweetsController.RemoveTweet(Tweet);
				}
				Tweet.OnCommentsCountChanged -= OnCommentsCountChanged;
				Tweet.OnLikesCountChanged    -= UpdateLikesCount;
				Tweet.OnRetweetsCountChanged -= UpdateRetweetsCount;
				Tweet.OnMessageChanged       -= OnTweetMessageChanged;
				Tweet.OnPlayerLikeChanged    -= OnPlayerLikeChanged;
				Tweet.OnPlayerRetweetChanged -= OnPlayerRetweetChanged;

				Tweet = null;

				_questController.OnSenderAvatarChanged -= OnSenderAvatarChanged;
				_questController.OnTweetImageChanged   -= OnTweetImageChanged;

				_ageController.OnIsAdultChanged -= OnIsAdultChanged;
			}
		}

		public float GetHeight() {
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
			if ( TweetRoot.activeSelf ) {
				return 150f + MessageText.preferredHeight + 10f + (TweetImageRoot.activeSelf ? 205f : 0f);
			}
			return 105f;
		}

		void InitSender(string displayName) {
			var index = displayName.IndexOf("@", StringComparison.InvariantCulture);
			if ( index > 0 ) {
				NameText.text = string.Format(NameTemplate, displayName.Substring(0, index),
					displayName.Substring(index, displayName.Length - index));
			} else {
				NameText.text = displayName;
			}
		}

		void OnCommentsClick() {
			if ( ReplyShown ) {
				_feedView.HideReply(this);
				ReplyShown = false;
			} else {
				_feedView.ShowReply(this);
				ReplyShown = true;
			}
		}

		bool _fakeLike;
		void OnLikesPressed() {
			var state = _tweetsController.GetPlayerLike(Tweet.Id);
			if ( !state ) {
				_tweetsController.SetPlayerLike(Tweet.Id, true);
				_fakeLike = true;
			}
		}

		void OnLikesReleased() {
			TryReleaseFakeLike();
		}

		void TryReleaseFakeLike() {
			if ( _fakeLike ) {
				_tweetsController.SetPlayerLike(Tweet.Id, false);
				_fakeLike = false;
			}
		}

		void OnLikesClick() {
			var state    = _tweetsController.GetPlayerLike(Tweet.Id);
			var newState = !state;
			_tweetsController.SetPlayerLike(Tweet.Id, newState);
			_questController.OnPlayerLikeChanged(Tweet.Id, newState);
		}

		bool _fakeRetweet;
		void OnRetweetsPressed() {
			var state = _tweetsController.GetPlayerRetweet(Tweet.Id);
			if ( !state ) {
				_tweetsController.SetPlayerRetweet(Tweet.Id, true);
				_fakeRetweet = true;
			}

		}

		void OnRetweetsReleased() {
			TryReleaseFakeRetweet();
		}

		void TryReleaseFakeRetweet() {
			if ( _fakeRetweet ) {
				_tweetsController.SetPlayerRetweet(Tweet.Id, false);
				_fakeRetweet = false;
			}
		}

		void OnRetweetsClick() {
			var state    = _tweetsController.GetPlayerRetweet(Tweet.Id);
			var newState = !state;
			_tweetsController.SetPlayerRetweet(Tweet.Id, newState);
			_questController.OnPlayerRetweetChanged(Tweet.Id, newState);
		}

		void OnImageClick() {
			_tweetsController.ClickImage(Tweet);
		}

		void OnCommentsCountChanged(int commentsCount) {
			CommentsText.text = commentsCount.ToString();
		}

		void UpdateLikesCount(int likesCount) {
			LikesText.text = likesCount.ToString();
		}

		void UpdateRetweetsCount(int retweetsCount) {
			RetweetsText.text = retweetsCount.ToString();
		}

		void OnPlayerLikeChanged(bool playerLike) {
			LikeIcon.color = playerLike ? Color.yellow : Color.white;
		}

		void OnPlayerRetweetChanged(bool playerRetweet) {
			RetweetIcon.color = playerRetweet ? Color.yellow : Color.white;
		}
	}
}
