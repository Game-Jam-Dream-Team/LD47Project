using System.Collections.Generic;

public sealed class GameState : Singleton<GameState> {
	public TweetsController TweetsController { get; private set; }

	readonly List<BaseController> _controllers = new List<BaseController>();

	public GameState() {
		Init();
	}

	void Init() {
		AddControllers();
		UnityContext.Instance.AddUpdateCallback(Update);
	}

	void Update() {
		foreach ( var controller in _controllers ) {
			controller.Update();
		}
	}

	void AddControllers() {
		TweetsController = AddController(new TweetsController());
	}

	T AddController<T>(T controller) where T : BaseController {
		controller.Init();
		_controllers.Add(controller);
		return controller;
	}
}
