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
			var newText = text.Insert(pos, ch.ToString());
			if ( DateTime.TryParseExact(text.Insert(pos, ch.ToString()),
				     new[] { "dd/mm/yyyy", "d/mm/yyyy", "dd/m/yyyy", "d/m/yyyy" },
				     CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
			     || DateTime.TryParseExact(newText = text.Remove(pos, 1).Insert(pos, ch.ToString()),
				     new[] { "dd/mm/yyyy", "d/mm/yyyy", "dd/m/yyyy", "d/m/yyyy" },
				     CultureInfo.InvariantCulture, DateTimeStyles.None, out _) ) {
				text = newText;
				++pos;
			} else {
				return '\0';
			}
			return ch;
		}
	}
}
