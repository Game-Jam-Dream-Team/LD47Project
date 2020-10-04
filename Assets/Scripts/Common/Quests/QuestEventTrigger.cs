using System;

namespace Game.Common.Quests {
	[Serializable]
	public sealed class QuestEventTrigger {
		public QuestEventTriggerType Type = QuestEventTriggerType.Unknown;
		public string                Arg;
	}
}
