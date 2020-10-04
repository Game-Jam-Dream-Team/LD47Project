using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Behaviour {
	public class SettingsWindow : MonoBehaviour {
		public GameObject     AgeBlock;
		public TMP_InputField AgeInput;
		public Button         SettingsButton;
		public Button         BackButton;

		void Awake() {
			gameObject.SetActive(false);
			AgeBlock.SetActive(false);
			SettingsButton.onClick.AddListener(Show);
			BackButton.onClick.AddListener(Hide);
			AgeInput.onValueChanged.AddListener(OnAgeChanged);
		}

		void Show() {
			gameObject.SetActive(true);
			AgeBlock.SetActive(IsAgeShouldBeShown());
		}

		void Hide() {
			gameObject.SetActive(false);
		}

		bool IsAgeShouldBeShown() {
			// TODO: check condition
			return false;
		}

		void OnAgeChanged(string value) {
			if ( !int.TryParse(value, out var intValue) ) {
				return;
			}
			if ( intValue >= 99 ) {
				// TODO: condition met
			}
		}
	}
}