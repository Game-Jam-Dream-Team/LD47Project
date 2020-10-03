public abstract class Singleton<T> where T : Singleton<T>, new() {
	static T _instance;
	public static T Instance {
		get {
			EnsureExists();
			return _instance;
		}
	}

	public static void EnsureExists() {
		if ( _instance == null ) {
			_instance = new T();
		}
	}
}
