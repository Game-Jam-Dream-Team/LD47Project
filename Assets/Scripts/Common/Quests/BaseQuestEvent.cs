using UnityEngine;

using System;

namespace Game.Common.Quests {
	[Serializable]
	public abstract class BaseQuestEvent {
		public QuestEventTrigger Trigger;
		[Space]
		public float OneShotGlitch;
		public float BaseGlitchIncrease;

		public abstract QuestEventType Type { get; }
	}
}
