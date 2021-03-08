using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(TextAsset))]
    internal class TextAssetEditor : Editor<TextAsset>
    {
        private string _path;
        private bool _isBinary;

        private string _text;
        private bool _cut;

        private void OnEnable()
        {
            _path = AssetDatabase.GetAssetPath(target);

            _isBinary = Path.GetExtension(_path) == ".bytes";

            if (!_isBinary)
                LoadText();
        }

        public override void OnInspectorGUI()
        {
            if (_isBinary)
                DrawBinaryMode();
            else
                DrawTextMode();
        }

        private void DrawBinaryMode()
        {
            GUI.enabled = true;
            EditorGUILayout.HelpBox("The TextAsset is marked as binary.", MessageType.Info);
        }

        private void DrawTextMode()
        {
            if (_cut)
            {
                GUI.enabled = true;
                EditorGUILayout.HelpBox("The text is too large for editing.", MessageType.Info);
                GUI.enabled = false;
                EditorGUILayout.TextArea(_text);
            }
            else
            {
                GUI.enabled = true;

                _text = EditorGUILayout.TextArea(_text);

                GUILayout.Space(5f);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Apply"))
                    {
                        File.WriteAllText(_path, _text);
                        AssetDatabase.Refresh();
                    }

                    GUILayout.Space(10f);

                    if (GUILayout.Button("Discard"))
                    {
                        GUIUtility.keyboardControl = 0;
                        LoadText();
                    }

                    EditorGUILayout.Space();
                }
            }
        }

        private void LoadText()
        {
            _text = target.text;

            int maxLen = 16382;

            if (_text.Length > maxLen)
            {
                char[] chars = _text.ToCharArray(0, maxLen);
                _text = new string(chars);
                _cut = true;
            }
        }
    }
}
