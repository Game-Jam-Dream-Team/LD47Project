using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;

using Game.State;

using SG;

namespace Game.Behaviour {
	public sealed class TweetsFeedView2 : MonoBehaviour, IDragHandler {
		public RectTransform TweetViewsRoot;
		public TweetView     Prefab;
		public float         TopOffset;
		public float         BottomOffset;

		Pool _pool;

		List<TweetView> _instances = new List<TweetView>();

		bool _shouldUpdate;

		void Awake() {
			_pool = new Pool("Tweet", Prefab.gameObject, gameObject, 10, PoolInflationType.INCREMENT);
		}

		void Start() {
			UpdateLayout();
			GameState.Instance.QuestController.TweetsUpdated += UpdateLayoutDelayed;
		}

		void OnDestroy() {
			GameState.Instance.QuestController.TweetsUpdated -= UpdateLayoutDelayed;
		}

		void UpdateLayoutDelayed() {
			_shouldUpdate = true;
		}

		void Update() {
			if ( _shouldUpdate ) {
				UpdateLayout();
				_shouldUpdate = false;
			}

			var firstInstance = _instances[0];
			if ( firstInstance.transform.position.y > TopOffset ) {
				_instances.RemoveAt(0);
				_instances.Add(firstInstance);
				var instance = _instances[_instances.Count - 2];
				firstInstance.transform.localPosition =
					instance.transform.localPosition + Vector3.down * instance.GetHeight();
			}
			var lastInstance = _instances[_instances.Count - 1];
			if ( lastInstance.transform.position.y < BottomOffset ) {
				_instances.RemoveAt(_instances.Count - 1);
				_instances.Insert(0, lastInstance);
				var instance = _instances[1];
				lastInstance.transform.localPosition =
					instance.transform.localPosition + Vector3.up * lastInstance.GetHeight();
			}
		}

		void UpdateLayout() {
			foreach ( var instance in _instances ) {
				instance.DeinitTweet();
				instance.PlayerCommentView.DeinitTweet();
				_pool.ReturnObjectToPool(instance.GetComponent<PoolObject>());
			}
			_instances.Clear();
			var tc = GameState.Instance.TweetsController;
			var qc = GameState.Instance.QuestController;
			var y  = 0.0f;
			foreach ( var tweet in qc.CurrentTweets ) {
				var tweetInstance = Add(y);
				tweetInstance.TweetRoot.SetActive(true);
				tweetInstance.ReplyRoot.SetActive(false);
				tweetInstance.InitTweet(tc, qc, tweet);
				y -= tweetInstance.GetHeight();
				foreach ( var comment in tweet.CommentIds ) {
					var commentInstance = Add(y);
					commentInstance.TweetRoot.SetActive(true);
					commentInstance.ReplyRoot.SetActive(false);
					var commentTweet = tc.GetTweetById(comment);
					commentInstance.InitTweet(tc, qc, commentTweet, false);
					y -= commentInstance.GetHeight();
				}
				var replyInstance = Add(y);
				replyInstance.InitReply(tweet);
				y -= replyInstance.GetHeight();
			}
		}

		TweetView Add(float offset) {
			var go = _pool.NextAvailableObject(true);
			go.transform.SetParent(TweetViewsRoot);
			go.transform.localPosition = new Vector3(0, offset);
			var view = go.GetComponent<TweetView>();
			_instances.Add(view);
			return view;
		}

		public void OnDrag(PointerEventData eventData) {
			var diff = eventData.delta.y;
			foreach ( var instance in _instances ) {
				instance.transform.position += Vector3.up * diff;
			}
		}
	}
}