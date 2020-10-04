using System;
using System.Linq;
using Game.Common;
using Random = UnityEngine.Random;

namespace Game.State {
	public sealed class QuestController : BaseController {
		const int OtherTweetsCount = 10;

		readonly TweetsController _tweetsController;

		int _questIndex;

		public Tweet[] CurrentTweets { get; private set; } = new Tweet[0];

		public QuestController(TweetsController tweetsController) {
			_tweetsController = tweetsController;
		}

		public override void Init() {
			SetupCurrentTweets();
		}

		void SetupCurrentTweets() {
			if ( !Enum.TryParse<TweetType>("Quest" + _questIndex, out var questType) ) {
				return;
			}
			var currentTweets = _tweetsController.GetRootTweetsByType(questType, OtherTweetsCount, TweetType.Filler, TweetType.Generated)
				.ToArray();
			Shuffle(currentTweets);
			CurrentTweets = currentTweets;
		}

		void Shuffle(Tweet[] tweets) {
			var count = tweets.Length;
			while ( count > 1 ) {
				--count;
				var rand = Random.Range(0, count + 1);
				var val  = tweets[rand];
				tweets[rand]  = tweets[count];
				tweets[count] = val;
			}
		}
	}
}