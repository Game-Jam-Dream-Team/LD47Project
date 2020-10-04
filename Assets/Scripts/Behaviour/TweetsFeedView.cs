using UnityEngine;
using UnityEngine.UI;

using Game.State;

using JetBrains.Annotations;

namespace Game.Behaviour {
	public sealed class TweetsFeedView : MonoBehaviour {
		public RectTransform          TweetViewsRoot;
		public LoopVerticalScrollRect TweetViewsScrollRect;

		public void Init() {
			LayoutRebuilder.ForceRebuildLayoutImmediate(TweetViewsRoot);
			TweetViewsScrollRect.verticalNormalizedPosition = 1f;
			TweetViewsScrollRect.totalCount = -1;
		}

		[UsedImplicitly]
		public void UpdateLayout() {
			TweetViewsScrollRect.RefreshCells();
			LayoutRebuilder.ForceRebuildLayoutImmediate(TweetViewsRoot);
			LayoutRebuilder.ForceRebuildLayoutImmediate(TweetViewsRoot); // for some reason one call isn't enough
		}

		void Start() {
			Refill();
			GameState.Instance.QuestController.TweetsUpdated += UpdateLayout;
		}

		void OnDestroy() {
			GameState.Instance.QuestController.TweetsUpdated -= UpdateLayout;
		}

		void Refill() {
			TweetViewsScrollRect.RefillCells();
		}
	}
}
