using UnityEngine;
using UnityEngine.UI;

using System;
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

		TweetsController _controller;

		Tweet _tweet;

		bool                   _isCommonInit;
		SenderCollection       _senderCollection;
		TweetSpritesCollection _tweetSpritesCollection;

		void OnDestroy() {
			if ( _tweet != null ) {
				_tweet.OnCommentsCountChanged -= UpdateCommentsCount;
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
			while ( index < 0 ) {
				index += tc.RootTweetsCount;
			}
			index %= tc.RootTweetsCount;
			var tweet = tc.GetRootTweetByIndex(index);
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

		public void InitTweet(TweetsController controller, Tweet tweet) {
			_controller = controller;
			_tweet      = tweet;

			var senderInfo = _senderCollection.GetSenderInfo(_tweet.SenderId);
			Avatar.sprite = senderInfo.Avatar;
			InitSender(senderInfo.DisplayName);
			MessageText.text = _tweet.Message;
			UpdateCommentsCount(_tweet.CommentsCount);
			UpdateLikesCount(_tweet.LikesCount);
			UpdateRetweetsCount(_tweet.RetweetsCount);

			if ( _tweet.ImageId == -1 ) {
				TweetImageRoot.SetActive(false);
			} else {
				TweetImageRoot.SetActive(true);
				TweetImage.sprite = _tweetSpritesCollection.GetTweetSprite(_tweet.ImageId);
			}

			_tweet.OnCommentsCountChanged += UpdateCommentsCount;
			_tweet.OnLikesCountChanged    += UpdateLikesCount;
			_tweet.OnRetweetsCountChanged += UpdateRetweetsCount;
		}

		public void DeinitTweet() {
			if ( _tweet != null ) {
				_tweet.OnCommentsCountChanged -= UpdateCommentsCount;
				_tweet.OnLikesCountChanged    -= UpdateLikesCount;
				_tweet.OnRetweetsCountChanged -= UpdateRetweetsCount;

				_tweet = null;
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
			SendMessageUpwards("TryShowCommentsScreen", _tweet, SendMessageOptions.DontRequireReceiver);
		}

		void OnLikesClick() { }

		void OnRetweetsClick() { }

		void OnImageClick() {
			_controller.ClickImage(_tweet);
		}

		void UpdateCommentsCount(int commentsCount) {
			CommentsText.text = commentsCount.ToString();
		}

		void UpdateLikesCount(int likesCount) {
			LikesText.text = likesCount.ToString();
		}

		void UpdateRetweetsCount(int retweetsCount) {
			RetweetsText.text = retweetsCount.ToString();
		}
	}
}
