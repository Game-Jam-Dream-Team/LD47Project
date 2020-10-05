using System;

namespace Game.Common.Quests {
	[Serializable]
	public sealed class ChangeTweetMessageQuestEvent : BaseQuestEvent {
		public int TweetId;
		public int ReservedTweetId;

		public override QuestEventType Type => QuestEventType.ChangeTweetMessage;
	}
}
