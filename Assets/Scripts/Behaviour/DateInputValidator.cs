using UnityEngine;

using System;
using System.Globalization;

using TMPro;

namespace Game.Behaviour {
	[CreateAssetMenu(menuName = "Create DateInputValidator")]
	public sealed class DateInputValidator : TMP_InputValidator {
		public override char Validate(ref string text, ref int pos, char ch) {
			if ( !char.IsNumber(ch) || (pos >= 10) ) {
				return '\0';
			}
			var newText = text;
			var newPos = pos;
			newText = (pos >= newText.Length - 4)
				? newText.Remove(pos, 1).Insert(pos, ch.ToString())
				: newText.Insert(newPos, ch.ToString());
			if ( DateTime.TryParseExact(newText, new[] { "dd/mm/yyyy", "d/mm/yyyy", "dd/m/yyyy", "d/m/yyyy" },
				CultureInfo.InvariantCulture, DateTimeStyles.None, out _) ) {
				text = newText;
				pos  = ++newPos;
			} else {
				return '\0';
			}
			return ch;
		}
	}
}
