using System;
using System.Linq;
using Game.Settings;
using Game.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.State {
	public sealed class QuestController : BaseController {
		const int   OtherTweetsCount      = 10;
		const float QuestFinishDelay      = 2.0f;
		const float QuestFinishGlitchTime = 2.0f;

		readonly TweetsController _tweetsController;
		readonly GlitchController _glitchController;

		QuestCollection _questCollection;

		QuestCollection.QuestInfo _upcomingQuestInfo;

		int   _questIndex;
		float _finishTimer;

		public event Action TweetsUpdated = () => {};

		public Tweet[] CurrentTweets { get; private set; } = new Tweet[0];

		public QuestController(TweetsController tweetsController, GlitchController glitchController) {
			_tweetsController = tweetsController;
			_glitchController = glitchController;
		}

		public override void Init() {
			_questCollection = Resources.Load<QuestCollection>("QuestCollection");
			SetupCurrentTweets();
		}

		public override void Update() {
			if ( _upcomingQuestInfo == null ) {
				return;
			}
			_finishTimer += Time.deltaTime;
			if ( _finishTimer < QuestFinishDelay ) {
				return;
			}
			_finishTimer = 0;
			HandleQuestFinish(_upcomingQuestInfo);
			_upcomingQuestInfo = null;
		}

		public bool TryPost(string message) {
			var info = _questCollection.TryGetQuestInfo(_questIndex);
			if ( info == null ) {
				Debug.LogError("No more quests");
				return false;
			}
			if ( info.ReplyId > 0 ) {
				Debug.LogWarning("Answer should be posted as a reply");
				return false;
			}
			return TryHandleAnswer(info, message);
		}

		public bool TryReply(int tweetId, string message) {
			var info = _questCollection.TryGetQuestInfo(_questIndex);
			if ( info == null ) {
				Debug.LogError("No more quests");
				return false;
			}
			if ( info.ReplyId == 0 ) {
				Debug.LogWarning("Answer should be posted as a post");
				return false;
			}
			if ( tweetId != info.ReplyId ) {
				Debug.LogWarning("Answer should be posted as a reply to another post");
				return false;
			}
			return TryHandleAnswer(info, message);
		}

		bool TryHandleAnswer(QuestCollection.QuestInfo info, string message) {
			if ( info.CorrectAnswer == message.Trim() ) {
				Debug.Log("Answer is correct");
				_upcomingQuestInfo = info;
				return true;
			}
			Debug.LogWarning("Answer is not correct");
			return false;
		}

		void HandleQuestFinish(QuestCollection.QuestInfo info) {
			_questIndex++;
			SetupCurrentTweets();
			_glitchController.AddConstantly(info.BaseGlitchIncrease);
			_glitchController.AddOneShot(info.OneShotGlitch, QuestFinishGlitchTime);
			TweetsUpdated();
		}

		void SetupCurrentTweets() {
			if ( !Enum.TryParse<TweetType>("Quest" + _questIndex, out var questType) ) {
				return;
			}
			var currentTweets = _tweetsController.GetRootTweetsByType(
					new TweetType[] { questType, TweetType.Player },
					OtherTweetsCount, TweetType.Filler, TweetType.Generated)
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