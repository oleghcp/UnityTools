using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityUtilityEditor
{
    [CustomEditor(typeof(TextAsset))]
    internal class TextAssetEditor : Editor
    {
        private string m_path;
        private bool m_isBinary;

        private string m_text;
        private bool m_cut;

        private void Awake()
        {
            m_path = AssetDatabase.GetAssetPath(target);

            m_isBinary = Path.GetExtension(m_path) == ".bytes";

            //Font.CreateDynamicFontFromOSFont();

            if (!m_isBinary)
                f_loadText();
        }

        public override void OnInspectorGUI()
        {
            if (m_isBinary)
                f_drawBinaryMode();
            else
                f_drawTextMode();
        }

        private void f_drawBinaryMode()
        {
            GUI.enabled = true;
            EditorGUILayout.HelpBox("The TextAsset is marked as binary.", MessageType.Info);
        }

        private void f_drawTextMode()
        {
            if (m_cut)
            {
                GUI.enabled = true;
                EditorGUILayout.HelpBox("The text is too large for editing.", MessageType.Info);
                GUI.enabled = false;
                EditorGUILayout.TextArea(m_text);
            }
            else
            {
                GUILayout.Space(5f);

                GUI.enabled = true;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Apply"))
                {
                    File.WriteAllText(m_path, m_text);
                    AssetDatabase.Refresh();
                }

                GUILayout.Space(10f);

                if (GUILayout.Button("Discard"))
                {
                    GUIUtility.keyboardControl = 0;
                    f_loadText();
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);

                m_text = EditorGUILayout.TextArea(m_text);
            }
        }

        private void f_loadText()
        {
            m_text = (target as TextAsset).text;

            int maxLen = 16382;

            if (m_text.Length > maxLen)
            {
                char[] chars = m_text.ToCharArray(0, maxLen);
                m_text = new string(chars);
                m_cut = true;
            }
        }
    }
}
