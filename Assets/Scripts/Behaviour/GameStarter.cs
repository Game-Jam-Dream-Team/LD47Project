using UnityEngine;

public sealed class GameStarter : MonoBehaviour {
	public TweetsFeedView TweetsFeedView;
	[Space]
	public SenderCollection SenderCollection;

	void OnEnable() {
		GameState.EnsureExists();
	}

	void Start() {
		TweetsFeedView.Init(SenderCollection);
	}
}
