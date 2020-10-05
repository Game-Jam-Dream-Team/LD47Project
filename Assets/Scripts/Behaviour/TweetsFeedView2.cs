using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using Game.Common;
using Game.State;

using JetBrains.Annotations;

using SG;

namespace Game.Behaviour {
	public sealed class TweetsFeedView2 : MonoBehaviour, IDragHandler, IScrollHandler {
		const float AnimTime = 1f;

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
			GameState.Instance.QuestController.TweetsUpdated    += UpdateLayoutDelayed;
			GameState.Instance.QuestController.OnCommentSpawned += OnCommentAdded;
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
			var ac = GameState.Instance.AgeController;
			var qc = GameState.Instance.QuestController;
			var y  = 0.0f;
			foreach ( var tweet in qc.CurrentTweets ) {
				var tweetInstance = Add(y);
				tweetInstance.TweetRoot.SetActive(true);
				tweetInstance.ReplyRoot.SetActive(false);
				tweetInstance.InitTweet(this, tc, ac, qc, tweet);
				y -= tweetInstance.GetHeight();
				foreach ( var comment in tweet.CommentIds ) {
					var commentInstance = Add(y);
					commentInstance.TweetRoot.SetActive(true);
					commentInstance.ReplyRoot.SetActive(false);
					var commentTweet = tc.GetTweetById(comment);
					commentInstance.InitTweet(this, tc, ac, qc, commentTweet, false);
					y -= commentInstance.GetHeight();
				}
			}
			StopAllCoroutines();
			StartCoroutine(ScrollAnim());
		}

		bool _isAnimActive;
		float _animTimer;

		IEnumerator ScrollAnim() {
			_isAnimActive = true;
			while ( _animTimer < AnimTime ) {
				_animTimer += Time.deltaTime;
				var diff = 200f;
				foreach ( var instance in _instances ) {
					instance.transform.position += Vector3.up * diff;
				}
				yield return null;
			}
			_animTimer    = 0f;
			_isAnimActive = false;
		}

		TweetView Add(float offset, int overrideIndex = -1) {
			var go = _pool.NextAvailableObject(true);
			go.transform.SetParent(TweetViewsRoot);
			go.transform.localPosition = new Vector3(0, offset);
			var view = go.GetComponent<TweetView>();
			if ( overrideIndex == -1 ) {
				_instances.Add(view);
			} else {
				if ( (overrideIndex < 0) || (overrideIndex > _instances.Count) ) {
					Debug.LogErrorFormat("Invalid overrideIndex '{0}'", overrideIndex);
					return null;
				}
				_instances.Insert(overrideIndex, view);
			}
			return view;
		}

		public void ShowReply(TweetView tweetView) {
			var tweetViewIndex = _instances.IndexOf(tweetView);
			if ( tweetViewIndex < 0 ) {
				Debug.LogError("Can't find TweetView instance");
				return;
			}
			var tweet = tweetView.Tweet;
			TweetView prevTweetView;
			int       index;
			if ( tweet.CommentIds.Count == 0 ) {
				prevTweetView = tweetView;
				index         = tweetViewIndex + 1;
			} else {
				prevTweetView = _instances[(tweetViewIndex + tweet.CommentIds.Count) % _instances.Count];
				index         = (tweetViewIndex + tweet.CommentIds.Count + 1) % _instances.Count;
			}
			var offset = prevTweetView.transform.localPosition.y - prevTweetView.GetHeight();
			var replyTweetView = Add(offset, index);
			replyTweetView.TweetRoot.SetActive(false);
			replyTweetView.ReplyRoot.SetActive(true);
			replyTweetView.InitReply(tweet, this);
			replyTweetView.transform.SetSiblingIndex(prevTweetView.transform.GetSiblingIndex() + 1);
			var instanceOffset = Vector3.down * (replyTweetView.GetHeight() + 25f);
			foreach ( var instance in _instances ) {
				if ( instance == replyTweetView ) {
					continue;
				}
				if ( instance.transform.localPosition.y < prevTweetView.transform.localPosition.y ) {
					instance.transform.Translate(instanceOffset);
				}
			}
		}

		public void HideReply(TweetView tweetView) {
			var tweetViewIndex = _instances.IndexOf(tweetView);
			if ( tweetViewIndex < 0 ) {
				Debug.LogError("Can't find TweetView instance");
				return;
			}
			var tweet = tweetView.Tweet;
			int index;
			if ( tweet.CommentIds.Count == 0 ) {
				index = (tweetViewIndex + 1) % _instances.Count;
			} else {
				index = (tweetViewIndex + tweet.CommentIds.Count + 1) % _instances.Count;
			}
			var replyTweetView = _instances[index];
			_instances.RemoveAt(index);
			var border = replyTweetView.transform.localPosition.y;
			var height = replyTweetView.GetHeight();
			var instanceOffset = Vector3.up * (height + 25f);
			foreach ( var instance in _instances ) {
				if ( instance.transform.localPosition.y <= border ) {
					instance.transform.Translate(instanceOffset);
				}
			}
			replyTweetView.PlayerCommentView.DeinitTweet();
			_pool.ReturnObjectToPool(replyTweetView.GetComponent<PoolObject>());
		}

