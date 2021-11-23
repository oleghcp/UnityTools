using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtilityEditor.Window;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorUtilityExt
    {
        public const string SCRIPT_FIELD = "m_Script";
        public const string ASSET_NAME_FIELD = "m_Name";

        private static MethodInfo _clearFunc;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SaveProject()
        {
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }

        public static void ClearConsoleWindow()
        {
            if (_clearFunc == null)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Editor));
                Type type = assembly.GetType("UnityEditor.LogEntries");
                _clearFunc = type.GetMethod("Clear");
            }
            _clearFunc.Invoke(null, null);
        }

#if UNITY_2019_3_OR_NEWER
        public static string ConvertToSystemTypename(string managedReferenceFieldTypename)
        {
            if (managedReferenceFieldTypename.IsNullOrEmpty())
                return string.Empty;

            string[] typeSplitString = managedReferenceFieldTypename.Split(' ');
            return $"{typeSplitString[1]}, {typeSplitString[0]}";
        }

        public static Type GetTypeFromSerializedPropertyTypename(string managedReferenceTypename)
        {
            if (managedReferenceTypename.IsNullOrEmpty())
                return null;

            return Type.GetType(ConvertToSystemTypename(managedReferenceTypename));
        }
#endif

        public static Type GetFieldType(PropertyDrawer drawer)
        {
            Type fieldType = drawer.fieldInfo.FieldType;
            return fieldType.IsArray ? fieldType.GetElementType() : fieldType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisplayDropDownList(Vector2 position, string[] displayedOptions, Predicate<int> checkEnabled, Action<int> onItemSelected)
        {
            DisplayDropDownList(new Rect(position, Vector2.zero), displayedOptions, checkEnabled, onItemSelected);
        }

        public static void DisplayDropDownList(in Rect buttonRect, string[] displayedOptions, Predicate<int> checkEnabled, Action<int> onItemSelected)
        {
            DropDownWindow list = ScriptableObject.CreateInstance<DropDownWindow>();

            for (int i = 0; i < displayedOptions.Length; i++)
            {
                int index = i;
                list.AddItem(displayedOptions[i], checkEnabled(i), () => onItemSelected(index));
            }

            list.ShowMenu(buttonRect);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisplayMultiSelectableList(Vector2 position, BitList flags, string[] displayedOptions, Action<BitList> onClose = null)
        {
            DisplayMultiSelectableList(position, flags, displayedOptions, onClose);
        }

        public static void DisplayMultiSelectableList(in Rect buttonRect, BitList flags, string[] displayedOptions, Action<BitList> onClose = null)
        {
            DropDownWindow.CreateForFlags(buttonRect, flags, displayedOptions, onClose);
        }

        //The functions based on https://gist.github.com/bzgeb/3800350
        //God save this guy
        public static bool GetDroppedObjects(in Rect position, out UnityObject[] result)
        {
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
                            result = DragAndDrop.objectReferences;
                            return true;
                        }
                    }
                    break;
            }

            result = null;
            return false;
        }

        public static void OpenScriptableObjectCode(UnityObject obj)
        {
            EditorApplication.ExecuteMenuItem("Assets/Open C# Project");

            using (SerializedObject serializedObject = new SerializedObject(obj))
            {
                SerializedProperty prop = serializedObject.FindProperty(SCRIPT_FIELD);
                string filePath = AssetDatabase.GetAssetPath(prop.objectReferenceValue);
                System.Diagnostics.Process.Start("devenv", "/edit " + filePath);
                prop.Dispose();
            }
        }
    }
}
