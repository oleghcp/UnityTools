using System.Runtime.CompilerServices;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorGui
    {
        internal static void DrawWrongTypeMessage(Rect position, GUIContent label, string message)
        {
            Rect rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(rect, message);
        }

        //The functions was taken from here: https://gist.github.com/bzgeb
        //God save this guy
        public static UnityObject[] DropArea(string text, float height)
        {
            Rect position = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));
            return DropArea(position, text);
        }

        public static UnityObject[] DropArea(Rect position, string text)
        {
            GUI.Box(position, text);

            Event curEvent = Event.current;
            switch (curEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (position.Contains(curEvent.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (curEvent.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            return DragAndDrop.objectReferences;
                        }
                    }
                    break;
            }

            return null;
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
