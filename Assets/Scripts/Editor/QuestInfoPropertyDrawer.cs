using UnityEditor;
using UnityEngine;

using System;
using System.Reflection;

using Game.Common.Quests;
using Game.Settings;

namespace Game.Editor {
	[CustomPropertyDrawer(typeof(QuestCollection.QuestInfo))]
	public sealed class QuestInfoPropertyDrawer : PropertyDrawer {
		const float Separator = 10f;
		const float Offset    = 18f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.PropertyField(position, property, label, true);
			var defaultHeight = EditorGUI.GetPropertyHeight(property);

			var questEventsProp = property.FindPropertyRelative("QuestEvents");
			var types = (QuestEventType[]) Enum.GetValues(typeof(QuestEventType));
			for ( var i = 0; i < types.Length; i++ ) {
				var questEventType = types[i];
				if ( GUI.Button(
					new Rect(position.x, position.y + Separator + defaultHeight + i * Offset, position.width, Offset),
					$"Add {questEventType.ToString()} event") ) {
					var className = $"Game.Common.Quests.{questEventType}QuestEvent";
					var type      = Assembly.GetAssembly(typeof(BaseQuestEvent)).GetType(className);
					if ( type == null ) {
						Debug.LogErrorFormat("Can't get type for string '{0}'", className);
						continue;
					}
					var newValue = Activator.CreateInstance(type);
					var index    = questEventsProp.arraySize;
					questEventsProp.InsertArrayElementAtIndex(index);
					var val = questEventsProp.GetArrayElementAtIndex(index);
					if ( newValue != null ) {
						val.managedReferenceValue = newValue;
					}
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var types = (QuestEventType[])Enum.GetValues(typeof(QuestEventType));
			return EditorGUI.GetPropertyHeight(property) + Separator + Offset * types.Length;
		}
	}
}
