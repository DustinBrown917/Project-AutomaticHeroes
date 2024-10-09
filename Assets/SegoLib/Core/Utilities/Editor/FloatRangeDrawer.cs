using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SegoLib.Editor
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect minRect = new Rect(position.x, position.y, position.width * 0.5f - 5, position.height);
            Rect maxRect = new Rect(position.x + position.width * 0.5f + 5, position.y, position.width * 0.5f - 5, position.height);

            SerializedProperty minProp = property.FindPropertyRelative("m_min");
            SerializedProperty maxProp = property.FindPropertyRelative("m_max");

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30f;

            EditorGUI.PropertyField(minRect, minProp, new GUIContent("Min"));

            if (minProp.floatValue > maxProp.floatValue)
                minProp.floatValue = maxProp.floatValue;

            EditorGUI.PropertyField(maxRect, maxProp, new GUIContent("Max"));

            if (maxProp.floatValue < minProp.floatValue)
                maxProp.floatValue = minProp.floatValue;

            EditorGUI.indentLevel = indent;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.EndProperty();
        }
    }
}
