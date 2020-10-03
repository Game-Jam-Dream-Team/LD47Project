using UnityEngine;
using UnityEngine.UI;

using System;

using TMPro;

public sealed class TweetView : MonoBehaviour {
	const string NameTemplate = "<b>{0}</b><color=#aaaaaa>{1}</color>";

	public Image      Avatar;
	public TMP_Text   NameText;
	public TMP_Text   MessageText;
	public Button     CommentButton;
	public TMP_Text   CommentsText;
	public Button     LikeButton;
	public TMP_Text   LikesText;
	public Button     RetweetButton;
	public TMP_Text   RetweetsText;
	public GameObject TweetImageRoot;
	public Image      TweetImage;

	Tweet _tweet;

	void OnDestroy() {
		if ( _tweet != null ) {
			_tweet.OnCommentsCountChanged -= UpdateCommentsCount;
			_tweet.OnLikesCountChanged    -= UpdateLikesCount;
			_tweet.OnRetweetsCountChanged -= UpdateRetweetsCount;
		}
	}

	public void Init(Tweet tweet, SenderCollection senderCollection) {
		_tweet = tweet;

		var senderInfo = senderCollection.GetSenderInfo(_tweet.SenderId);
		Avatar.sprite = senderInfo.Avatar;
		InitSender(senderInfo.DisplayName);
		MessageText.text = _tweet.Message;
		UpdateCommentsCount(_tweet.CommentsCount);
		UpdateLikesCount(_tweet.LikesCount);
		UpdateRetweetsCount(_tweet.RetweetsCount);

		if ( string.IsNullOrEmpty(_tweet.ImageId) ) {
			TweetImageRoot.SetActive(false);
		} else {
			TweetImageRoot.SetActive(true);
			// TODO: init tweet image
		}

		CommentButton.onClick.AddListener(OnCommentsClick);
		LikeButton.onClick.AddListener(OnLikesClick);
		RetweetButton.onClick.AddListener(OnRetweetsClick);

		_tweet.OnCommentsCountChanged += UpdateCommentsCount;
		_tweet.OnLikesCountChanged    += UpdateLikesCount;
		_tweet.OnRetweetsCountChanged += UpdateRetweetsCount;
	}

	void InitSender(string displayName) {
		var index = displayName.IndexOf("@", StringComparison.InvariantCulture);
		if ( index > 0 ) {
			NameText.text = string.Format(NameTemplate, displayName.Substring(0, index),
				displayName.Substring(index, displayName.Length - index));
		} else {
			NameText.text = displayName;
		}
	}

	void OnCommentsClick() {

	}

	void OnLikesClick() {

	}

	void OnRetweetsClick() {

	}

	void UpdateCommentsCount(int commentsCount) {
		CommentsText.text = commentsCount.ToString();
	}

	void UpdateLikesCount(int likesCount) {
		LikesText.text = likesCount.ToString();
	}

	void UpdateRetweetsCount(int retweetsCount) {
		RetweetsText.text = retweetsCount.ToString();
	}
}
