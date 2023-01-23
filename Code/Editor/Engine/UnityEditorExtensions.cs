using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.CSharp;
using UnityUtility.CSharp.Collections;
using UnityUtility.NumericEntities;

namespace UnityUtilityEditor.Engine
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
#if UNITY_2021_1_OR_NEWER
                case SerializedPropertyType.Hash128:
                    self.hash128Value = default;
                    break;
#endif
                default:
                    throw new UnsupportedValueException(self.propertyType);
            }
            #endregion
        }

#if UNITY_2019_3_OR_NEWER
        public static bool HasManagedReferenceValue(this SerializedProperty self)
        {
#if UNITY_2021_2_OR_NEWER
            return self.managedReferenceValue != null;
#else
            return self.managedReferenceFullTypename.HasAnyData();
#endif
        }
#endif

        public static IEnumerable<SerializedProperty> EnumerateProperties(this SerializedObject self, bool copyIterationState = true)
        {
            SerializedProperty iterator = self.GetIterator();

            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                yield return copyIterationState ? iterator.Copy()
                                                : iterator;
            }
        }

        public static IEnumerable<SerializedProperty> EnumerateInnerProperties(this SerializedProperty self, bool copyIterationState = true)
        {
            SerializedProperty iterator = self.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            if (moveNext(true))
            {
                do
                {
                    yield return copyIterationState ? iterator.Copy()
                                                    : iterator;
                } while (moveNext(false));
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
                result = self.GetArrayElementAtIndex(i);

                if (condition(result))
                    return i;

                result.Dispose();
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
                        self.MoveArrayElement(j, j + 1);
                }
                len--;
            }
        }

        public static SerializedProperty AddArrayElement(this SerializedProperty self)
        {
            int index = self.arraySize++;
            return self.GetArrayElementAtIndex(index);
        }

        public static SerializedProperty PushArrayElementAtIndex(this SerializedProperty self, int index)
        {
            self.InsertArrayElementAtIndex(index);
            return self.GetArrayElementAtIndex(index);
        }

        public static IntMask GetIntMaskValue(this SerializedProperty self)
        {
            using (SerializedProperty inner = self.FindPropertyRelative(IntMask.FieldName))
            {
                return inner.intValue;
            }
        }

        public static void SetIntMaskValue(this SerializedProperty self, IntMask value)
        {
            using (SerializedProperty inner = self.FindPropertyRelative(IntMask.FieldName))
            {
                inner.intValue = (int)value;
            }
        }

        public static Diapason GetDiapasonValue(this SerializedProperty self)
        {
            return new Diapason(getValue(Diapason.MinFieldName), getValue(Diapason.MaxFieldName));

            float getValue(string fieldName)
            {
                using (SerializedProperty inner = self.FindPropertyRelative(fieldName))
                {
                    return inner.floatValue;
                }
            }
        }

        public static void SetDiapasonValue(this SerializedProperty self, Diapason value)
        {
            setValue(Diapason.MinFieldName, value.Min);
            setValue(Diapason.MaxFieldName, value.Max);

            void setValue(string fieldName, float fieldValue)
            {
                using (SerializedProperty inner = self.FindPropertyRelative(fieldName))
                {
                    inner.floatValue = fieldValue;
                }
            }
        }

        public static DiapasonInt GetDiapasonIntValue(this SerializedProperty self)
        {
            return new DiapasonInt(getValue(DiapasonInt.MinFieldName), getValue(DiapasonInt.MaxFieldName));

            int getValue(string fieldName)
            {
                using (SerializedProperty inner = self.FindPropertyRelative(fieldName))
                {
                    return inner.intValue;
                }
            }
        }

        public static void SetDiapasonIntValue(this SerializedProperty self, DiapasonInt value)
        {
            setValue(DiapasonInt.MinFieldName, value.Min);
            setValue(DiapasonInt.MaxFieldName, value.Max);

            void setValue(string fieldName, int fieldValue)
            {
                using (SerializedProperty inner = self.FindPropertyRelative(fieldName))
                {
                    inner.intValue = fieldValue;
                }
            }
        }

        public static bool Disposed(this SerializedObject self)
        {
            try { bool check = self.isEditingMultipleObjects; }
            catch (ArgumentNullException) { return true; }

            return false;
        }

        #region
        public static void Draw(this SerializedProperty self, params GUILayoutOption[] options)
        {
            EditorGUILayout.PropertyField(self, options);
        }

        public static void Draw(this SerializedProperty self, GUIContent label, params GUILayoutOption[] options)
        {
            EditorGUILayout.PropertyField(self, label, options);
        }

        public static void Draw(this SerializedProperty self, bool includeChildren, params GUILayoutOption[] options)
        {
            EditorGUILayout.PropertyField(self, includeChildren, options);
        }

        public static void Draw(this SerializedProperty self, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
        {
            EditorGUILayout.PropertyField(self, label, includeChildren, options);
        }

        public static void Draw(this SerializedProperty self, in Rect position)
        {
            EditorGUI.PropertyField(position, self);
        }

        public static void Draw(this SerializedProperty self, in Rect position, GUIContent label)
        {
            EditorGUI.PropertyField(position, self, label);
        }

        public static void Draw(this SerializedProperty self, in Rect position, bool includeChildren)
        {
            EditorGUI.PropertyField(position, self, includeChildren);
        }

        public static void Draw(this SerializedProperty self, in Rect position, GUIContent label, bool includeChildren)
        {
            EditorGUI.PropertyField(position, self, label, includeChildren);
        }

        public static float GetHeight(this SerializedProperty self)
        {
            return EditorGUI.GetPropertyHeight(self);
        }

        public static float GetHeight(this SerializedProperty self, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(self, label);
        }

        public static float GetHeight(this SerializedProperty self, bool includeChildren)
        {
            return EditorGUI.GetPropertyHeight(self, includeChildren);
        }

        public static float GetHeight(this SerializedProperty self, GUIContent label, bool includeChildren)
        {
            return EditorGUI.GetPropertyHeight(self, label, includeChildren);
        }

        public static float GetHeight(this SerializedProperty self, Predicate<SerializedProperty> ignoreCondition)
        {
            return EditorGuiUtility.GetDrawHeight(self, ignoreCondition);
        }
        #endregion
    }
}
