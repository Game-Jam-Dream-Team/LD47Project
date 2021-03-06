using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game.Common;

using Random = UnityEngine.Random;

namespace Game.State {
	public sealed class TweetsController : BaseController {
		public const int PlayerId = 0;

		const string TweetsContainerPathPrefix = "Tweets_";
		const string Separator                 = "######";
		const string TweetIdPrefix             = "#id:";
		const string TweetCommentIdsPrefix     = "#commentIds:";
		const string TweetLikesPrefix          = "#likes:";
		const string TweetRetweetsPrefix       = "#retweets:";

		readonly List<Tweet> _allTweets     = new List<Tweet>();
		readonly List<Tweet> _allRootTweets = new List<Tweet>();

		public event Action<Tweet> ImageClick = _ => { };

		public override void Init() {
			LoadAllTweets();
			foreach ( var tweet in _allTweets ) {
				if ( _allTweets.All(x => !x.CommentIds.Contains(tweet.Id)) ) {
					_allRootTweets.Add(tweet);
				} else {
					tweet.Type = TweetType.Comment;
				}
			}
			_allTweets.TrimExcess();
			_allRootTweets.TrimExcess();
		}

		public Tweet GetTweetById(int tweetId) {
			foreach ( var tweet in _allTweets ) {
				if ( tweet.Id == tweetId ) {
					return tweet;
				}
			}
			Debug.LogErrorFormat("Can't find tweet with id '{0}'", tweetId);
			return null;
		}

		public void ClickImage(Tweet tweet) {
			ImageClick.Invoke(tweet);
		}

		public IEnumerable<Tweet> GetRootTweetsByType(TweetType[] requiredTypes, int otherTypesCount, params TweetType[] otherTypes) {
			return
				_allRootTweets
					.Where(t => requiredTypes.Contains(t.Type))
					.Concat(_allRootTweets.Where(t => otherTypes.Contains(t.Type)).Take(otherTypesCount));
		}

		public void AddTweet(Tweet tweet) {
			_allTweets.Add(tweet);
			_allRootTweets.Add(tweet);
		}

		public void AddComment(Tweet mainTweet, Tweet commentTweet) {
			_allTweets.Add(commentTweet);
			mainTweet.AddComment(commentTweet.Id);
		}

		public void RemoveTweet(Tweet tweet) {
			if ( _allTweets.Remove(tweet) ) {
				if ( !_allRootTweets.Remove(tweet) ) {
					foreach ( var parentTweet in _allTweets.Where(x => x.CommentIds.Contains(tweet.Id)) ) {
						parentTweet.RemoveComment(tweet.Id);
					}
				}
			}
		}

		public void ReplaceTweetMessage(int tweetId, string newMessage) {
			var tweet = GetTweetById(tweetId);
			if ( tweet == null ) {
				return;
			}
			tweet.Message = newMessage;
		}

		public bool GetPlayerRetweet(int tweetId) {
			var tweet = GetTweetById(tweetId);
			if ( tweet == null ) {
				return false;
			}
			return tweet.PlayerRetweet;
		}

		public void SetPlayerRetweet(int tweetId, bool playerRetweet) {
			var tweet = GetTweetById(tweetId);
			if ( tweet == null ) {
				return;
			}
			if ( tweet.PlayerRetweet == playerRetweet ) {
				return;
			}
			tweet.PlayerRetweet  = playerRetweet;
			tweet.RetweetsCount += playerRetweet ? 1 : -1;
		}

		public bool GetPlayerLike(int tweetId) {
			var tweet = GetTweetById(tweetId);
			if ( tweet == null ) {
				return false;
			}
			return tweet.PlayerLike;
		}

		public void SetPlayerLike(int tweetId, bool playerLike) {
			var tweet = GetTweetById(tweetId);
			if ( tweet == null ) {
				return;
			}
			if ( tweet.PlayerLike == playerLike ) {
				return;
			}
			tweet.PlayerLike = playerLike;
			tweet.LikesCount += playerLike ? 1 : -1;
		}

		void LoadAllTweets() {
			var tweetTypes = ((TweetType[]) Enum.GetValues(typeof(TweetType))).ToList();
			tweetTypes.Remove(TweetType.Temporary);
			tweetTypes.Remove(TweetType.Comment);
			tweetTypes.Remove(TweetType.Player);
			foreach ( var tweetType in tweetTypes ) {
				LoadTweetsForType(tweetType);
			}
		}

