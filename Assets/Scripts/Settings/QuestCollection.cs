using UnityEngine;

using Game.Common.Quests;

using System;
using System.Collections.Generic;

namespace Game.Settings {
	[CreateAssetMenu(menuName = "Create QuestCollection")]
	public sealed class QuestCollection : ScriptableObject {
		[Serializable]
		public sealed class QuestInfo {
			public int    ReplyId;
			public string CorrectAnswer;
			public float  OneShotGlitch;
			public float  BaseGlitchIncrease;
			[Space] [SerializeReference]
			public List<BaseQuestEvent> QuestEvents = new List<BaseQuestEvent>();
		}

		public List<QuestInfo> QuestInfos = new List<QuestInfo>();

		public QuestInfo TryGetQuestInfo(int index) {
			if ( index < QuestInfos.Count ) {
				return QuestInfos[index];
			}
			return null;
		}
	}
}
