using System;

namespace Game.State {
	public sealed class AgeController : BaseController {
		bool _canChangeAge;
		bool _isAdult;

		public bool CanChangeAge {
			get => _canChangeAge;
			set {
				if ( _canChangeAge == value ) {
					return;
				}
				_canChangeAge = value;
				OnCanChangeAgeChanged?.Invoke(_canChangeAge);
			}
		}

		public bool IsAdult {
			get => _isAdult;
			set {
				if ( _isAdult == value ) {
					return;
				}
				_isAdult = value;
				OnIsAdultChanged?.Invoke(_isAdult);
			}
		}

		public event Action<bool> OnCanChangeAgeChanged;
		public event Action<bool> OnIsAdultChanged;
	}
}
