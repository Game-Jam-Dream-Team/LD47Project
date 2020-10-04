using UnityEngine;

namespace Game.State {
	public sealed class TweetUpdateController : BaseController {
		const float UpdateInterval = 1f;

		readonly QuestController _questController;

		float _timer;

		public TweetUpdateController(QuestController questController) {
			_questController = questController;
		}

		public override void Update() {
			_timer += Time.deltaTime;
			if ( _timer >= UpdateInterval ) {
				// Rewrite
				foreach ( var tweet in _questController.CurrentTweets ) {
					tweet.LikesCount    += Random.Range(0, 10);
					tweet.RetweetsCount += Random.Range(0, 2);
				}
				_timer -= UpdateInterval;
			}
		}
	}
}