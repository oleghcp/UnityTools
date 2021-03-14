using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(TextAsset))]
    internal class TextAssetEditor : Editor<TextAsset>
    {
        private string _path;
        private bool _isBinary;

        private string _text;
        private bool _tooLarge;

        private void OnEnable()
        {
            _path = AssetDatabase.GetAssetPath(target);
            _isBinary = Path.GetExtension(_path) == ".bytes";

            if (!_isBinary)
                LoadText();
        }

        public override void OnInspectorGUI()
        {
            if (_isBinary || _tooLarge)
            {
                base.OnInspectorGUI();
                return;
            }

            GUI.enabled = true;
            DrawTextField();
        }

        private void DrawTextField()
        {
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

        private void LoadText()
        {
            _text = target.text;
            _tooLarge = _text.Length > 16382;
        }
    }
}
