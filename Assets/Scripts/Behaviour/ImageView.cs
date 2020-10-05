using System.Collections.Generic;
using Game.Common;
using Game.State;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Behaviour {
	public sealed class ImageView : MonoBehaviour {
		public GameObject Background;
		public Image      Image;

		TweetSpritesCollection _tweetSpritesCollection;

		Tweet                                    _curTweet;
		List<TweetSpritesCollection.TimedSprite> _sprites;

		int   _index;
		float _timer;
		float _delay;

		void Awake() {
			Close();
			_tweetSpritesCollection = Resources.Load<TweetSpritesCollection>("TweetSpritesCollection");
		}

		void Start() {
			GameState.Instance.TweetsController.ImageClick += OnImageClick;
		}

		void OnDestroy() {
			GameState.Instance.TweetsController.ImageClick -= OnImageClick;
		}

		void Update() {
			if ( _sprites == null ) {
				return;
			}
			_timer += Time.deltaTime;
			if ( _timer < _delay ) {
				return;
			}
			_timer = 0;
			CycleSprites();
		}

		void CycleSprites() {
			_index++;
			if ( _index >= _sprites.Count ) {
				_sprites = null;
				Close();
				return;
			}
			SetupTimedSprite(_index);
		}

		void OnImageClick(Tweet tweet) {
			Open(tweet);
		}

		void Open(Tweet tweet) {
			Background.SetActive(true);
			_curTweet = tweet;
			_sprites  = _tweetSpritesCollection.GetTweetSprites(tweet.ImageId);
			if ( (_sprites != null) && (_sprites.Count > 0) ) {
				_index = 0;
				_timer = 0;
				SetupTimedSprite(_index);
			} else {
				_sprites = null;
				Image.sprite = _tweetSpritesCollection.GetTweetSprite(_curTweet.Id, tweet.ImageId);
			}
			Image.enabled = true;
		}

		public void Close() {
			if ( _sprites != null ) {
				return;
			}
			Background.SetActive(false);
			Image.enabled = false;
			if ( _curTweet != null ) {
				var tweetId = _curTweet.Id;
				_curTweet = null;
				GameState.Instance.QuestController.OnImageShowFinished(tweetId);
			}
		}

		void SetupTimedSprite(int index) {
			var entry = _sprites[index];
			var sprite = entry.Sprite;
			Image.sprite = sprite;
			_delay = entry.Time;
			Image.GetComponent<RectTransform>()
				.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sprite.rect.height);
			var glitch = entry.Glitch;
			if ( glitch > 0 ) {
				GameState.Instance.GlitchController.AddOneShot(glitch, _delay / 2);
			}
		}
	}
}