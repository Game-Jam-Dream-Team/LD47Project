using UnityEngine;

using System;
using System.Collections.Generic;

namespace Game.Behaviour {
	[CreateAssetMenu(menuName = "Create SenderCollection")]
	public sealed class SenderCollection : ScriptableObject {
		[Serializable]
		public sealed class SenderInfo {
			public string DisplayName;
			public int    Id;
			public Sprite Avatar;
			[NonSerialized]
			public Sprite OverrideAvatar;
		}

		public List<SenderInfo> SenderInfos = new List<SenderInfo>();

		public void SetOverrideSprite(int senderId, Sprite overrideAvatar) {
			foreach ( var senderInfo in SenderInfos ) {
				if ( senderInfo.Id == senderId ) {
					senderInfo.OverrideAvatar = overrideAvatar;
					return;
				}
			}
			Debug.LogErrorFormat("Can't find SenderInfo for id '{0}'", senderId);
		}

		public SenderInfo GetSenderInfo(int senderId) {
			foreach ( var senderInfo in SenderInfos ) {
				if ( senderInfo.Id == senderId ) {
					return senderInfo;
				}
			}
			Debug.LogErrorFormat("Can't find SenderInfo for id '{0}'", senderId);
			return null;
		}
	}
}