		[UsedImplicitly]
		public void OnCommentAdded(Tweet parentTweet, int childIndex) {
			TweetView tweetView      = null;
			var       tweetViewIndex = -1;
			for ( var i = 0; i < _instances.Count; i++ ) {
				var instance = _instances[i];
				if ( instance.Tweet == parentTweet ) {
					tweetView      = instance;
					tweetViewIndex = i;
					break;
				}
			}
			if ( !tweetView ) {
				Debug.LogError("Can't find parent TweetView instance");
				return;
			}

			TweetView prevTweetView;
			int       index;
			if ( parentTweet.CommentIds.Count == 1 ) {
				prevTweetView = tweetView;
				index         = tweetViewIndex + 1;
			} else {
				prevTweetView = _instances[(tweetViewIndex + childIndex) % _instances.Count];
				index         = (tweetViewIndex + childIndex + 1) % _instances.Count;
			}
			var offset           = prevTweetView.transform.localPosition.y - prevTweetView.GetHeight();
			var commentTweetView = Add(offset, index);
			var tc = GameState.Instance.TweetsController;
			var ac = GameState.Instance.AgeController;
			var qc = GameState.Instance.QuestController;
			commentTweetView.TweetRoot.SetActive(true);
			commentTweetView.ReplyRoot.SetActive(false);
			commentTweetView.InitTweet(this, tc, ac, qc,
				tc.GetTweetById(parentTweet.CommentIds[childIndex]), false);
			commentTweetView.transform.SetSiblingIndex(prevTweetView.transform.GetSiblingIndex() + 1);
			var instanceOffset = Vector3.down * (commentTweetView.GetHeight() + 50f);
			foreach ( var instance in _instances ) {
				if ( instance == commentTweetView ) {
					continue;
				}
				if ( instance.transform.localPosition.y < prevTweetView.transform.localPosition.y ) {
					instance.transform.Translate(instanceOffset);
				}
			}
			// HideAllReplies();
		}

		public void OnTweetViewRemoved(TweetView tweetView) {
			var tweetViewIndex = _instances.IndexOf(tweetView);
			if ( tweetViewIndex < 0 ) {
				Debug.LogError("Can't find TweetView index");
				return;
			}

			var height = tweetView.GetHeight();
			var border = tweetView.transform.localPosition.y;
			_instances.RemoveAt(tweetViewIndex);
			tweetView.DeinitTweet();
			tweetView.PlayerCommentView.DeinitTweet();
			_pool.ReturnObjectToPool(tweetView.GetComponent<PoolObject>());
			var instanceOffset = Vector3.up * height;
			foreach ( var instance in _instances ) {
				if ( instance.transform.localPosition.y < border ) {
					instance.transform.Translate(instanceOffset);
				}
			}
			HideAllReplies();
		}

		public void UpdateTweetViewSize(TweetView tweetView) {
			var tweetViewIndex = _instances.IndexOf(tweetView);
			if ( tweetViewIndex < 0 ) {
				Debug.LogError("Can't find TweetView index");
				return;
			}
			var nextTweetView = _instances[(tweetViewIndex + 1) % _instances.Count];
			if ( nextTweetView.transform.localPosition.y > tweetView.transform.localPosition.y ) {
				return;
			}
			var diff = tweetView.transform.localPosition.y - tweetView.GetHeight() -
			           nextTweetView.transform.localPosition.y;
			foreach ( var instance in _instances ) {
				if ( instance == tweetView ) {
					continue;
				}
				if ( instance.transform.localPosition.y < tweetView.transform.localPosition.y ) {
					instance.transform.Translate(Vector3.up * diff);
				}
			}
		}

		void HideAllReplies() {
			var copy = new List<TweetView>(_instances);
			foreach ( var instance in copy ) {
				if ( _instances.Contains(instance) && instance.ReplyShown ) {
					HideReply(instance);
					instance.ReplyShown = false;
				}
			}
		}

		public void OnDrag(PointerEventData eventData) {
			if ( _isAnimActive ) {
				return;
			}
			var diff = eventData.delta.y;
			foreach ( var instance in _instances ) {
				instance.transform.position += Vector3.up * diff;
			}
		}

		public void OnScroll(PointerEventData eventData) {
			if ( _isAnimActive ) {
				return;
			}
			var diff = eventData.scrollDelta.y;
			foreach ( var instance in _instances ) {
				instance.transform.position += Vector3.up * -80f * diff;
			}
		}
	}
}