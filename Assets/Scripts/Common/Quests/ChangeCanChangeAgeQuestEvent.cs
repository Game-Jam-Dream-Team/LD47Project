using System;

namespace Game.Common.Quests {
	[Serializable]
	public sealed class ChangeCanChangeAgeQuestEvent : BaseQuestEvent {
		public bool NewCanChangeAge;

		public override QuestEventType Type => QuestEventType.ChangeCanChangeAge;
	}
}
