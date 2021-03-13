using System.Runtime.CompilerServices;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorGui
    {
        internal static void WrongTypeLabel(Rect position, GUIContent label, string message)
        {
            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(rect, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityObject[] DropArea(Rect position)
        {
            return DropArea(position, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityObject[] DropArea(Rect position, string text)
        {
            return DropArea(position, text, EditorStylesExt.DropArea);
        }

        public static UnityObject[] DropArea(Rect position, string text, GUIStyle style)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);
            UnityObject[] objects = DragAndDropData.GetObjectsById(controlId);

            GUI.Box(position, text, style);

            if (EditorUtilityExt.GetDroppedObjects(position, out var result))
            {
                DragAndDropData.PlaceObjects(controlId, result);
                GUI.changed = true;
            }

            return objects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DropDown(Rect propertyRect, int selectedIndex, string[] displayedOptions)
        {
            return DropDown(propertyRect, null, selectedIndex, displayedOptions);
        }

        public static int DropDown(Rect propertyRect, string label, int selectedIndex, string[] displayedOptions)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard);

            if (label != null)
                propertyRect = EditorGUI.PrefixLabel(propertyRect, new GUIContent(label));

            selectedIndex = DropDownData.GetSelectedIndexById(selectedIndex, controlId);

            if (EditorGUI.DropdownButton(propertyRect, new GUIContent(displayedOptions[selectedIndex]), FocusType.Keyboard))
            {
                EditorUtilityExt.DisplayDropDownList(propertyRect,
                                                     displayedOptions,
                                                     index => index == selectedIndex,
                                                     index => DropDownData.MenuAction(controlId, index));
            }

            return selectedIndex;
        }
    }
}
