using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

namespace Game.Behaviour {
	public sealed class SplashScreen : MonoBehaviour {
		public Image    Background;
		public Image    Foreground;
		public TMP_Text Text;

		void Start() {
			Text.alpha = 0f;
			Background.color = Color.white;
			Foreground.color = new Color(1f, 1f, 1f, 0f);
			var anim = DOTween.Sequence()
				.AppendInterval(1f)
				.Append(Foreground.DOFade(1f, 1f))
				.Insert(0.3f, Text.DOFade(1f, 1f))
				.AppendCallback(() => Background.color = new Color(1f, 1f,1f, 0f))
				.AppendInterval(2f)
				.Append(Text.DOFade(0f, 0.5f))
				.Insert(3.2f, Background.DOFade(0f, 0.5f));
			anim.onComplete += () => gameObject.SetActive(false);
		}
	}
}
