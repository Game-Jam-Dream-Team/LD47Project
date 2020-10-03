using UnityEngine;
using UnityEngine.UI;

public sealed class TweetsFeedView : MonoBehaviour {
	public RectTransform TweetViewsRoot;
	public ScrollRect    TweetViewsScrollRect;
	[Space]
	public GameObject TweetViewPrefab;

	public void Init(SenderCollection senderCollection) {
		foreach ( var tweet in GameState.Instance.TweetsController.Tweets ) {
			var tweetViewGo = Instantiate(TweetViewPrefab, TweetViewsRoot);
			var tweetView   = tweetViewGo.GetComponent<TweetView>();
			tweetView.Init(tweet, senderCollection);
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(TweetViewsRoot);
		TweetViewsScrollRect.verticalNormalizedPosition = 1f;
	}
}
