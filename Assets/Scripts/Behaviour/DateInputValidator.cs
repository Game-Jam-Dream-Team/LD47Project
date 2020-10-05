using UnityEngine;

using System;

using TMPro;

namespace Game.Behaviour {
	[CreateAssetMenu(menuName = "Create DateInputValidator")]
	public sealed class DateInputValidator : TMP_InputValidator {
		public override char Validate(ref string text, ref int pos, char ch) {
			if ( !char.IsNumber(ch) || (pos >= 10) ) {
				return '\0';
			}
			if ( pos == 2 || pos == 5 ) {
				++pos;
				return '\0';
			}
			var newText = text.Remove(pos, 1).Insert(pos, ch.ToString());
			char res;
			if ( DateTime.TryParse(newText, out _) ) {
				text = newText;
				res = ch;
			} else {
				res = '\0';
			}
			++pos;
			if ( pos == 2 || pos == 5 ) {
				++pos;
			}
			return res;
		}
	}
}
