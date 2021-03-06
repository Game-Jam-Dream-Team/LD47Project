using UnityEngine;

using System;
using System.Collections.Generic;

namespace Game.Behaviour {
	[CreateAssetMenu(menuName = "Create TweetSpritesCollection")]
	public sealed class TweetSpritesCollection : ScriptableObject {
		[Serializable]
		public sealed class TimedSprite {
			public Sprite Sprite;
			public float  Time;
			public float  Glitch;
		}

		[Serializable]
		public sealed class TweetSpriteInfo {
			public int               Id;
			public bool              AgeRestricted;
			public Sprite            Sprite;
			public List<TimedSprite> Sprites;
		}

		sealed class OverrideSprite {
			public int    TweetId;
			public Sprite Sprite;
			public bool   AgeRestricted;
		}

		public List<TweetSpriteInfo> TweetSpriteInfos = new List<TweetSpriteInfo>();

		readonly List<OverrideSprite> _overrideSprites = new List<OverrideSprite>();

		public void SetOverrideSprite(int tweetId, Sprite overrideSprite, bool ageRestricted) {
			foreach ( var os in _overrideSprites ) {
				if ( os.TweetId == tweetId ) {
					os.Sprite        = overrideSprite;
					os.AgeRestricted = ageRestricted;
					return;
				}
			}
			_overrideSprites.Add(new OverrideSprite {
				TweetId       = tweetId,
				Sprite        = overrideSprite,
				AgeRestricted = ageRestricted
			});
		}

		public bool IsAgeRestricted(int tweetId, int tweetSpriteId) {
			foreach ( var overrideSprite in _overrideSprites ) {
				if ( overrideSprite.TweetId == tweetId ) {
					return overrideSprite.AgeRestricted;
				}
			}
			foreach ( var tweetSpriteInfo in TweetSpriteInfos ) {
				if ( tweetSpriteInfo.Id == tweetSpriteId ) {
					return tweetSpriteInfo.AgeRestricted;
				}
			}
			Debug.LogErrorFormat("Can't find TweetSpriteInfo for id '{0}'", tweetSpriteId);
			return false;
		}

		public Sprite GetTweetSprite(int tweetId, int tweetSpriteId) {
			foreach ( var overrideSprite in _overrideSprites ) {
				if ( overrideSprite.TweetId == tweetId ) {
					return overrideSprite.Sprite;
				}
			}
			foreach ( var tweetSpriteInfo in TweetSpriteInfos ) {
				if ( tweetSpriteInfo.Id == tweetSpriteId ) {
					return tweetSpriteInfo.Sprite;
				}
			}
			Debug.LogErrorFormat("Can't find TweetSpriteInfo for id '{0}'", tweetSpriteId);
			return null;
		}

		public List<TimedSprite> GetTweetSprites(int tweetSpriteId) {
			if ( tweetSpriteId == -1 ) {
				return null;
			}
			foreach ( var tweetSpriteInfo in TweetSpriteInfos ) {
				if ( tweetSpriteInfo.Id == tweetSpriteId ) {
					return tweetSpriteInfo.Sprites;
				}
			}
			Debug.LogErrorFormat("Can't find TweetSpriteInfo for id '{0}'", tweetSpriteId);
			return null;
		}
	}
}
