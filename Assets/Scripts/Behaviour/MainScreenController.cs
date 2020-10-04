using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace Game.Behaviour {
	public sealed class MainScreenController : MonoBehaviour {
		const float ShowNewPostAnimDuration  = 0.5f;
		const float HideNewPostAnimDuration  = 0.5f;

		public NewPostScreenController NewPostScreenController;
		public Button                  BackButton;
		public Button                  NewPostButton;
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

		void TryGoToTweetsFeed() {
			if ( TryHideNewPostScreen() ) {
			}
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
