using System.Collections.Generic;

namespace Game.Common.Quests {
	public sealed class StartRetweetChainQuestEvent : BaseQuestEvent {
		public int       ChainId;
		public List<int> TweetIds = new List<int>();

		public override QuestEventType Type => QuestEventType.StartRetweetChain;
	}
}
