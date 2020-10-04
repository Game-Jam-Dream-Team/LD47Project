using Game.State;
using UnityEngine;

namespace Game.Behaviour {
	public sealed class GameStarter : MonoBehaviour {
		void OnEnable() {
			GameState.EnsureExists();
		}
	}
}
