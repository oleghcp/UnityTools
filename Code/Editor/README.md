## OlegHcpEditor

### AssetDatabaseExt

```csharp
public static class AssetDatabaseExt
{
    public const string ASSET_FOLDER = "Assets/";
    public const string PROJECT_SETTINGS_FOLDER = "ProjectSettings/";
    public const string USER_SETTINGS_FOLDER = "UserSettings/";
    public const string LIBRARY_FOLDER = "Library/";
    public const string ASSET_EXTENSION = ".asset";
    public const string PREFAB_EXTENSION = ".prefab";

    public static string FullPathToProjectRelative(string fullPath);
    public static void CreateScriptableObjectAsset(Type type, string assetPath = null);
    public static void CreateScriptableObjectAsset(Type type, UnityObject rootObject, string assetName = null);
    public static T LoadAssetByGuid<T>(string guid) where T : UnityObject;
    public static UnityObject LoadAssetByGuid(string guid, Type type);
    public static Assembly[] LoadScriptAssemblies();
    public static IEnumerable<Assembly> EnumerateScriptAssemblies();
    public static void ConvertCodeFilesToUtf8();
    public static void ConvertTextFilesToUtf8();
    public static void ConvertToUtf8(params string[] extensions);
    public static IEnumerable<string> EnumerateAssetFiles();
    public static IEnumerable<string> EnumerateAssetFiles(string searchPattern);
    public static IEnumerable<string> EnumerateSettingsFiles();
    public static IEnumerable<string> EnumerateSettingsFiles(string searchPattern);
}
```

### EditorGui

```csharp
public static class EditorGui
{
    public static Diapason DiapasonField(in Rect position, string text, Diapason diapason, float minLimit = float.NegativeInfinity, float maxLimit = float.PositiveInfinity);
    public static Diapason DiapasonField(in Rect position, GUIContent label, Diapason diapason, float minLimit = float.NegativeInfinity, float maxLimit = float.PositiveInfinity);
    public static DiapasonInt DiapasonIntField(in Rect position, string text, DiapasonInt diapason, int minLimit = int.MinValue, int maxLimit = int.MaxValue);
    public static DiapasonInt DiapasonIntField(in Rect position, GUIContent label, DiapasonInt diapason, int minLimit = int.MinValue, int maxLimit = int.MaxValue);
    public static bool ToggleButton(in Rect position, string text, bool value);
    public static bool ToggleButton(in Rect position, string text, bool value, GUIStyle style);
    public static bool ToggleButton(in Rect position, GUIContent content, bool value);
    public static bool ToggleButton(in Rect position, GUIContent content, bool value, GUIStyle style);
    public static UnityObject[] DropArea(in Rect position);
    public static UnityObject[] DropArea(in Rect position, string text);
    public static UnityObject[] DropArea(in Rect position, GUIContent content);
    public static UnityObject[] DropArea(in Rect position, string text, GUIStyle style);
    public static UnityObject[] DropArea(in Rect position, GUIContent content, GUIStyle style);
    public static int DropDown(in Rect propertyRect, int selectedIndex, string[] displayedOptions);
    public static int DropDown(in Rect propertyRect, string label, int selectedIndex, string[] displayedOptions);
    public static int DropDown(Rect propertyRect, GUIContent label, int selectedIndex, string[] displayedOptions);
    public static int IntDropDown(in Rect propertyRect, int selectedValue, string[] displayedOptions, int[] optionValues);
    public static int IntDropDown(in Rect propertyRect, string label, int selectedValue, string[] displayedOptions, int[] optionValues);
    public static int IntDropDown(in Rect propertyRect, GUIContent label, int selectedValue, string[] displayedOptions, int[] optionValues);
    public static Enum EnumDropDown(in Rect propertyRect, Enum selected);
    public static Enum EnumDropDown(in Rect propertyRect, string label, Enum selected);
    public static Enum EnumDropDown(in Rect propertyRect, GUIContent label, Enum selected);
    public static int MaskDropDown(in Rect propertyRect, int mask, string[] displayedOptions);
    public static int MaskDropDown(in Rect propertyRect, string label, int mask, string[] displayedOptions);
    public static int MaskDropDown(Rect propertyRect, GUIContent label, int mask, string[] displayedOptions);
    public static Enum FlagsDropDown(in Rect propertyRect, Enum flags);
    public static Enum FlagsDropDown(in Rect propertyRect, string label, Enum flags);
    public static Enum FlagsDropDown(in Rect propertyRect, GUIContent label, Enum flags);
}
```

