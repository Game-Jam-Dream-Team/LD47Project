using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public sealed class ProgressController : BaseController {
	const string Url = "https://konhit.xyz/ld47server";

	readonly UnityContext _unityContext;

	string _sessionId;

	public ProgressController(UnityContext unityContext) {
		_unityContext = unityContext;
	}

	public override void Init() {
		StartGame();
	}

	public void StartGame() {
		_sessionId = Guid.NewGuid().ToString();
		_unityContext.StartCoroutine(StartGameRoutine(_sessionId));
	}

	public void FinishGame(string variant, Action onFinished) =>
		_unityContext.StartCoroutine(FinishGameRoutine(_sessionId, variant, onFinished));

	public void RequestSummary(Action<Dictionary<string, int>> onFinished) =>
		_unityContext.StartCoroutine(RequestSummaryRoutine(onFinished));

	IEnumerator StartGameRoutine(string sessionId) {
		var req = UnityWebRequest.Post($"{Url}/start?sessionId={sessionId}", string.Empty);
		yield return req.SendWebRequest();
		Debug.Log($"StartGame: {req.result}, {req.error}");
	}

	IEnumerator FinishGameRoutine(string sessionId, string variant, Action onFinished) {
		var req = UnityWebRequest.Post($"{Url}/finish?sessionId={sessionId}&variant={variant}", string.Empty);
		yield return req.SendWebRequest();
		Debug.Log($"FinishGame: {req.result}, {req.error}");
		onFinished();
	}

	IEnumerator RequestSummaryRoutine(Action<Dictionary<string, int>> onFinished) {
		var req = UnityWebRequest.Get($"{Url}/summary");
		yield return req.SendWebRequest();
		Debug.Log($"RequestSummaryRoutine: {req.result}, {req.error}");
		if ( req.result != UnityWebRequest.Result.Success ) {
			onFinished(new Dictionary<string, int>());
			yield break;
		}
		var text    = req.downloadHandler.text;
		var jNode   = SimpleJSON.JSON.Parse(text);
		var results = jNode["results"].Linq.ToDictionary(p => p.Key, p => int.Parse(p.Value));
		onFinished(results);
	}
}