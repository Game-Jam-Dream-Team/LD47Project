using UnityEngine;
using UnityEngine.UI;

namespace Game.Behaviour {
	public sealed class TweetsFeedView : MonoBehaviour {
		public RectTransform          TweetViewsRoot;
		public LoopVerticalScrollRect TweetViewsScrollRect;

		public void Init() {
			LayoutRebuilder.ForceRebuildLayoutImmediate(TweetViewsRoot);
			TweetViewsScrollRect.verticalNormalizedPosition = 1f;
		}
	}
}
