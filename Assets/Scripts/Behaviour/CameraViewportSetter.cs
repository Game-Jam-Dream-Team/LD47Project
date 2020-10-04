using UnityEngine;

namespace Game.Behaviour {
	[RequireComponent(typeof(Camera))]
	public sealed class CameraViewportSetter : MonoBehaviour {
		public RectTransform Target;

		public void Start() {
			var width = Target.rect.width;
			var ratio = width / Screen.width;
			var cam   = GetComponent<Camera>();
			var rect  = cam.rect;
			rect.x     = (1 - ratio) / 2;
			rect.width = ratio;
			cam.rect   = rect;
		}
	}
}