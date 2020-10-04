namespace Game.Common.Quests {
	public sealed class SpawnCommentQuestEvent : BaseQuestEvent {
		public int ParentTweetId;
		public int TweetId;

		public override QuestEventType Type => QuestEventType.SpawnComment;
	}
}
