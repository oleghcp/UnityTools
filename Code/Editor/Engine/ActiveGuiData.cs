using System;
using System.Collections.Generic;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Engine
{
    internal class DragAndDropData
    {
        private static DragAndDropData _instance;
        private int _id;
        private UnityObject[] _droppedObjects;

        public static UnityObject[] GetObjectsById(int controlId)
        {
            UnityObject[] objects = null;

            if (_instance != null)
            {
                if (_instance._id == controlId)
                {
                    GUI.changed = true;

                    objects = _instance._droppedObjects;
                    _instance = null;
                }
            }

            return objects;
        }

        public static void PlaceObjects(int controlId, UnityObject[] objects)
        {
            if (_instance != null)
                _instance._id = controlId;
            else
                _instance = new DragAndDropData { _id = controlId };

            _instance._droppedObjects = objects;
        }
    }

    internal class DropDownData
    {
        private static DropDownData _instance;
        private int _id;
        private int _selectedValue;

        public static int GetSelectedValueById(int selectedValue, int controlId)
        {
            if (_instance != null)
            {
                if (_instance._id == controlId)
                {
                    GUI.changed = true;

                    selectedValue = _instance._selectedValue;
                    _instance = null;
                }
            }

            return selectedValue;
        }

        public static void MenuAction(int controlId, int value)
        {
            if (_instance != null)
                _instance._id = controlId;
            else
                _instance = new DropDownData { _id = controlId };

            _instance._selectedValue = value;
        }
    }

    internal static class EnumDropDownData
    {
        private static Dictionary<Type, Data> _data;

        public static Data GetData(Type enumType)
        {
            if (_data == null)
                _data = new Dictionary<Type, Data>();

            if (_data.TryGetValue(enumType, out Data data))
                return data;

            return _data.Place(enumType, new Data(enumType));
        }

        public class Data
        {
            private Type _enumType;
            private Array _enumValues;
            private string[] _enumNames;
            private string[] _indexableEnumNames;

            public Array EnumValues => _enumValues ?? (_enumValues = Enum.GetValues(_enumType));
            public string[] EnumNames => _enumNames ?? (_enumNames = Enum.GetNames(_enumType));

            public string[] IndexableEnumNames
            {
                get
                {
                    if (_indexableEnumNames == null)
                    {
                        if (_enumType.IsDefined(typeof(FlagsAttribute), false) || EnumValues.Length == 0)
                            return Array.Empty<string>();

                        Array enumValues = EnumValues;
                        object lastElement = enumValues.GetValue(enumValues.Length - 1);
                        _indexableEnumNames = new string[Convert.ToInt32(lastElement) + 1];

                        foreach (Enum item in enumValues)
                        {
                            _indexableEnumNames[Convert.ToInt32(item)] = item.GetName();
                        }
                    }

                    return _indexableEnumNames;
                }
            }

            public Data(Type enumType)
            {
                _enumType = enumType;

                _enumValues = Enum.GetValues(enumType);
                _enumNames = Enum.GetNames(enumType);
            }
        }
    }
}
