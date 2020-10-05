using System;

namespace Game.Common.Quests {
	[Serializable]
	public sealed class ChangeTweetLikesCountQuestEvent : BaseQuestEvent {
		public int TweetId;
		public int NewLikesCount;

		public override QuestEventType Type => QuestEventType.ChangeTweetLikesCount;
	}
}