### EditorGuiLayout

```csharp
public static class EditorGuiLayout
{
    public static Diapason DiapasonField(string label, Diapason diapason, float minLimit, float maxLimit, params GUILayoutOption[] options);
    public static Diapason DiapasonField(GUIContent label, Diapason diapason, float minLimit, float maxLimit, params GUILayoutOption[] options);
    public static Diapason DiapasonField(string label, Diapason diapason, params GUILayoutOption[] options);
    public static Diapason DiapasonField(GUIContent label, Diapason diapason, params GUILayoutOption[] options);
    public static DiapasonInt DiapasonIntField(string label, DiapasonInt diapason, int minLimit, int maxLimit, params GUILayoutOption[] options);
    public static DiapasonInt DiapasonIntField(GUIContent label, DiapasonInt diapason, int minLimit, int maxLimit, params GUILayoutOption[] options);
    public static DiapasonInt DiapasonIntField(string label, DiapasonInt diapason, params GUILayoutOption[] options);
    public static DiapasonInt DiapasonIntField(GUIContent label, DiapasonInt diapason, params GUILayoutOption[] options);
    public static bool ToggleButton(string text, bool value, params GUILayoutOption[] options);
    public static bool ToggleButton(string text, bool value, GUIStyle style, params GUILayoutOption[] options);
    public static bool ToggleButton(GUIContent content, bool value, params GUILayoutOption[] options);
    public static bool ToggleButton(GUIContent content, bool value, GUIStyle style, params GUILayoutOption[] options);
    public static UnityObject[] DropArea(params GUILayoutOption[] options);
    public static UnityObject[] DropArea(string text, params GUILayoutOption[] options);
    public static UnityObject[] DropArea(GUIContent content, params GUILayoutOption[] options);
    public static UnityObject[] DropArea(string text, GUIStyle style, params GUILayoutOption[] options);
    public static UnityObject[] DropArea(GUIContent content, GUIStyle style, params GUILayoutOption[] options);
    public static int DropDown(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options);
    public static int DropDown(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options);
    public static int DropDown(GUIContent label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options);
    public static int IntDropDown(int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options);
    public static int IntDropDown(string label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options);
    public static int IntDropDown(GUIContent label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options);
    public static Enum EnumDropDown(Enum selected, params GUILayoutOption[] options);
    public static Enum EnumDropDown(string label, Enum selected, params GUILayoutOption[] options);
    public static Enum EnumDropDown(GUIContent label, Enum selected, params GUILayoutOption[] options);
    public static int MaskDropDown(int mask, string[] displayedOptions, params GUILayoutOption[] options);
    public static int MaskDropDown(string label, int mask, string[] displayedOptions, params GUILayoutOption[] options);
    public static int MaskDropDown(GUIContent label, int mask, string[] displayedOptions, params GUILayoutOption[] options);
    public static Enum FlagsDropDown(Enum flags, params GUILayoutOption[] options);
    public static Enum FlagsDropDown(string label, Enum flags, params GUILayoutOption[] options);
    public static Enum FlagsDropDown(GUIContent label, Enum flags, params GUILayoutOption[] options);
    public static Rect BeginHorizontalCentering(params GUILayoutOption[] options);
    public static Rect BeginHorizontalCentering(GUIStyle style, params GUILayoutOption[] options);
    public static void EndHorizontalCentering();
    public static Rect BeginVerticalCentering(params GUILayoutOption[] options);
    public static Rect BeginVerticalCentering(GUIStyle style, params GUILayoutOption[] options);
    public static void EndVerticalCentering();

    public class HorizontalCenteringScope : GUI.Scope
    {
        public Rect Rect { get; }

        public HorizontalCenteringScope(params GUILayoutOption[] options);
        public HorizontalCenteringScope(GUIStyle style, params GUILayoutOption[] options);

        protected override void CloseScope();
    }

    public class VerticalCenteringScope : GUI.Scope
    {
        public Rect Rect { get; }

        public VerticalCenteringScope(params GUILayoutOption[] options);
        public VerticalCenteringScope(GUIStyle style, params GUILayoutOption[] options);

        protected override void CloseScope();
    }
}
```

