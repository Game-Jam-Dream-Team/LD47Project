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

		Tweet _mainTweet;

		bool _isCommonInit;

		public void InitTweet(Tweet mainTweet) {
			TryCommonInit();

			_mainTweet = mainTweet;

			InputField.text = string.Empty;
		}

		public void DeinitTweet() {
			_mainTweet = null;
		}

		void OnSendCommentClick() {
			var message  = InputField.text;
			var tweetId  = message.GetHashCode();
			var senderId = TweetsController.PlayerId;
			var tweet    = new Tweet(tweetId, senderId, message, -1, new List<int>());
			GameState.Instance.TweetsController.AddComment(_mainTweet, tweet);
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
