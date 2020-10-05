using DG.Tweening;
using Game.State;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Behaviour {
	public class FinishView : MonoBehaviour {
		public GameObject  MainView;
		public GameObject  IconInactive;
		public GameObject  IconActive;
		public GameObject  Badge;
		public TMP_Text    BadgeText;
		public ResultsView Results;

		void Awake() {
			gameObject.SetActive(false);
			GameState.Instance.QuestController.GameFinish += OnGameFinish;
		}

		void OnGameFinish() {
			gameObject.SetActive(true);
			SoundSource.Current.PlayFinal();
		}

		void Start() {
			IconActive.SetActive(false);
			IconInactive.SetActive(false);
			Badge.SetActive(false);
			var seq = DOTween.Sequence();
			seq.Append(MainView.transform.DOScale(Vector3.zero, 0.5f));
			seq.AppendCallback(() => IconInactive.SetActive(true));
			seq.AppendInterval(2.0f);
			seq.AppendCallback(() => IconActive.SetActive(true));
			seq.AppendCallback(() => {
				Badge.SetActive(true);
				BadgeText.text = "1";
			});
			for ( var i = 2; i < 100; i++ ) {
				var iCopy    = i;
				var duration = Random.Range(0.01f, 0.15f);
				seq.AppendInterval(duration);
				seq.AppendCallback(() => {
					Badge.transform.localScale = Vector3.one;
					BadgeText.text = iCopy.ToString();
					GameState.Instance.GlitchController.AddOneShot((float)iCopy / 200, duration);
				});
				seq.Append(Badge.transform.DOShakeScale(duration));
			}
			seq.AppendInterval(1.5f);
			seq.AppendCallback(() => {
				Results.gameObject.SetActive(true);
			});
		}

		void OnDestroy() {
			GameState.Instance.QuestController.GameFinish -= OnGameFinish;
		}
	}
}