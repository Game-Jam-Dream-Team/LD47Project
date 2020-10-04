using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Behaviour {
	[RequireComponent(typeof(Image))]
	public sealed class StaticImageView : MonoBehaviour {
		public List<TweetSpritesCollection.TimedSprite> Sprites;

		public int   RandomEndingIndex;
		public int   MinCycleRange;
		public int   MinStartCycle;
		public float RandomEndingChance;

		Image _image;

		int   _index;
		float _timer;
		float _delay;

		int _cycle;
		int _lastRandomCycle;

		void Awake() {
			_image = GetComponent<Image>();
		}

		void Start() {
			SetupTimedSprite(0);
		}

		void Update() {
			_timer += Time.deltaTime;
			if ( _timer < _delay ) {
				return;
			}
			_timer = 0;
			CycleSprites();
		}

		void CycleSprites() {
			_index++;
			if ( _index == Sprites.Count ) {
				_index = 0;
				_cycle++;
			}
			if ( _index == RandomEndingIndex ) {
				if ( (_cycle < MinStartCycle) || ((_cycle - _lastRandomCycle) < MinCycleRange) || (Random.value > RandomEndingChance) ) {
					_index = 0;
				} else {
					_lastRandomCycle = _cycle;
				}
				_cycle++;
			}
			SetupTimedSprite(_index);
		}

		void SetupTimedSprite(int index) {
			_image.sprite = Sprites[index].Sprite;
			_image.enabled = Sprites[index].Sprite;
			if ( _image.enabled ) {
				_image.SetNativeSize();
			}
			_delay = Sprites[index].Time;
		}
	}
}