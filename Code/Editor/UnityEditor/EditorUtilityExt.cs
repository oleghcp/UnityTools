using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public const string ASSET_EXTENSION = ".asset";
        public const string ASSET_FOLDER = "Assets/";
        internal const string TEMPLATES_FOLDER = "Templates/";

        private static MethodInfo _clearFunc;

        public static UnityObject LoadAssetByGuid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject));
        }

        public static Assembly[] GetAssemblies()
        {
            return Directory.GetFiles(@"Library\ScriptAssemblies\", "*.dll", SearchOption.AllDirectories)
                            .Select(file => Assembly.LoadFrom(file))
                            .ToArray();
        }

        public static Type[] GetTypes(Assembly[] assemblies, Func<Type, bool> selector)
        {
            List<Type> types = new List<Type>();

            for (int i = 0; i < assemblies.Length; i++)
            {
                types.AddRange(assemblies[i].GetTypes());
            }

            return types.Where(selector).ToArray();
        }

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

        public static void CreateScriptableObjectAsset(Type type, string assetPath = null)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(type);
            string name = assetPath.IsNullOrWhiteSpace() ? $"{ASSET_FOLDER}{type.Name}{ASSET_EXTENSION}"
                                                         : assetPath;
            AssetDatabase.CreateAsset(so, name);
            AssetDatabase.SaveAssets();
        }

        public static void CreateScriptableObjectAsset(Type type, UnityObject rootObject, string assetName = null)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(type);
            so.name = assetName.IsNullOrWhiteSpace() ? type.Name : assetName;
            AssetDatabase.AddObjectToAsset(so, rootObject);
            AssetDatabase.SaveAssets();
        }

        public static (string AssemblyName, string ClassName) SplitSerializedPropertyTypename(string typename)
        {
            if (typename.IsNullOrEmpty())
                return (null, null);

            string[] typeSplitString = typename.Split(' ');
            return (typeSplitString[0], typeSplitString[1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConvertToSystemTypename(in (string assemblyName, string className) splittedTypename)
        {
            return $"{splittedTypename.className}, {splittedTypename.assemblyName}";
        }

        public static Type GetTypeFromSerializedPropertyTypename(string typename)
        {
            (string assemblyName, string className) splittedTypename = SplitSerializedPropertyTypename(typename);
            return Type.GetType(ConvertToSystemTypename(splittedTypename));
        }

        public static Type GetFieldType(PropertyDrawer drawer)
        {
            Type drawnType = drawer.fieldInfo.FieldType;
            return drawnType.IsArray ? drawnType.GetElementType() : drawnType;
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
            DisplayMultiSelectableList(new Rect(position, Vector2.zero), flags, displayedOptions, onClose);
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
    }
}
