namespace Game.Common.Quests {
	public sealed class RemoveTweetQuestEvent : BaseQuestEvent {
		public int TweetId;

		public override QuestEventType Type => QuestEventType.RemoveTweet;
	}
}
