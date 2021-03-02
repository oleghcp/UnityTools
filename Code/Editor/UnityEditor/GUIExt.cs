using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class GUIExt
    {
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

        public static void DrawObjectFields(SerializedObject serializedObject, Predicate<SerializedProperty> ignoreCondition = null)
        {
            using (SerializedProperty iterator = serializedObject.GetIterator())
            {
                for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    if (ignoreCondition != null && ignoreCondition(iterator))
                        continue;

                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
        }

        public static void DrawObjectFields(Rect startLinePsition, SerializedObject serializedObject, Predicate<SerializedProperty> ignoreCondition = null)
        {
            using (SerializedProperty iterator = serializedObject.GetIterator())
            {
                EditorGUI.indentLevel++;

                for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    if (ignoreCondition != null && ignoreCondition(iterator))
                        continue;

                    EditorGUI.PropertyField(startLinePsition, iterator, true);
                    startLinePsition.y += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}
