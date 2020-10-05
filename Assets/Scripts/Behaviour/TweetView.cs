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

		TweetsController _tweetsController;
		AgeController    _ageController;
		QuestController  _questController;

		Tweet _tweet;

		bool                   _isCommonInit;
		SenderCollection       _senderCollection;
		TweetSpritesCollection _tweetSpritesCollection;

		void OnDestroy() {
			LikeButton.OnPressed     -= OnLikesPressed;
			LikeButton.OnReleased    -= OnLikesReleased;
			RetweetButton.OnPressed  -= OnRetweetsPressed;
			RetweetButton.OnReleased -= OnRetweetsReleased;
			if ( _tweet != null ) {
				TryReleaseFakeLike();
				TryReleaseFakeRetweet();
				_tweet.OnCommentsCountChanged -= OnCommentsCountChanged;
				_tweet.OnLikesCountChanged    -= UpdateLikesCount;
				_tweet.OnRetweetsCountChanged -= UpdateRetweetsCount;
				_tweet.OnMessageChanged       -= OnTweetMessageChanged;
				_tweet.OnPlayerLikeChanged    -= OnPlayerLikeChanged;
				_tweet.OnPlayerRetweetChanged -= OnPlayerRetweetChanged;
			}
		}

		void OnEnable() {
			TryCommonInit();
		}

		[UsedImplicitly]
		public void ScrollCellIndex(int index) {
			if ( _tweet != null ) {
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
					InitReply(tweet);
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
				InitTweet(tc, ac, qc, tweet, isRoot);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
		}

		public void InitReply(Tweet tweet) {
			TweetRoot.SetActive(false);
			ReplyRoot.SetActive(true);
			PlayerCommentView.InitTweet(tweet);
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

		public void InitTweet(TweetsController tweetsController, AgeController ageController,
			QuestController questController, Tweet tweet, bool isRoot = true) {
			_tweetsController = tweetsController;
			_ageController    = ageController;
			_questController  = questController;
			_tweet            = tweet;

			_questController.OnSenderAvatarChanged += OnSenderAvatarChanged;
			_questController.OnTweetImageChanged   += OnTweetImageChanged;
			_tweet.OnMessageChanged                += OnTweetMessageChanged;

			var senderInfo = _senderCollection.GetSenderInfo(_tweet.SenderId);
			Avatar.sprite = senderInfo.OverrideAvatar ? senderInfo.OverrideAvatar : senderInfo.Avatar;
			InitSender(senderInfo.DisplayName);
			MessageText.text = _tweet.Message;
			CommentsText.text = _tweet.CommentsCount.ToString();
			UpdateLikesCount(_tweet.LikesCount);
			UpdateRetweetsCount(_tweet.RetweetsCount);

			AgeRestrictionRoot.SetActive(false);
			if ( _tweet.ImageId == -1 ) {
				TweetImageRoot.SetActive(false);
			} else {
				TweetImageRoot.SetActive(true);
				TweetImage.sprite = _tweetSpritesCollection.GetTweetSprite(_tweet.Id, _tweet.ImageId);
				if ( _tweetSpritesCollection.IsAgeRestricted(_tweet.ImageId) ) {
					AgeRestrictionRoot.SetActive(!_ageController.IsAdult);
					_ageController.OnIsAdultChanged += OnIsAdultChanged;
				}
			}

			LayoutGroup.padding.left     = isRoot ? 0 : 100;
			RightAreaTransform.sizeDelta = new Vector2(isRoot ? 490 : 390, RightAreaTransform.sizeDelta.y);

			if ( tweet.Type == TweetType.Temporary ) {
				StartCoroutine(TempDisappearCoro());
			} else if ( isRoot ) {
				_tweet.OnCommentsCountChanged += OnCommentsCountChanged;
				_tweet.OnLikesCountChanged    += UpdateLikesCount;
				_tweet.OnRetweetsCountChanged += UpdateRetweetsCount;
				_tweet.OnPlayerLikeChanged    += OnPlayerLikeChanged;
				_tweet.OnPlayerRetweetChanged += OnPlayerRetweetChanged;
				OnPlayerLikeChanged(_tweet.PlayerLike);
				OnPlayerRetweetChanged(_tweet.PlayerRetweet);
			}
		}

		void OnSenderAvatarChanged(int senderId, Sprite newAvatar) {
			if ( _tweet.SenderId != senderId ) {
				return;
			}
			Avatar.sprite = newAvatar;
		}

		void OnTweetImageChanged(int tweetId, Sprite newImage) {
			if ( _tweet.Id != tweetId ) {
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
			yield return new WaitForSeconds(2f);
			_tweetsController.RemoveTweet(_tweet);
		}

		public void DeinitTweet() {
			if ( _tweet != null ) {
				if ( _tweet.Type == TweetType.Temporary ) {
					StopAllCoroutines();
					_tweetsController.RemoveTweet(_tweet);
				}
				_tweet.OnCommentsCountChanged -= OnCommentsCountChanged;
				_tweet.OnLikesCountChanged    -= UpdateLikesCount;
				_tweet.OnRetweetsCountChanged -= UpdateRetweetsCount;
				_tweet.OnMessageChanged       -= OnTweetMessageChanged;
				_tweet.OnPlayerLikeChanged    -= OnPlayerLikeChanged;
				_tweet.OnPlayerRetweetChanged -= OnPlayerRetweetChanged;

				_tweet = null;

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
		}

		bool _fakeLike;
		void OnLikesPressed() {
			var state = _tweetsController.GetPlayerLike(_tweet.Id);
			if ( !state ) {
				_tweetsController.SetPlayerLike(_tweet.Id, true);
				_fakeLike = true;
			}
		}

		void OnLikesReleased() {
			TryReleaseFakeLike();
		}

		void TryReleaseFakeLike() {
			if ( _fakeLike ) {
				_tweetsController.SetPlayerLike(_tweet.Id, false);
				_fakeLike = false;
			}
		}

		void OnLikesClick() {
			var state    = _tweetsController.GetPlayerLike(_tweet.Id);
			var newState = !state;
			_tweetsController.SetPlayerLike(_tweet.Id, newState);
			_questController.OnPlayerLikeChanged(_tweet.Id, newState);
		}

		bool _fakeRetweet;
		void OnRetweetsPressed() {
			var state = _tweetsController.GetPlayerRetweet(_tweet.Id);
			if ( !state ) {
				_tweetsController.SetPlayerRetweet(_tweet.Id, true);
				_fakeRetweet = true;
			}

		}

		void OnRetweetsReleased() {
			TryReleaseFakeRetweet();
		}

		void TryReleaseFakeRetweet() {
			if ( _fakeRetweet ) {
				_tweetsController.SetPlayerRetweet(_tweet.Id, false);
				_fakeRetweet = false;
			}
		}

		void OnRetweetsClick() {
			var state    = _tweetsController.GetPlayerRetweet(_tweet.Id);
			var newState = !state;
			_tweetsController.SetPlayerRetweet(_tweet.Id, newState);
			_questController.OnPlayerRetweetChanged(_tweet.Id, newState);
		}

		void OnImageClick() {
			_tweetsController.ClickImage(_tweet);
		}

		void OnCommentsCountChanged(int commentsCount) {
			var tweet            = _tweet;
			var tweetsController = _tweetsController;
			var ageController    = _ageController;
			var questController  = _questController;
			DeinitTweet();
			InitTweet(tweetsController, ageController, questController, tweet);
			SendMessageUpwards("UpdateLayoutDelayed", SendMessageOptions.DontRequireReceiver);
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
