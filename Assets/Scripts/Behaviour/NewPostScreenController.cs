using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using Game.Common;
using Game.State;

using TMPro;

namespace Game.Behaviour {
	public sealed class NewPostScreenController : MonoBehaviour {
		public Button         BackButton;
		public Button         PostButton;
		public Image          PlayerAvatar;
		public TMP_InputField NewPostInputField;

		bool _isShown;

		MainScreenController _mainScreenController;

		public void CommonInit(MainScreenController mainScreenController) {
			_mainScreenController = mainScreenController;

			var senderCollection = Resources.Load<SenderCollection>("SenderCollection");
			PlayerAvatar.sprite = senderCollection.GetSenderInfo(TweetsController.PlayerId).Avatar;

			BackButton.onClick.AddListener(OnBackButtonClick);
			PostButton.onClick.AddListener(OnPostButtonClick);
		}

		public bool TryShow() {
			if ( _isShown ) {
				return false;
			}
			_isShown = true;
			return true;
		}

		public bool TryHide() {
			if ( !_isShown ) {
				return false;
			}
			_isShown = false;
			return true;
		}

		void OnBackButtonClick() {
			_mainScreenController.TryHideNewPostScreen();
		}

		void OnPostButtonClick() {
			var message = NewPostInputField.text;
			var tweetId = message.GetHashCode();
			var qc      = GameState.Instance.QuestController;
			var tc      = GameState.Instance.TweetsController;
			var type    = qc.TryPost(message) ? TweetType.Quest1 : TweetType.Temporary;
			var tweet = new Tweet(tweetId, type, TweetsController.PlayerId, message, -1, new List<int>());
			tc.AddTweet(tweet);
			NewPostInputField.text = string.Empty;
			_mainScreenController.TryHideNewPostScreen();
		}
	}
}
