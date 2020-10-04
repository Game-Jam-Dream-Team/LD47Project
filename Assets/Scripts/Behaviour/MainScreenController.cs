using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Game.Common;
using JetBrains.Annotations;

namespace Game.Behaviour {
	public sealed class MainScreenController : MonoBehaviour {
		const float ShowCommentsAnimDuration = 0.5f;
		const float HideCommentsAnimDuration = 0.5f;

		public TweetsFeedView           TweetsFeedView;
		public CommentsScreenController CommentsScreenController;
		public Button                   BackButton;
		[Space]
		public RectTransform TweetsFeedViewRoot;
		public RectTransform TweetsFeedViewRootShowPos;
		public RectTransform TweetsFeedViewRootHidePos;
		[Space]
		public RectTransform CommentsScreenRoot;
		public RectTransform CommentsScreenRootShowPos;
		public RectTransform CommentsScreenRootHidePos;

		Tween _curAnim;

		void Start() {
			BackButton.onClick.AddListener(TryHideCommentsScreen);
		}

		[UsedImplicitly]
		public void TryShowCommentsScreen(Tweet mainTweet) {
			if ( CommentsScreenController.TryShow(mainTweet) ) {
				_curAnim?.Kill(true);
				_curAnim = DOTween.Sequence()
					.Insert(0f,
						TweetsFeedViewRoot.DOAnchorPos(TweetsFeedViewRootHidePos.anchoredPosition,
							ShowCommentsAnimDuration))
					.Insert(0f,
						CommentsScreenRoot.DOAnchorPos(CommentsScreenRootShowPos.anchoredPosition,
							ShowCommentsAnimDuration));
			}
		}

		public void TryHideCommentsScreen() {
			if ( CommentsScreenController.TryHide() ) {
				_curAnim?.Kill(true);
				_curAnim = DOTween.Sequence()
					.Insert(0f,
						TweetsFeedViewRoot.DOAnchorPos(TweetsFeedViewRootShowPos.anchoredPosition,
							HideCommentsAnimDuration))
					.Insert(0f,
						CommentsScreenRoot.DOAnchorPos(CommentsScreenRootHidePos.anchoredPosition,
							HideCommentsAnimDuration));
			}
		}
	}
}
