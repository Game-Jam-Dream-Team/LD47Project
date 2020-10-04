using UnityEngine.UI;

namespace Game.Behaviour {
	public class EmptyGraphic : Graphic {
		protected override void OnPopulateMesh(VertexHelper vh) {
			vh.Clear();
		}
	}
}
