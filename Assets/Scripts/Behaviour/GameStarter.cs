using Game.State;
using UnityEngine;

namespace Game.Behaviour {
	public sealed class GameStarter : MonoBehaviour {
		public TweetsFeedView TweetsFeedView;

		void OnEnable() {
			GameState.EnsureExists();
		}
	}
}
