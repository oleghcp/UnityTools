﻿using System;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using OlegHcp.Mathematics;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(DateTimeTicksAttribute))]
    internal class DateTimeTicksDrawer : PropertyDrawer
    {
        private string _dateFormat = "MM.dd.yyyy H:mm:ss";
        private GUIContent[] _dateLabels = new[] { new GUIContent("Year"), new GUIContent("Month"), new GUIContent("Day") };
        private GUIContent[] _timeLabels = new[] { new GUIContent("Hour"), new GUIContent("Min"), new GUIContent("Sec"), new GUIContent("..") };
        private int[] _date = new int[3];
        private int[] _time = new int[4];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this).GetTypeCode() != TypeCode.Int64)
            {
                property.isExpanded = false;
                EditorGui.ErrorLabel(position, label, $"Use {nameof(DateTimeTicksAttribute)} with {nameof(Int64)}.");
                return;
            }

            DateTime value = new DateTime(property.longValue);

            if (!property.isExpanded)
            {
                Rect dataRect = EditorGUI.PrefixLabel(position, label);
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none, true);
                GUI.Label(dataRect, value.ToString(_dateFormat));
                return;
            }

            Rect lineRect = EditorGuiUtility.GetLinePosition(position, 0);
            Rect buttonRect = EditorGUI.PrefixLabel(lineRect, label);
            bool pressed = DrawButton(buttonRect, property);
            property.isExpanded = EditorGUI.Foldout(lineRect, property.isExpanded, GUIContent.none, true);

            if (pressed)
                return;

            position.xMin += EditorGuiUtility.IndentLevelOffset;

            _date[0] = value.Year;
            _date[1] = value.Month;
            _date[2] = value.Day;

            lineRect = EditorGuiUtility.GetLinePosition(position, 1);
            EditorGUI.MultiIntField(lineRect, _dateLabels, _date);

            _date[0] = _date[0].ClampMin(1);
            _date[1] = _date[1].Clamp(1, 12);
            _date[2] = _date[2].Clamp(1, DateTime.DaysInMonth(_date[0], _date[1]));

            _time[0] = value.Hour;
            _time[1] = value.Minute;
            _time[2] = value.Second;
            _time[3] = value.Millisecond;

            lineRect = EditorGuiUtility.GetLinePosition(position, 2);
            EditorGUI.MultiIntField(lineRect, _timeLabels, _time);

            _time[0] = _time[0].Clamp(0, 23);
            _time[1] = _time[1].Clamp(0, 59);
            _time[2] = _time[2].Clamp(0, 59);
            _time[3] = _time[3].Clamp(0, 999);

            property.longValue = new DateTime(_date[0], _date[1], _date[2], _time[0], _time[1], _time[2], _time[3]).Ticks;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f;

            return EditorGUIUtility.singleLineHeight;
        }

        private bool DrawButton(in Rect pos, SerializedProperty property)
        {
            bool pressed = GUI.Button(pos, "Now");

            if (pressed)
                property.longValue = DateTime.Now.Ticks;

            return pressed;
        }
    }
}
