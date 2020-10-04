using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Game.Common;
using Game.State;

using JetBrains.Annotations;
using TMPro;

namespace Game.Behaviour {
	public sealed class TweetView : MonoBehaviour {
		const string NameTemplate = "<b>{0}</b><color=#aaaaaa>{1}</color>";

		public Image      Avatar;
		public TMP_Text   NameText;
		public TMP_Text   MessageText;
		public Button     CommentButton;
		public TMP_Text   CommentsText;
		public Button     LikeButton;
		public TMP_Text   LikesText;
		public Button     RetweetButton;
		public TMP_Text   RetweetsText;
		public GameObject TweetImageRoot;
		public Image      TweetImage;
		public Button     TweetImageButton;
		[Space]
		public RectTransform     CommentsRoot;
		public RectTransform     PlayerCommentViewTransform;
		public PlayerCommentView PlayerCommentView;
		public GameObject        TweetCommentPrefab;

		TweetsController _controller;

		Tweet _tweet;

		bool                   _isCommonInit;
		SenderCollection       _senderCollection;
		TweetSpritesCollection _tweetSpritesCollection;

		readonly List<TweetView> _commentViews = new List<TweetView>();

		void OnDestroy() {
			if ( _tweet != null ) {
				_tweet.OnCommentsCountChanged -= OnCommentsCountChanged;
				_tweet.OnLikesCountChanged    -= UpdateLikesCount;
				_tweet.OnRetweetsCountChanged -= UpdateRetweetsCount;
			}
		}

		void OnEnable() {
			TryCommonInit();
		}

		[UsedImplicitly]
		public void ScrollCellIndex(int index) {
			if ( _tweet != null ) {
				DeinitTweet();
			}
			var tc = GameState.Instance.TweetsController;
			var qc = GameState.Instance.QuestController;
			while ( index < 0 ) {
				index += qc.CurrentTweets.Length;
			}
			index %= qc.CurrentTweets.Length;
			var tweet = qc.CurrentTweets[index];
			InitTweet(tc, tweet);
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
		}

		public void TryCommonInit() {
			if ( _isCommonInit ) {
				return;
			}

			_senderCollection       = Resources.Load<SenderCollection>("SenderCollection");
			_tweetSpritesCollection = Resources.Load<TweetSpritesCollection>("TweetSpritesCollection");

			CommentButton.onClick.AddListener(OnCommentsClick);
			LikeButton.onClick.AddListener(OnLikesClick);
			RetweetButton.onClick.AddListener(OnRetweetsClick);
			TweetImageButton.onClick.AddListener(OnImageClick);

			_isCommonInit = true;
		}

		public void InitTweet(TweetsController controller, Tweet tweet, bool isRoot = true) {
			_controller = controller;
			_tweet      = tweet;

			var senderInfo = _senderCollection.GetSenderInfo(_tweet.SenderId);
			Avatar.sprite = senderInfo.Avatar;
			InitSender(senderInfo.DisplayName);
			MessageText.text = _tweet.Message;
			CommentsText.text = _tweet.CommentsCount.ToString();
			UpdateLikesCount(_tweet.LikesCount);
			UpdateRetweetsCount(_tweet.RetweetsCount);

			if ( _tweet.ImageId == -1 ) {
				TweetImageRoot.SetActive(false);
			} else {
				TweetImageRoot.SetActive(true);
				TweetImage.sprite = _tweetSpritesCollection.GetTweetSprite(_tweet.ImageId);
			}

			if ( CommentsRoot ) {
				CommentsRoot.gameObject.SetActive(isRoot && (tweet.Type != TweetType.Temporary) &&
				                                  (_tweet.CommentIds.Count > 0));
			}
			if ( tweet.Type == TweetType.Temporary ) {
				StartCoroutine(TempDisappearCoro());
			} else if ( isRoot ) {
				PlayerCommentView.InitTweet(_tweet);

				foreach ( var commentId in _tweet.CommentIds ) {
					AddCommentView(commentId);
				}
				PlayerCommentViewTransform.SetAsLastSibling();

				_tweet.OnCommentsCountChanged += OnCommentsCountChanged;
				_tweet.OnLikesCountChanged    += UpdateLikesCount;
				_tweet.OnRetweetsCountChanged += UpdateRetweetsCount;
			}
		}

		void AddCommentView(int commentId) {
			var commentViewGo = Instantiate(TweetCommentPrefab, CommentsRoot);
			var commentView = commentViewGo.GetComponent<TweetView>();
			commentView.TryCommonInit();
			commentView.InitTweet(_controller, _controller.GetTweetById(commentId), false);
			commentView.transform.SetAsLastSibling();
			_commentViews.Add(commentView);
		}

		IEnumerator TempDisappearCoro() {
			yield return new WaitForSeconds(2f);
			_controller.RemoveTweet(_tweet);
		}

		public void DeinitTweet() {
			if ( _tweet != null ) {
				if ( PlayerCommentView ) {
					PlayerCommentView.DeinitTweet();
				}

				_tweet.OnCommentsCountChanged -= OnCommentsCountChanged;
				_tweet.OnLikesCountChanged    -= UpdateLikesCount;
				_tweet.OnRetweetsCountChanged -= UpdateRetweetsCount;

				_tweet = null;

				foreach ( var commentView in _commentViews ) {
					commentView.DeinitTweet();
					Destroy(commentView.gameObject);
				}
				_commentViews.Clear();
			}
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
			CommentsRoot.gameObject.SetActive(!CommentsRoot.gameObject.activeSelf);
			SendMessageUpwards("UpdateLayout");
		}

		void OnLikesClick() { }

		void OnRetweetsClick() { }

		void OnImageClick() {
			_controller.ClickImage(_tweet);
		}

		void OnCommentsCountChanged(int commentsCount) {
			var tweet      = _tweet;
			var controller = _controller;
			DeinitTweet();
			InitTweet(controller, tweet);
			SendMessageUpwards("UpdateLayout");
		}

		void UpdateLikesCount(int likesCount) {
			LikesText.text = likesCount.ToString();
		}

		void UpdateRetweetsCount(int retweetsCount) {
			RetweetsText.text = retweetsCount.ToString();
		}
	}
}
