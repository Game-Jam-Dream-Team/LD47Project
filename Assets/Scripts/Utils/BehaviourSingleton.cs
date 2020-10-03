using UnityEngine;

public abstract class BehaviourSingleton<T> : MonoBehaviour where T : BehaviourSingleton<T> {
	static T _instance;
	public static T Instance {
		get {
			if ( !_instance ) {
				var go = new GameObject($"[{typeof(T).Name}]");
				_instance = go.AddComponent<T>();
			}
			return _instance;
		}
	}
}
