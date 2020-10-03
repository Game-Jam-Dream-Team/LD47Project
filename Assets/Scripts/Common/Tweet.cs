using System;

public sealed class Tweet {
	public readonly int    Id;
	public readonly int    SenderId;
	public readonly string Message;
	public readonly string ImageId;

	int _commentsCount;
	int _likesCount;
	int _retweetsCount;

	public int CommentsCount {
		get => _commentsCount;
		set {
			if ( _commentsCount == value ) {
				return;
			}
			_commentsCount = value;
			OnCommentsCountChanged?.Invoke(_commentsCount);
		}
	}

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

	public Tweet(int id, int senderId, string message, string imageId = null) {
		Id       = id;
		SenderId = senderId;
		Message  = message;
		ImageId  = imageId;
	}
}
