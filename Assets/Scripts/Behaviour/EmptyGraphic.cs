﻿using UnityEngine.UI;

public class EmptyGraphic : Graphic {
	protected override void OnPopulateMesh(VertexHelper vh) {
		vh.Clear();
	}
}
