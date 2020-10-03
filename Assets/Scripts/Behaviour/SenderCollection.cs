using UnityEngine;

using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Create SenderCollection")]
public sealed class SenderCollection : ScriptableObject {
	[Serializable]
	public sealed class SenderInfo {
		public string DisplayName;
		public int    Id;
		public Sprite Avatar;
	}

	public List<SenderInfo> SenderInfos = new List<SenderInfo>();

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
