using System;
using System.Collections.Generic;

namespace Game.Common {
	public sealed class Tweet {
		public readonly int    Id;
		public readonly int    SenderId;
		public readonly string Message;
		public readonly int    ImageId;

		public readonly List<int> CommentIds;

		int _likesCount;
		int _retweetsCount;

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

		public event Action<int> OnCommentsCountChanged;
		public event Action<int> OnLikesCountChanged;
		public event Action<int> OnRetweetsCountChanged;

		public Tweet(int id, int senderId, string message, int imageId, List<int> commentIds) {
			Id         = id;
			SenderId   = senderId;
			Message    = message;
			ImageId    = imageId;
			CommentIds = commentIds;
		}

		public void AddComment(int commentId) {
			CommentIds.Add(commentId);
			OnCommentsCountChanged?.Invoke(CommentsCount);
		}
	}
}