### EditorGuiUtility

```csharp
public static class EditorGuiUtility
{
    public static string BuiltInSkinsPath { get; }
    public static float SmallButtonWidth { get; }
    public static float StandardHorizontalSpacing { get; }
    public static float IndentLevelOffset { get; }

    public static GUIContent TempContent(string label, string tooltip = null);
    public static GUIContent TempContent(Texture2D image, string tooltip = null);
    public static Rect GetLinePosition(in Rect basePosition, int lineIndex);
    public static Rect GetLinePosition(in Rect basePosition, int lineIndex, float lineHeight);
    public static Rect GetLinePosition(in Rect basePosition, int line, int column, int columnCount);
    public static float GetDrawHeight(SerializedObject serializedObject, Predicate<SerializedProperty> ignoreCondition = null);
    public static float GetDrawHeight(SerializedProperty property, Predicate<SerializedProperty> ignoreCondition = null);
    public static string GetTypeDisplayName(Type type);
}
```

### EditorUtilityExt

```csharp
public static class EditorUtilityExt
{
    public const string SCRIPT_FIELD = "m_Script";
    public const string ASSET_NAME_FIELD = "m_Name";

    public static void OpenCsProject();
    public static void ClearConsoleWindow();
    public static string ConvertToSystemTypename(string managedReferenceFieldTypename);
    public static Type GetTypeFromSerializedPropertyTypename(string managedReferenceTypename);
    public static Type GetFieldType(PropertyDrawer drawer);
    public static void DisplayDropDownList(Vector2 position, string[] displayedOptions, Predicate<int> checkEnabled, Action<int> onItemSelected);
    public static void DisplayDropDownList(in Rect buttonRect, string[] displayedOptions, Predicate<int> checkEnabled, Action<int> onItemSelected);
    public static void DisplayMultiSelectableList(Vector2 position, BitList flags, string[] displayedOptions, Action<BitList> onClose = null);
    public static void DisplayMultiSelectableList(in Rect buttonRect, BitList flags, string[] displayedOptions, Action<BitList> onClose = null);
    public static bool GetDroppedObjects(in Rect position, out UnityObject[] result);
    public static void OpenScriptableObjectCode(ScriptableObject scriptableObject);
    public static void OpenFolder(string path);
    public static void ExecuteWithProgressBar(string title, string info, IEnumerator<float> iterator, Action onSuccess = null);
    public static void ExecuteWithProgressBarCancelable(string title, string info, IEnumerator<float> iterator, Action onSuccess = null, Action onCansel = null);
    public static void ExecuteWithProgressBar(string title, IEnumerator<(string info, float progress)> iterator, Action onSuccess = null);
    public static void ExecuteWithProgressBarCancelable(string title, IEnumerator<(string info, float progress)> iterator, Action onSuccess = null, Action onCansel = null);
}
```

### EditorStylesExt

```csharp
public static class EditorStylesExt
{
    public static GUIStyle DropArea { get; }
    public static GUIStyle DropDown { get; }
    public static GUIStyle Rect { get; }
}
```

### DropDownMenu

```csharp
public class DropDownMenu
{
    public DropDownMenu();

    public void ShowMenu();
    public void ShowMenu(Vector2 position);
    public void ShowMenu(in Rect buttonRect);
    public void AddItem(string content, Action onSelected);
    public void AddItem(string content, bool on, Action onSelected);
    public void AddDisabledItem(string content);
    public void AddDisabledItem(string content, bool on);
    public void AddSeparator();
}
```

### Editor

```csharp
public abstract class Editor<T> : Editor, IReadOnlyList<T> where T : UnityObject
{
    public new T target { get; set; }
    public new IReadOnlyList<T> targets { get; }
}
```

### AttributeDrawer

```csharp
public abstract class AttributeDrawer<TAttribute> : PropertyDrawer where TAttribute : PropertyAttribute
{
    public new TAttribute attribute { get; }
}
```
