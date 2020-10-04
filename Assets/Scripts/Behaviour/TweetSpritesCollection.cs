using UnityEngine;

using System;
using System.Collections.Generic;
using UnityEngine.Video;

namespace Game.Behaviour {
	[CreateAssetMenu(menuName = "Create TweetSpritesCollection")]
	public sealed class TweetSpritesCollection : ScriptableObject {
		[Serializable]
		public sealed class TweetSpriteInfo {
			public int       Id;
			public Sprite    Sprite;
			public VideoClip Video;
		}

		public List<TweetSpriteInfo> TweetSpriteInfos = new List<TweetSpriteInfo>();

		public Sprite GetTweetSprite(int tweetSpriteId) {
			foreach ( var tweetSpriteInfo in TweetSpriteInfos ) {
				if ( tweetSpriteInfo.Id == tweetSpriteId ) {
					return tweetSpriteInfo.Sprite;
				}
			}
			Debug.LogErrorFormat("Can't find TweetSpriteInfo for id '{0}'", tweetSpriteId);
			return null;
		}

		public VideoClip GetTweetVideo(int tweetSpriteId) {
			foreach ( var tweetSpriteInfo in TweetSpriteInfos ) {
				if ( tweetSpriteInfo.Id == tweetSpriteId ) {
					return tweetSpriteInfo.Video;
				}
			}
			Debug.LogErrorFormat("Can't find TweetSpriteInfo for id '{0}'", tweetSpriteId);
			return null;
		}
	}
}
