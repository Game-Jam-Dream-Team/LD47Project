using Game.Common;
using UnityEngine;
using UnityEngine.UI;

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
			// TODO: send comment
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
