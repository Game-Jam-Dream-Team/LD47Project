using UnityEngine;

namespace Game.Behaviour {
	[RequireComponent(typeof(AudioSource))]
	public class SoundSource : MonoBehaviour {
		AudioSource _source;

		public static SoundSource Current { get; private set; }

		public AudioClip ClickClip;
		public AudioClip TweetSentClip;
		public AudioClip QuestFinishedClip;
		public AudioClip InitialGifClip;
		public AudioClip FinalClip;

		public void Awake() {
			Current = this;
			_source = GetComponent<AudioSource>();
		}

		public void OnDestroy() {
			Current = null;
		}

		public void PlayClick() {
			TryPlayClip(ClickClip);
		}

		public void PlayTweetSent() {
			TryPlayClip(TweetSentClip);
		}

		public void PlayQuestFinished() {
			TryPlayClip(QuestFinishedClip);
		}

		public void PlayInitialGif() {
			TryPlayClip(InitialGifClip);
		}

		public void PlayFinal() {
			TryPlayClip(FinalClip);
		}

		void TryPlayClip(AudioClip clip) {
			if ( clip ) {
				_source.PlayOneShot(clip);
			}
		}
	}
}