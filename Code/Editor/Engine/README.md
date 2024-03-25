## OlegHcpEditor.Engine

### UnityEditorExtensions

```csharp
public static class UnityEditorExtensions
{
    public static void ResetToDefault(this SerializedProperty self);
    public static bool HasManagedReferenceValue(this SerializedProperty self);
    public static IEnumerable<SerializedProperty> EnumerateProperties(this SerializedObject self, bool copyIterationState = true);
    public static IEnumerable<SerializedProperty> EnumerateInnerProperties(this SerializedProperty self, bool copyIterationState = true);
    public static IEnumerable<SerializedProperty> EnumerateArrayElements(this SerializedProperty self);
    public static int GetArrayElement(this SerializedProperty self, out SerializedProperty result, Predicate<SerializedProperty> condition);
    public static void SortArray<T>(this SerializedProperty self, Func<SerializedProperty, T> selector);
    public static SerializedProperty AddArrayElement(this SerializedProperty self);
    public static SerializedProperty PushArrayElementAtIndex(this SerializedProperty self, int index);
    public static IntMask GetIntMaskValue(this SerializedProperty self);
    public static void SetIntMaskValue(this SerializedProperty self, IntMask value);
    public static Diapason GetDiapasonValue(this SerializedProperty self);
    public static void SetDiapasonValue(this SerializedProperty self, Diapason value);
    public static DiapasonInt GetDiapasonIntValue(this SerializedProperty self);
    public static void SetDiapasonIntValue(this SerializedProperty self, DiapasonInt value);
    public static bool Disposed(this SerializedObject self);

    public static void Draw(this SerializedProperty self, params GUILayoutOption[] options);
    public static void Draw(this SerializedProperty self, GUIContent label, params GUILayoutOption[] options);
    public static void Draw(this SerializedProperty self, bool includeChildren, params GUILayoutOption[] options);
    public static void Draw(this SerializedProperty self, GUIContent label, bool includeChildren, params GUILayoutOption[] options);
    public static void Draw(this SerializedProperty self, in Rect position);
    public static void Draw(this SerializedProperty self, in Rect position, GUIContent label);
    public static void Draw(this SerializedProperty self, in Rect position, bool includeChildren);
    public static void Draw(this SerializedProperty self, in Rect position, GUIContent label, bool includeChildren);
    
    public static float GetHeight(this SerializedProperty self);
    public static float GetHeight(this SerializedProperty self, GUIContent label);
    public static float GetHeight(this SerializedProperty self, bool includeChildren);
    public static float GetHeight(this SerializedProperty self, GUIContent label, bool includeChildren);
    public static float GetHeight(this SerializedProperty self, Predicate<SerializedProperty> ignoreCondition);
}
```

### AssetExtensions

```csharp
public static class AssetExtensions
{
    public static bool IsFolder(this UnityObject self);
    public static bool IsNativeAsset(this UnityObject self);
    public static string GetAssetPath(this UnityObject self);
    public static string GetAssetGuid(this UnityObject self);
    public static Texture2D GetAssetIcon(this UnityObject self);
}
```

### UnityObjectExtensions

```csharp
public static class UnityObjectExtensions
{
    public static void DestroyImmediate(this UnityObject self)
    public static void DestroyChildrenImmediate(this Transform self)
    public static void DestroyChildrenImmediate(this Transform self, Predicate<Transform> predicate)
}
```
