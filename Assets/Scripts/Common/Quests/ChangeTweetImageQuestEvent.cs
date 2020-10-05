using UnityEngine;

using System;

namespace Game.Common.Quests {
	[Serializable]
	public sealed class ChangeTweetImageQuestEvent : BaseQuestEvent {
		public int    TweetId;
		public Sprite NewImage;
		public bool   AgeRestricted;

		public override QuestEventType Type => QuestEventType.ChangeTweetImage;
	}
}
