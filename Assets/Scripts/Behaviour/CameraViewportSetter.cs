using UnityEngine;

namespace Game.Behaviour {
	[RequireComponent(typeof(Camera))]
	public sealed class CameraViewportSetter : MonoBehaviour {
		public void Start() {
			var cam   = GetComponent<Camera>();
			var rect  = cam.rect;
			var ratio = 600.0f / 1920;
			rect.x     = (1 - ratio) / 2;
			rect.width = ratio;
			cam.rect   = rect;
		}
	}
}