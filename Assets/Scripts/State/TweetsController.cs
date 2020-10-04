using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Random = UnityEngine.Random;

public sealed class TweetsController : BaseController {
	const string TweetsContainerPathPrefix = "Tweets_";
	const string Separator                 = "######";
	const string TweetIdPrefix             = "#id:";
	const string TweetCommentIdsPrefix     = "#commentIds:";
	const float  UpdateInterval            = 1f;

	readonly Dictionary<TweetType, string> _tweetTypeToPathPostfix = new Dictionary<TweetType, string> {
		{ TweetType.Filler,    "Filler" },
		{ TweetType.Generated, "Generated" },
		{ TweetType.Popular,   "Popular" },
		{ TweetType.Hacker,    "Hacker" },
		{ TweetType.Theme1,    "Theme1" },
		{ TweetType.Theme2,    "Theme2" },
		{ TweetType.Theme3,    "Theme3" },
	};

	readonly List<Tweet> _tweets     = new List<Tweet>();
	readonly List<Tweet> _rootTweets = new List<Tweet>();

	float _timer;

	public int RootTweetsCount => _rootTweets.Count;

	public event Action<Tweet> ImageClick = _ => {};

	public override void Init() {
		LoadTweets();
		foreach ( var tweet in _tweets ) {
			if ( _tweets.All(x => !x.CommentIds.Contains(tweet.Id)) ) {
				_rootTweets.Add(tweet);
			}
		}
		_tweets.TrimExcess();
		_rootTweets.TrimExcess();
		var count = _rootTweets.Count;
		while ( count > 1 ) {
			--count;
			var rand = Random.Range(0, count + 1);
			var val  = _rootTweets[rand];
			_rootTweets[rand]  = _rootTweets[count];
			_rootTweets[count] = val;
		}
	}

	public override void Update() {
		_timer += Time.deltaTime;
		if ( _timer >= UpdateInterval ) {
			foreach ( var tweet in _tweets ) {
				tweet.LikesCount    += Random.Range(0, 10);
				tweet.RetweetsCount += Random.Range(0, 2);
			}
			_timer -= UpdateInterval;
		}
	}

	public Tweet GetRootTweetByIndex(int index) {
		return _rootTweets[index];
	}

	public Tweet GetTweetById(int tweetId) {
		foreach ( var tweet in _tweets ) {
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

	void LoadTweets() {
		var tweetTypes = (TweetType[])Enum.GetValues(typeof(TweetType));
		foreach ( var tweetType in tweetTypes ) {
			LoadTweetsForType(tweetType);
		}
	}

	void LoadTweetsForType(TweetType tweetType) {
		var path = TweetsContainerPathPrefix + _tweetTypeToPathPostfix[tweetType];
		var tweetsContainer = Resources.Load<TextAsset>(path);
		if ( !tweetsContainer ) {
			Debug.LogErrorFormat("TweetsController: can't load TweetsContainer on path '{0}'", path);
			return;
		}
		var composedTweetMessage = new StringBuilder();
		var tweetId              = -1;
		var tweetSenderId        = -1;
		var tweetImageId         = -1;
		var tweetCommentIds      = new List<int>();
		using ( var sr = new StringReader(tweetsContainer.text) ) {
			while ( true ) {
				var line = sr.ReadLine();
				if ( line == null ) {
					if ( composedTweetMessage.Length > 0 ) {
						var message = composedTweetMessage.ToString();
						AddTweet(tweetId, tweetSenderId, message, tweetImageId, tweetCommentIds);
					}
					break;
				}
				if ( line == Separator ) {
					if ( composedTweetMessage.Length > 0 ) {
						var message = composedTweetMessage.ToString();
						AddTweet(tweetId, tweetSenderId, message, tweetImageId, tweetCommentIds);
						composedTweetMessage.Clear();
						tweetId       = -1;
						tweetSenderId = -1;
						tweetImageId  = -1;
						tweetCommentIds.Clear();
					}
				} else {
					if ( tweetSenderId == -1  ) {
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
					} else {
						composedTweetMessage.AppendLine(line);
					}
				}
			}
		}
	}

	void AddTweet(int tweetId, int senderId, string message, int imageId, List<int> commentIds) {
		if ( tweetId == -1 ) {
			tweetId = message.GetHashCode();
		}
		var tweet = new Tweet(tweetId, senderId, message, imageId, new List<int>(commentIds));
		_tweets.Add(tweet);
	}
}
