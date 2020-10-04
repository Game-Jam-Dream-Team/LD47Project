using System.Collections.Generic;
using Game.State;
using SG;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Behaviour {
	public sealed class TweetsFeedView2 : MonoBehaviour, IDragHandler {
		public RectTransform TweetViewsRoot;
		public TweetView     Prefab;
		public float         TopOffset;
		public float         BottomOffset;

		Pool _pool;

		List<TweetView> _instances = new List<TweetView>();

		void Awake() {
			_pool = new Pool("Tweet", Prefab.gameObject, gameObject, 10, PoolInflationType.INCREMENT);
		}

		void Start() {
			UpdateLayout();
			GameState.Instance.QuestController.TweetsUpdated += UpdateLayout;
		}

		void OnDestroy() {
			GameState.Instance.QuestController.TweetsUpdated -= UpdateLayout;
		}

		void UpdateLayout() {
			foreach ( var instance in _instances ) {
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
				y -= 200;
				foreach ( var comment in tweet.CommentIds ) {
					var commentInstance = Add(y);
					commentInstance.TweetRoot.SetActive(true);
					commentInstance.ReplyRoot.SetActive(false);
					var commentTweet = tc.GetTweetById(comment);
					commentInstance.InitTweet(tc, qc, commentTweet, false);
					y -= 200;
				}
				var replyInstance = Add(y);
				replyInstance.InitReply(tweet);
				y -= 200;
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
			if ( diff > 0 ) {
				var firstInstance = _instances[0];
				if ( firstInstance.transform.position.y > TopOffset ) {
					_instances.RemoveAt(0);
					_instances.Add(firstInstance);
					firstInstance.transform.position = _instances[_instances.Count - 2].transform.position + Vector3.down * 200;
				}
			} else {
				var lastInstance = _instances[_instances.Count - 1];
				if ( lastInstance.transform.position.y < BottomOffset ) {
					_instances.RemoveAt(_instances.Count - 1);
					_instances.Insert(0, lastInstance);
					lastInstance.transform.position = _instances[1].transform.position + Vector3.up * 200;
				}
			}
		}
	}
}