using System;
using System.Collections.Generic;

public sealed class UnityContext : BehaviourSingleton<UnityContext> {
	readonly List<Action> _updateCallbacks = new List<Action>();
	readonly List<Action> _toAdd           = new List<Action>(1);
	readonly List<Action> _toRemove        = new List<Action>(1);

	bool _isUpdating;

	void Update() {
		_isUpdating = true;
		foreach ( var updateCallback in _updateCallbacks ) {
			if ( _toRemove.Contains(updateCallback) ) {
				continue;
			}
			updateCallback?.Invoke();
		}
		_isUpdating = false;
		if ( _toAdd.Count > 0 ) {
			_updateCallbacks.AddRange(_toAdd);
			_toAdd.Clear();
		}
		if ( _toRemove.Count > 0 ) {
			_updateCallbacks.RemoveAll(x => _toRemove.Contains(x));
			_toRemove.Clear();
		}
	}

	public void AddUpdateCallback(Action updateCallback) {
		if ( updateCallback == null ) {
			return;
		}
		if ( _isUpdating ) {
			if ( !_toRemove.Remove(updateCallback) ) {
				_toAdd.Add(updateCallback);
			}
		} else {
			_updateCallbacks.Add(updateCallback);
		}
	}

	public void RemoveUpdateCallback(Action updateCallback) {
		if ( updateCallback == null ) {
			return;
		}
		if ( _isUpdating ) {
			if ( !_toAdd.Remove(updateCallback) ) {
				_toRemove.Add(updateCallback);
			}
		} else {
			_updateCallbacks.Remove(updateCallback);
		}
	}
}
