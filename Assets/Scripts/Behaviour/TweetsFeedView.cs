using UnityEngine;
using UnityEngine.UI;

public sealed class TweetsFeedView : MonoBehaviour {
	public RectTransform          TweetViewsRoot;
	public LoopVerticalScrollRect TweetViewsScrollRect;

	public void Init() {
		LayoutRebuilder.ForceRebuildLayoutImmediate(TweetViewsRoot);
		TweetViewsScrollRect.verticalNormalizedPosition = 1f;
	}
}
