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
		var tweetSender          = -1;
		using ( var sr = new StringReader(tweetsContainer.text) ) {
			while ( true ) {
				var line = sr.ReadLine();
				if ( line == null ) {
					if ( composedTweetMessage.Length > 0 ) {
						var message = composedTweetMessage.ToString();
						AddTweet(tweetSender, message);
					}
					break;
				}
				if ( line == Separator ) {
					if ( composedTweetMessage.Length > 0 ) {
						var message = composedTweetMessage.ToString();
						AddTweet(tweetSender, message);
						composedTweetMessage.Clear();
						tweetSender = -1;
					}
				} else {
					if ( (tweetSender == -1) && (int.TryParse(line, out tweetSender)) ) {
					} else {
						composedTweetMessage.AppendLine(line);
					}
				}
			}
		}
	}

	void AddTweet(int senderId, string message) {
		var id    = message.GetHashCode();
		var tweet = new Tweet(id, senderId, message);
		_tweets.Add(tweet);
	}
}
