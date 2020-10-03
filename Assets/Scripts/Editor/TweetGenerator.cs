using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class TweetGenerator {
	[MenuItem("Game/GenerateTweets")]
	public static void Generate() {
		var wc         = new WebClient();
		var regex      = new Regex("<p><span>&ldquo;<\\/span>(.*)<span>&rdquo;<\\/span><\\/p>");
		var results    = new HashSet<string>();
		var duplicates = 0;
		for ( var i = 0; i < 1000; i++ ) {
			var str     = wc.DownloadString("https://thoughts.sushant-kumar.com/");
			var content = WebUtility.HtmlDecode(regex.Match(str).Groups[1].Value);
			if ( !results.Add(content) ) {
				duplicates++;
			}
		}
		var senderCollection = AssetDatabase.LoadAssetAtPath<SenderCollection>("Assets/Prefabs/SenderCollection.asset");
		Debug.Log($"Results: {results.Count}, duplicates: {duplicates}");
		var lines = new List<string>();
		foreach ( var t in results ) {
			var senderIndex = Random.Range(1, senderCollection.SenderInfos.Count);
			lines.Add("######");
			lines.Add(senderIndex.ToString());
			lines.Add(t);
		}
		File.WriteAllLines("Assets/Resources/Tweets_Generated.txt", lines);
	}
}