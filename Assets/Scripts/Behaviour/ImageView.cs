using Game.Common;
using Game.State;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Behaviour {
	public sealed class ImageView : MonoBehaviour {
		public GameObject Background;
		public Image      Image;

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
			Image.sprite  = _tweetSpritesCollection.GetTweetSprite(tweet.ImageId);
			Image.enabled = true;
		}

		public void Close() {
			Background.SetActive(false);
			Image.enabled = false;
		}
	}
}