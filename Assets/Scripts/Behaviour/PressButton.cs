using UnityEngine.EventSystems;
using UnityEngine.UI;

using System;

namespace Game.Behaviour {
	public sealed class PressButton : Button {
		public event Action OnPressed;
		public event Action OnReleased;

		public override void OnPointerDown(PointerEventData eventData) {
			base.OnPointerDown(eventData);
			OnPressed?.Invoke();
		}

		public override void OnPointerUp(PointerEventData eventData) {
			base.OnPointerUp(eventData);
			OnReleased?.Invoke();
		}

		public override void OnPointerClick(PointerEventData eventData) {
			base.OnPointerClick(eventData);
			SoundSource.Current.PlayClick();
		}
	}
}
