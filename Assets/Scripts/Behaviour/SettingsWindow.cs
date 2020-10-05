using UnityEngine;
using UnityEngine.UI;

using System;
using System.Globalization;

using Game.State;

using TMPro;

namespace Game.Behaviour {
	public class SettingsWindow : MonoBehaviour {
		public GameObject     AgeBlock;
		public TMP_InputField AgeInput;
		public Button         SettingsButton;
		public Button         BackButton;

		string _oldValue;

		void Awake() {
			gameObject.SetActive(false);
			AgeBlock.SetActive(false);
			SettingsButton.onClick.AddListener(Show);
			BackButton.onClick.AddListener(Hide);
			AgeInput.text = DateTime.Now.ToString("dd/MM/yyyy");
			_oldValue = AgeInput.text;
			AgeInput.onValueChanged.AddListener(OnAgeChanged);
		}

		void Show() {
			gameObject.SetActive(true);
			AgeBlock.SetActive(IsAgeShouldBeShown());
			SoundSource.Current.PlayClick();
		}

		void Hide() {
			gameObject.SetActive(false);
			SoundSource.Current.PlayClick();
		}

		bool IsAgeShouldBeShown() {
			return GameState.Instance.AgeController.CanChangeAge;
		}

		void OnAgeChanged(string value) {
			if ( !DateTime.TryParseExact(value, new[] { "dd/mm/yyyy", "d/mm/yyyy", "dd/m/yyyy", "d/m/yyyy" },
				CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ) {
				AgeInput.text = _oldValue;
				return;
			}
			var maxBirth = DateTime.Now.AddYears(-21);
			GameState.Instance.AgeController.IsAdult = (date <= maxBirth);
			_oldValue = value;
		}
	}
}