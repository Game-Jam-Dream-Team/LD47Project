using System;
using System.Collections.Generic;

namespace Game.Common {
	public sealed class Tweet {
		public readonly int    Id;
		public readonly int    SenderId;
		public readonly int    ImageId;

		public readonly List<int> CommentIds;

		string _message;

		public TweetType Type;

		int _likesCount;
		int _retweetsCount;

		bool _playerRetweet;

		public int CommentsCount => CommentIds.Count;

		public int LikesCount {
			get => _likesCount;
			set {
				if ( _likesCount == value ) {
					return;
				}
				_likesCount = value;
				OnLikesCountChanged?.Invoke(_likesCount);
			}
		}

		public int RetweetsCount {
			get => _retweetsCount;
			set {
				if ( _retweetsCount == value ) {
					return;
				}
				_retweetsCount = value;
				OnRetweetsCountChanged?.Invoke(_retweetsCount);
			}
		}

		public string Message {
			get => _message;
			set {
				if ( _message == value ) {
					return;
				}
				_message = value;
				OnMessageChanged?.Invoke(_message);
			}
		}

		public bool PlayerRetweet {
			get => _playerRetweet;
			set {
				if ( _playerRetweet == value ) {
					return;
				}
				_playerRetweet = value;
				OnPlayerRetweetChanged?.Invoke(_playerRetweet);
			}
		}

		public event Action<int>    OnCommentsCountChanged;
		public event Action<int>    OnLikesCountChanged;
		public event Action<int>    OnRetweetsCountChanged;
		public event Action<string> OnMessageChanged;
		public event Action<bool>   OnPlayerRetweetChanged;

		public Tweet(int id, TweetType type, int senderId, string message, int imageId, List<int> commentIds) {
			Id         = id;
			Type       = type;
			SenderId   = senderId;
			ImageId    = imageId;
			CommentIds = commentIds;

			Message = message;
		}

		public void AddComment(int commentId) {
			CommentIds.Add(commentId);
			OnCommentsCountChanged?.Invoke(CommentsCount);
		}

		public void RemoveComment(int commentId) {
			if ( CommentIds.Remove(commentId) ) {
				OnCommentsCountChanged?.Invoke(CommentsCount);
			}
		}
	}
}
