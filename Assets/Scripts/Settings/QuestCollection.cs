using UnityEngine;

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
