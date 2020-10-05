using System.Collections.Generic;
using Game.Utils;

namespace Game.State {
	public sealed class GameState : Singleton<GameState> {
		public TweetsController      TweetsController   { get; private set; }
		public ProgressController    ProgressController { get; private set; }
		public GlitchController      GlitchController   { get; private set; }
		public QuestController       QuestController    { get; private set; }

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
			var unityContext = UnityContext.Instance;
			TweetsController   = AddController(new TweetsController());
			ProgressController = AddController(new ProgressController(unityContext));
			GlitchController   = AddController(new GlitchController());
			QuestController    = AddController(new QuestController(TweetsController, GlitchController, ProgressController));
		}

		T AddController<T>(T controller) where T : BaseController {
			controller.Init();
			_controllers.Add(controller);
			return controller;
		}
	}
}