		void LoadTweetsForType(TweetType tweetType) {
			var path            = TweetsContainerPathPrefix + tweetType;
			var tweetsContainer = Resources.Load<TextAsset>(path);
			if ( !tweetsContainer ) {
				Debug.LogErrorFormat("TweetsController: can't load TweetsContainer on path '{0}'", path);
				return;
			}
			var composedTweetMessage = new StringBuilder();
			var tweetId              = -1;
			var tweetSenderId        = -1;
			var tweetImageId         = -1;
			var tweetLikes           = Random.Range(10, 1000);
			var tweetRetweets        = Random.Range(2, 100);
			var tweetCommentIds      = new List<int>();
			using ( var sr = new StringReader(tweetsContainer.text) ) {
				while ( true ) {
					var line = sr.ReadLine();
					if ( line == null ) {
						if ( composedTweetMessage.Length > 0 ) {
							var message = composedTweetMessage.ToString();
							AddTweet(tweetId, tweetType, tweetSenderId, message, tweetImageId, tweetCommentIds,
								tweetLikes, tweetRetweets);
						}
						break;
					}
					if ( line == Separator ) {
						if ( composedTweetMessage.Length > 0 ) {
							var message = composedTweetMessage.ToString();
							AddTweet(tweetId, tweetType, tweetSenderId, message, tweetImageId, tweetCommentIds,
								tweetLikes, tweetRetweets);
							composedTweetMessage.Clear();
							tweetId       = -1;
							tweetSenderId = -1;
							tweetImageId  = -1;
							tweetLikes    = Random.Range(10, 1000);
							tweetRetweets = Random.Range(2, 100);
							tweetCommentIds.Clear();
						}
					} else {
						if ( tweetSenderId == -1 ) {
							if ( !int.TryParse(line, out tweetSenderId) ) {
								var split = line.Split('|');
								if ( (split.Length != 2) || !int.TryParse(split[0], out tweetSenderId) ||
								     !int.TryParse(split[1], out tweetImageId) ) {
									Debug.LogErrorFormat("Can't parse first tweet line '{0}'", line);
								}
							}
						} else if ( line.StartsWith(TweetIdPrefix) ) {
							if ( !int.TryParse(line.Substring(TweetIdPrefix.Length, line.Length - TweetIdPrefix.Length),
								out tweetId) ) {
								Debug.LogErrorFormat("Can't parse tweet id from line '{0}'", line);
							}
						} else if ( line.StartsWith(TweetCommentIdsPrefix) ) {
							var split = line.Substring(TweetCommentIdsPrefix.Length,
								line.Length - TweetCommentIdsPrefix.Length).Split(',');
							foreach ( var commentIdStr in split ) {
								if ( !int.TryParse(commentIdStr, out var commentId) ) {
									Debug.LogErrorFormat("Can't parse comment ids from line '{0}'", line);
									break;
								}
								tweetCommentIds.Add(commentId);
							}
						} else if ( line.StartsWith(TweetLikesPrefix) ) {
							if ( !int.TryParse(
								line.Substring(TweetLikesPrefix.Length, line.Length - TweetLikesPrefix.Length),
								out tweetLikes) ) {
								Debug.LogErrorFormat("Can't parse tweet likes from line '{0}'", line);
							}
						} else if ( line.StartsWith(TweetRetweetsPrefix) ) {
							if ( !int.TryParse(
								line.Substring(TweetRetweetsPrefix.Length, line.Length - TweetRetweetsPrefix.Length),
								out tweetRetweets) ) {
								Debug.LogErrorFormat("Can't parse tweet retweets from line '{0}'", line);
							}
						} else {
							composedTweetMessage.AppendLine(line);
						}
					}
				}
			}
		}

		void AddTweet(int tweetId, TweetType type, int senderId, string message, int imageId, List<int> commentIds,
			int likes, int retweets) {
			if ( tweetId == -1 ) {
				tweetId = message.GetHashCode();
			}
			var tweet =
				new Tweet(tweetId, type, senderId, message, imageId, new List<int>(commentIds)) {
					LikesCount    = likes,
					RetweetsCount = retweets
				};
			_allTweets.Add(tweet);
		}
	}
}
