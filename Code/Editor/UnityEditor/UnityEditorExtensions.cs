using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public static class UnityEditorExtensions
    {
        public static void ResetToDefault(this SerializedProperty self)
        {
            #region Switch/Case
            switch (self.propertyType)
            {
                case SerializedPropertyType.FixedBufferSize:
                    //SerializedProperty.fixedBufferSize is read only
                    break;

                case SerializedPropertyType.Generic:
                    if (self.isArray)
                        self.ClearArray();
                    else
                        self.EnumerateInnerProperties()
                            .ForEach(ResetToDefault);
                    break;

                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Gradient:
                    self.EnumerateInnerProperties()
                        .ForEach(ResetToDefault);
                    break;

                case SerializedPropertyType.ObjectReference:
                    self.objectReferenceValue = null;
                    break;

                case SerializedPropertyType.ExposedReference:
                    self.exposedReferenceValue = null;
                    break;

#if UNITY_2019_3_OR_NEWER
                case SerializedPropertyType.ManagedReference:
                    self.managedReferenceValue = null;
                    break;
#endif
                case SerializedPropertyType.String:
                case SerializedPropertyType.Character:
                    self.stringValue = string.Empty;
                    break;

                case SerializedPropertyType.Integer:
                    self.intValue = default;
                    break;

                case SerializedPropertyType.Boolean:
                    self.boolValue = default;
                    break;

                case SerializedPropertyType.Float:
                    self.floatValue = default;
                    break;

                case SerializedPropertyType.Color:
                    self.colorValue = default;
                    break;

                case SerializedPropertyType.Enum:
                    self.enumValueIndex = default;
                    break;

                case SerializedPropertyType.Vector2:
                    self.vector2Value = default;
                    break;

                case SerializedPropertyType.Vector3:
                    self.vector3Value = default;
                    break;

                case SerializedPropertyType.Vector4:
                    self.vector4Value = default;
                    break;

                case SerializedPropertyType.Rect:
                    self.rectValue = default;
                    break;

                case SerializedPropertyType.ArraySize:
                    self.arraySize = default;
                    break;

                case SerializedPropertyType.AnimationCurve:
                    self.animationCurveValue = new AnimationCurve();
                    break;

                case SerializedPropertyType.Bounds:
                    self.boundsValue = default;
                    break;

                case SerializedPropertyType.Quaternion:
                    self.quaternionValue = Quaternion.identity;
                    break;

                case SerializedPropertyType.Vector2Int:
                    self.vector2IntValue = default;
                    break;

                case SerializedPropertyType.Vector3Int:
                    self.vector3IntValue = default;
                    break;

                case SerializedPropertyType.RectInt:
                    self.rectIntValue = default;
                    break;

                case SerializedPropertyType.BoundsInt:
                    self.boundsIntValue = default;
                    break;

                default:
                    throw new UnsupportedValueException(self.propertyType);
            }
            #endregion
        }

        public static IEnumerable<SerializedProperty> EnumerateProperties(this SerializedObject self)
        {
            SerializedProperty iterator = self.GetIterator();

            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                yield return iterator;
            }
        }

        public static IEnumerable<SerializedProperty> EnumerateInnerProperties(this SerializedProperty self)
        {
            SerializedProperty iterator = self.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            if (moveNext(true))
            {
                do { yield return iterator; } while (moveNext(false));
            }

            bool moveNext(bool enterChildren)
            {
                return iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end);
            }
        }

        public static IEnumerable<SerializedProperty> EnumerateArrayElements(this SerializedProperty self)
        {
            int len = self.arraySize;

            for (int i = 0; i < len; i++)
            {
                yield return self.GetArrayElementAtIndex(i);
            }
        }

        public static int GetArrayElement(this SerializedProperty self, out SerializedProperty result, Predicate<SerializedProperty> condition)
        {
            int count = self.arraySize;

            for (int i = 0; i < count; i++)
            {
                SerializedProperty element = self.GetArrayElementAtIndex(i);

                if (condition(element))
                {
                    result = element;
                    return i;
                }

                element.Dispose();
            }

            result = null;
            return -1;
        }

        public static void SortArray<T>(this SerializedProperty self, Func<SerializedProperty, T> selector)
        {
            Comparer<T> defComp = Comparer<T>.Default;

            int length = self.arraySize - 1;
            int len = length;

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    T a = selector(self.GetArrayElementAtIndex(j));
                    T b = selector(self.GetArrayElementAtIndex(j + 1));

                    if (defComp.Compare(a, b) > 0)
                    {
                        self.MoveArrayElement(j, j + 1);
                    }
                }
                len--;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddArrayElement(this SerializedProperty self)
        {
            self.InsertArrayElementAtIndex(self.arraySize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializedProperty PlaceArrayElement(this SerializedProperty self)
        {
            return self.PushArrayElementAtIndex(self.arraySize);
        }

        public static SerializedProperty PushArrayElementAtIndex(this SerializedProperty self, int index)
        {
            self.InsertArrayElementAtIndex(index);
            return self.GetArrayElementAtIndex(index);
        }

        public static void SetBytesValue(this SerializedProperty self, Bytes value)
        {
            using (SerializedProperty inner = self.FindPropertyRelative(Bytes.FieldName))
            {
                inner.intValue = (int)value;
            }
        }

        public static Bytes GetBytesValue(this SerializedProperty self)
        {
            using (SerializedProperty inner = self.FindPropertyRelative(Bytes.FieldName))
            {
                return inner.intValue;
            }
        }

        public static bool Disposed(this SerializedObject self)
        {
            try { bool check = self.isEditingMultipleObjects; }
            catch (Exception) { return true; }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFolder(this UnityObject self)
        {
            return ProjectWindowUtil.IsFolder(self.GetInstanceID());
        }
    }
}
