using UnityEngine;

using System;

namespace Game.Common.Quests {
	[Serializable]
	public sealed class ChangeSenderAvatarQuestEvent : BaseQuestEvent {
		public int    SenderId;
		public Sprite NewAvatar;

		public override QuestEventType Type => QuestEventType.ChangeSenderAvatar;
	}
}
