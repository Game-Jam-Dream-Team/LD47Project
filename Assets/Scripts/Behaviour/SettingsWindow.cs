using UnityEngine;
using UnityEngine.UI;

using System;

using Game.State;

using TMPro;

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
			AgeInput.text = DateTime.Now.ToString("dd/MM/yyyy");
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
			return GameState.Instance.AgeController.CanChangeAge;
		}

		void OnAgeChanged(string value) {
			if ( !DateTime.TryParse(value, out var date) ) {
				return;
			}
			var maxBirth = DateTime.Now.AddYears(-21);
			GameState.Instance.AgeController.IsAdult = (date <= maxBirth);
		}
	}
}