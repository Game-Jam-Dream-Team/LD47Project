using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using Game.Common;
using Game.State;

using TMPro;

namespace Game.Behaviour {
	public sealed class PlayerCommentView : MonoBehaviour {
		public TMP_InputField InputField;
		public Button         SendCommentButton;

		Tweet           _mainTweet;
		TweetsFeedView2 _feedView;

		bool _isCommonInit;

		public void InitTweet(Tweet mainTweet, TweetsFeedView2 feedView) {
			TryCommonInit();

			_mainTweet = mainTweet;
			_feedView  = feedView;

			InputField.text = string.Empty;
		}

		public void DeinitTweet() {
			_mainTweet = null;
		}

		void OnSendCommentClick() {
			var message  = InputField.text;
			var tweetId  = message.GetHashCode();
			var senderId = TweetsController.PlayerId;
			var qc       = GameState.Instance.QuestController;
			var type = qc.TryReply(_mainTweet.Id, message)
				? TweetType.Comment
				: TweetType.Temporary;
			var childIndex = _mainTweet.CommentIds.Count;
			var tweet = new Tweet(tweetId, type, senderId, message, -1, new List<int>());
			GameState.Instance.TweetsController.AddComment(_mainTweet, tweet);
			_feedView.OnCommentAdded(_mainTweet, childIndex);
			if ( GameState.Instance.QuestController.OnCommentPosted(_mainTweet.Id) ) {
				tweet.Type = TweetType.Comment;
			}
			InputField.text = string.Empty;
		}

		void TryCommonInit() {
			if ( _isCommonInit ) {
				return;
			}

			SendCommentButton.onClick.AddListener(OnSendCommentClick);

			_isCommonInit = true;
		}
	}
}
