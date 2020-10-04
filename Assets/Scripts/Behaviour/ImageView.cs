using Game.Common;
using Game.State;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Game.Behaviour {
	public sealed class ImageView : MonoBehaviour {
		public GameObject  Background;
		public Image       Image;
		public VideoPlayer Video;

		TweetSpritesCollection _tweetSpritesCollection;

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

		void OnImageClick(Tweet tweet) {
			Open(tweet);
		}

		void Open(Tweet tweet) {
			Background.SetActive(true);
			var sprite = _tweetSpritesCollection.GetTweetSprite(tweet.ImageId);
			var video = _tweetSpritesCollection.GetTweetVideo(tweet.ImageId);
			Image.sprite  = sprite;
			Image.enabled = !video; // Video overrides sprite
			Video.clip    = video;
			Video.enabled = video;
			Video.Play();
		}

		public void Close() {
			if ( Video.isPlaying ) {
				Debug.LogWarning("Waiting until video was finished");
				return;
			}
			Background.SetActive(false);
			Image.enabled = false;
			Video.enabled = false;
		}
	}
}