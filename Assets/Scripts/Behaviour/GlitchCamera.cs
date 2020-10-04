using Game.State;
using Kino;
using UnityEngine;

namespace Game.Behaviour {
	[RequireComponent(typeof(AnalogGlitch))]
	[RequireComponent(typeof(DigitalGlitch))]
	public sealed class GlitchCamera : MonoBehaviour {
		public float UpdateInterval;

		AnalogGlitch  _analog;
		DigitalGlitch _digital;

		float _timer;

		void Awake() {
			_analog  = GetComponent<AnalogGlitch>();
			_digital = GetComponent<DigitalGlitch>();
		}

		void Update() {
			_timer += Time.deltaTime;
			var gc = GameState.Instance.GlitchController;
			if ( (_timer < UpdateInterval) && gc.IsOneShootHandled) {
				return;
			}
			_timer = 0;
			gc.IsOneShootHandled = true;
			var level = gc.CurrentLevel * 3;
			_analog.colorDrift     = Random.Range(0, level);
			_analog.scanLineJitter = Random.Range(0, level - _analog.colorDrift);
			_digital.intensity     = Random.Range(0, level - _analog.colorDrift - _analog.scanLineJitter);
		}
	}
}