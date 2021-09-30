using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtility.Controls.ControlStuff;
using UnityObject = UnityEngine.Object;

#if UNITY_2018_3_OR_NEWER
namespace Project
{
    public class GamepadAxesSettingsProvider : SettingsProvider
    {
        private SerializedObject _playerSettings;
        private SerializedObject _inputSettings;

        private SerializedProperty _axesArray;
        private SerializedProperty _activeInputHandler;

        private List<string> _names = new List<string>();

        private Vector2 _scrollPos;

        private int _pads = 2;
        private int _axes = 15;

        public GamepadAxesSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            string assetPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}ProjectSettings{AssetDatabaseExt.ASSET_EXTENSION}";
            PlayerSettings playerSettings = AssetDatabase.LoadAssetAtPath<PlayerSettings>(assetPath);
            _playerSettings = new SerializedObject(playerSettings);
            _activeInputHandler = _playerSettings.FindProperty("activeInputHandler");

            assetPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}InputManager{AssetDatabaseExt.ASSET_EXTENSION}";
            UnityObject inputSettings = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
            _inputSettings = new SerializedObject(inputSettings);
            _axesArray = _inputSettings.FindProperty("m_Axes");
            RefreshAxes();
        }

        [SettingsProvider]
        private static SettingsProvider CreateMyCustomSettingsProvider()
        {
            return new GamepadAxesSettingsProvider($"{nameof(UnityUtility)}/Gamepad Axes",
                                                   SettingsScope.Project,
                                                   new[] { "Gamepad", "Axes" });
        }

        public override void OnGUI(string searchContext)
        {
            if (_activeInputHandler.intValue == 1)
            {
                EditorGUILayout.HelpBox("Active input handling doesn't include Input Manager (Old).", MessageType.Info);
                return;
            }

            _inputSettings.Update();

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Input Manager", GUILayout.MaxWidth(150f), GUILayout.Height(25f)))
                Selection.activeObject = _inputSettings.targetObject;

            GUILayout.Space(5f);

            _scrollPos.y = GUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;
            for (int i = 0; i < _names.Count; i++)
            {
                EditorGUILayout.LabelField(_names[i], GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            }
            GUILayout.EndScrollView();

            GUILayout.Space(5f);

            EditorGUILayout.LabelField("Add gamepad axes:", EditorStyles.boldLabel);
            GUILayout.Space(5f);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gamepads amount:", GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            _pads = EditorGUILayout.IntField(_pads, GUILayout.Width(30f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Axes amount:", GUILayout.MaxWidth(150f), GUILayout.MinWidth(10f));
            _axes = EditorGUILayout.IntField(_axes, GUILayout.Width(30f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5f);

            if (GUILayout.Button("Add axes", GUILayout.Width(190f), GUILayout.Height(25f)))
            {
                StringBuilder builder = new StringBuilder();

                for (int pad = 0; pad < _pads; pad++)
                {
                    for (int axis = 0; axis < _axes; axis++)
                    {
                        AddAxis(axis + 1, pad + 1, builder);
                    }
                }
                RefreshAxes();
            }

            EditorGUILayout.Space(5f);

            if (GUILayout.Button("Remove axes", GUILayout.Height(25f), GUILayout.Width(190f)))
            {
                int i = 0;

                while (i < _axesArray.arraySize)
                {
                    SerializedProperty axis = _axesArray.GetArrayElementAtIndex(i);

                    if (axis.FindPropertyRelative("type").enumValueIndex != 2)
                    {
                        i++;
                        continue;
                    }

                    string name = axis.FindPropertyRelative("m_Name").stringValue;
                    int num = axis.FindPropertyRelative("joyNum").enumValueIndex;
                    bool propVal = name[0] - (char)num == '@' && name[1] == ':' && int.TryParse(name[2].ToString(), out _);

                    if (!propVal) { i++; }
                    else { _axesArray.DeleteArrayElementAtIndex(i); }
                }

                RefreshAxes();
            }

            GUILayout.Space(10f);

            _inputSettings.ApplyModifiedProperties();
        }

        private void RefreshAxes()
        {
            _names.Clear();

            foreach (var axis in _axesArray.EnumerateArrayElements())
            {
                _names.Add(axis.FindPropertyRelative("m_Name").stringValue);
            }
        }

        private void AddAxis(int axNum, int padNum, StringBuilder builder)
        {
            SerializedProperty axis = _axesArray.AddArrayElement();

            axis.FindPropertyRelative("m_Name").stringValue = InputUnility.AxisName(axNum, padNum, builder);
            axis.FindPropertyRelative("dead").floatValue = 0.1f;
            axis.FindPropertyRelative("sensitivity").floatValue = 1f;
            axis.FindPropertyRelative("type").enumValueIndex = 2;
            axis.FindPropertyRelative("axis").enumValueIndex = axNum - 1;
            axis.FindPropertyRelative("joyNum").enumValueIndex = padNum;
        }
    }
}
#endif
