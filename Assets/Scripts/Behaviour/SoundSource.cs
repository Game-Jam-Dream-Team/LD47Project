using UnityEngine;

namespace Game.Behaviour {
	[RequireComponent(typeof(AudioSource))]
	public class SoundSource : MonoBehaviour {
		AudioSource _source;

		public static SoundSource Current { get; private set; }

		public AudioClip ClickClip;

		public void Awake() {
			Current = this;
			_source = GetComponent<AudioSource>();
		}

		public void OnDestroy() {
			Current = null;
		}

		public void PlayClick() {
			if ( ClickClip ) {
				_source.PlayOneShot(ClickClip);
			}
		}
	}
}