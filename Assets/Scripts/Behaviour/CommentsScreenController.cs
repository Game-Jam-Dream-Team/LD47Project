using UnityEngine;
using UnityEngine.UI;

public sealed class CommentsScreenController : MonoBehaviour {
	public TweetView         MainTweetView;
	public CommentsFeedView  CommentsFeedView;
	public PlayerCommentView PlayerCommentView;

	bool _isShown;

	public bool TryShow(Tweet mainTweet) {
		if ( _isShown ) {
			return false;
		}

		MainTweetView.TryCommonInit();
		MainTweetView.InitTweet(GameState.Instance.TweetsController, mainTweet);
		LayoutRebuilder.ForceRebuildLayoutImmediate(MainTweetView.transform as RectTransform);

		CommentsFeedView.InitTweet(mainTweet);
		PlayerCommentView.InitTweet(mainTweet);

		_isShown = true;
		return true;
	}

	public bool TryHide() {
		if ( !_isShown ) {
			return false;
		}

		MainTweetView.DeinitTweet();
		CommentsFeedView.DeinitTweet();
		PlayerCommentView.DeinitTweet();

		_isShown = false;
		return true;
	}
}
