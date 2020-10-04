using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using Game.Common;
using Game.State;

namespace Game.Behaviour {
	public sealed class CommentsFeedView : MonoBehaviour {
		public         ScrollRect    CommentViewsScrollRect;
		public         RectTransform CommentViewsParent;
		[Space] public GameObject    CommentViewPrefab;

		readonly List<TweetView> _activeTweetViews = new List<TweetView>();
		readonly List<TweetView> _tweetViewsPool   = new List<TweetView>();

		public void InitTweet(Tweet mainTweet) {
			var tc = GameState.Instance.TweetsController;
			foreach ( var commentId in mainTweet.CommentIds ) {
				var tweetView = GetFreeTweetView();
				tweetView.InitTweet(tc, tc.GetTweetById(commentId));
			}

			// for some reason needs both ForceRebuildLayoutImmediate calls to rebuild properly
			LayoutRebuilder.ForceRebuildLayoutImmediate(CommentViewsParent);
			CommentViewsScrollRect.verticalNormalizedPosition = 1f;
			LayoutRebuilder.ForceRebuildLayoutImmediate(CommentViewsParent);
		}

		public void DeinitTweet() {
			foreach ( var activeTweetView in _activeTweetViews ) {
				activeTweetView.DeinitTweet();
				activeTweetView.gameObject.SetActive(false);
				_tweetViewsPool.Add(activeTweetView);
			}
			_activeTweetViews.Clear();
		}

		TweetView GetFreeTweetView() {
			TweetView tweetView;
			if ( _tweetViewsPool.Count > 0 ) {
				tweetView = _tweetViewsPool[0];
				_tweetViewsPool.RemoveAt(0);
			} else {
				var tweetViewGo = Instantiate(CommentViewPrefab, CommentViewsParent);
				tweetView = tweetViewGo.GetComponent<TweetView>();
				tweetView.TryCommonInit();
				_activeTweetViews.Add(tweetView);
			}
			tweetView.gameObject.SetActive(true);
			return tweetView;
		}
	}
}
