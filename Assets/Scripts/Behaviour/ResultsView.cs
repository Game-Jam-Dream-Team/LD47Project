using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.State;
using TMPro;
using UnityEngine;

namespace Game.Behaviour {
	public class ResultsView : MonoBehaviour {
		public TMP_Text ResultsText;

		void Awake() {
			gameObject.SetActive(false);
		}

		void Start() {
			GameState.Instance.ProgressController.RequestSummary(OnSummary);
		}

		void OnSummary(Dictionary<string, int> results) {
			var sb = new StringBuilder();
			var totalCount = results.Sum(p => p.Value);
			var orderedResults = results.OrderBy(r => r.Key);
			foreach ( var result in orderedResults ) {
				var verboseKey = string.IsNullOrEmpty(result.Key)
					? "Still in the loop"
					: $"Finished quest #{int.Parse(result.Key) + 1}";
				sb.AppendLine($"{verboseKey} = {(float)(result.Value)/totalCount:P}");
			}
			ResultsText.text = sb.ToString();
		}
	}
}