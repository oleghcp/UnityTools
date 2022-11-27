﻿using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtility.Inspector;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(BitList))]
    [CustomPropertyDrawer(typeof(IntMask))]
    internal class BitMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGui.ErrorLabel(position, label, $"Use {nameof(DrawFlagsAttribute)}.");
        }
    }
}
