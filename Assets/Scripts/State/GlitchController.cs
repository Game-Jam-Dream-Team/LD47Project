using UnityEngine;

namespace Game.State {
	public sealed class GlitchController : BaseController {
		float _baseLevel;
		float _oneShotLevel;
		float _oneShotDuration;

		public float CurrentLevel => Mathf.Clamp01(_baseLevel + _oneShotLevel);

		public bool IsOneShootHandled;

		public void AddConstantly(float value) {
			_baseLevel += value;
		}

		public void AddOneShot(float value, float duration) {
			_oneShotLevel    += value;
			_oneShotDuration += duration;
			IsOneShootHandled = false;
		}

		public override void Update() {
			_oneShotDuration -= Time.deltaTime;
			if ( _oneShotDuration > 0 ) {
				return;
			}
			_oneShotLevel    = 0;
			_oneShotDuration = 0;
		}
	}
}