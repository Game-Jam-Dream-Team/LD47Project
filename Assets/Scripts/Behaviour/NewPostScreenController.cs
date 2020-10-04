using UnityEngine;
using UnityEngine.UI;

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

			// TODO: init PlayerAvatar.sprite

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
			// TODO: post
			_mainScreenController.TryHideNewPostScreen();
			NewPostInputField.text = string.Empty;
		}
	}
}
