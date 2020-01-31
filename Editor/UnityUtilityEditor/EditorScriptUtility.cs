using UnityObject = UnityEngine.Object;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEditor;
using UU.Collections;

namespace UUEditor
{
    internal static class EditorScriptUtility
    {
        internal const string Category = "UnityUtility";

        internal static Assembly[] GetAssemblies()
        {
            var files = Directory.GetFiles(@"Library\ScriptAssemblies\", "*.dll", SearchOption.AllDirectories);

            return files.Select(file => Assembly.LoadFrom(file)).ToArray();
        }

        internal static Type[] GetTypes(Assembly[] assemblies, Func<Type, bool> selector)
        {
            List<Type> types = new List<Type>();

            for (int i = 0; i < assemblies.Length; i++)
            {
                types.AddRange(assemblies[i].GetTypes());
            }

            return types.Where(selector).ToArray();
        }

        internal static void CreateAsset(string objectName, string fileName = null)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(objectName);
            string name = string.Concat("Assets/", fileName.HasUsefulData() ? fileName : objectName, ".asset");
            AssetDatabase.CreateAsset(so, name);
            AssetDatabase.SaveAssets();
        }

        internal static void CreateAsset(string objectName, UnityObject rootObject)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(objectName);
            so.name = objectName;
            AssetDatabase.AddObjectToAsset(so, rootObject);
            AssetDatabase.SaveAssets();
        }

        internal static bool DrawCenterButton(string text, float w, float h)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            bool pressed = GUILayout.Button(text, GUILayout.MinWidth(w), GUILayout.MaxHeight(h));
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            return pressed;
        }

        internal static void DrawCenterLabel(string text, float w)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel, GUILayout.Width(w));
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        internal static void DrawWrongTypeMessage(Rect position, GUIContent label, string message)
        {
            EditorGUI.LabelField(position, label);
            EditorGUI.LabelField(EditorGUI.PrefixLabel(position, label), message);
        }

        //The function was taken here: https://gist.github.com/bzgeb
        //God save this guy
        internal static UnityObject[] DrawDropArea(string text, float h)
        {
            Event curEvent = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0f, h, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, text);

            switch (curEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (dropArea.Contains(curEvent.mousePosition))
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

        internal static bool EqualizeSize(SerializedProperty arrayProp, int targetSize, object defVal)
        {
            bool changed = false;

            while (arrayProp.arraySize < targetSize)
            {
                arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
                f_initProp(arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1), defVal);

                changed = true;
            }

            while (arrayProp.arraySize > targetSize)
            {
                arrayProp.DeleteArrayElementAtIndex(arrayProp.arraySize - 1);

                changed = true;
            }

            return changed;
        }

        private static void f_initProp(SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = (int)value;
                    break;

                case SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    break;

                case SerializedPropertyType.Enum:
                    property.enumValueIndex = (int)value;
                    break;
            }
        }

        internal static void MoveToggles(BitArrayMask bits, int length, bool up)
        {
            if (up)
            {
                for (int i = 0; i < length; i++)
                {
                    f_toggleIteration(bits, -1, i, i > 0);
                }
            }
            else
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    f_toggleIteration(bits, 1, i, i < length - 1);
                }
            }
        }

        internal static void MoveElements(SerializedProperty arrayProp, BitArrayMask bits, bool up, object defVal)
        {
            int length = arrayProp.arraySize;

            if (up)
            {
                for (int i = 0; i < length; i++)
                {
                    f_elementIteration(arrayProp, bits, defVal, -1, i, i > 0);
                }
            }
            else
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    f_elementIteration(arrayProp, bits, defVal, 1, i, i < length - 1);
                }
            }
        }

        private static void f_toggleIteration(BitArrayMask bits, int offset, int i, bool condition)
        {
            if (bits.Get(i))
            {
                bits.Set(i, false);

                if (condition)
                    bits.Set(i + offset, true);
            }
        }

        private static void f_elementIteration(SerializedProperty arrayProp, BitArrayMask bits, object defVal, int offset, int i, bool condition)
        {
            if (bits.Get(i))
            {
                if (condition)
                    arrayProp.MoveArrayElement(i, i + offset);
                else
                    f_initProp(arrayProp.GetArrayElementAtIndex(i), defVal);
            }
        }
    }
}
