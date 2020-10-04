using UnityEngine;
using UnityEngine.UI;

using Game.Common;

using DG.Tweening;
using JetBrains.Annotations;

namespace Game.Behaviour {
	public sealed class MainScreenController : MonoBehaviour {
		const float ShowCommentsAnimDuration = 0.5f;
		const float HideCommentsAnimDuration = 0.5f;
		const float ShowNewPostAnimDuration  = 0.5f;
		const float HideNewPostAnimDuration  = 0.5f;

		public CommentsScreenController CommentsScreenController;
		public NewPostScreenController  NewPostScreenController;
		public Button                   BackButton;
		public Button                   NewPostButton;
		[Space]
		public RectTransform TweetsFeedViewRoot;
		public RectTransform TweetsFeedViewRootShowPos;
		public RectTransform TweetsFeedViewRootHidePos;
		[Space]
		public RectTransform CommentsScreenRoot;
		public RectTransform CommentsScreenRootShowPos;
		public RectTransform CommentsScreenRootHidePos;
		[Space]
		public RectTransform NewPostScreenRoot;
		public RectTransform NewPostScreenRootShowPos;
		public RectTransform NewPostScreenRootHidePos;

		Tween _curAnim;

		void Start() {
			NewPostScreenController.CommonInit(this);

			BackButton.onClick.AddListener(TryGoToTweetsFeed);
			NewPostButton.onClick.AddListener(TryShowNewPostScreen);
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

		void TryGoToTweetsFeed() {
			if ( TryHideCommentsScreen() ) {
			} else if ( TryHideNewPostScreen() ) {
			}
		}

		bool TryHideCommentsScreen() {
			if ( CommentsScreenController.TryHide() ) {
				_curAnim?.Kill(true);
				_curAnim = DOTween.Sequence()
					.Insert(0f,
						TweetsFeedViewRoot.DOAnchorPos(TweetsFeedViewRootShowPos.anchoredPosition,
							HideCommentsAnimDuration))
					.Insert(0f,
						CommentsScreenRoot.DOAnchorPos(CommentsScreenRootHidePos.anchoredPosition,
							HideCommentsAnimDuration));
				return true;
			}
			return false;
		}

		public bool TryHideNewPostScreen() {
			if ( NewPostScreenController.TryHide() ) {
				_curAnim?.Kill(true);
				_curAnim = DOTween.Sequence()
					.Insert(0f,
						NewPostScreenRoot.DOAnchorPos(NewPostScreenRootHidePos.anchoredPosition,
							HideNewPostAnimDuration));
				return true;
			}
			return false;
		}

		void TryShowNewPostScreen() {
			if ( NewPostScreenController.TryShow() ) {
				_curAnim?.Kill(true);
				_curAnim = DOTween.Sequence()
					.Insert(0f, NewPostScreenRoot.DOAnchorPos(NewPostScreenRootShowPos.anchoredPosition, ShowNewPostAnimDuration));
				;
			}
		}
	}
}
