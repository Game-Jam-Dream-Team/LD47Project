using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Random = UnityEngine.Random;

public sealed class TweetsController : BaseController {
	const string TweetsContainerPathPrefix = "Tweets_";
	const string Separator                 = "######";
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

	readonly List<Tweet> _tweets = new List<Tweet>();

	float _timer;

	public int         TweetsCount => _tweets.Count;
	public List<Tweet> Tweets      => new List<Tweet>(_tweets);

	public override void Init() {
		LoadTweets();
		var count = _tweets.Count;
		while ( count > 1 ) {
			--count;
			var rand = Random.Range(0, count + 1);
			var val  = _tweets[rand];
			_tweets[rand]  = _tweets[count];
			_tweets[count] = val;
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

	public Tweet GetTweet(int index) {
		return _tweets[index];
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
		var tweetSenderId        = -1;
		var tweetImageId         = -1;
		using ( var sr = new StringReader(tweetsContainer.text) ) {
			while ( true ) {
				var line = sr.ReadLine();
				if ( line == null ) {
					if ( composedTweetMessage.Length > 0 ) {
						var message = composedTweetMessage.ToString();
						AddTweet(tweetSenderId, message, tweetImageId);
					}
					break;
				}
				if ( line == Separator ) {
					if ( composedTweetMessage.Length > 0 ) {
						var message = composedTweetMessage.ToString();
						AddTweet(tweetSenderId, message, tweetImageId);
						composedTweetMessage.Clear();
						tweetSenderId = -1;
						tweetImageId  = -1;
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
					} else {
						composedTweetMessage.AppendLine(line);
					}
				}
			}
		}
	}

	void AddTweet(int senderId, string message, int imageId) {
		var id    = message.GetHashCode();
		var tweet = new Tweet(id, senderId, message, imageId);
		_tweets.Add(tweet);
	}
}
