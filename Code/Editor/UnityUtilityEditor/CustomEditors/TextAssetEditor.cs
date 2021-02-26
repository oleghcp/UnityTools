﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(TextAsset))]
    internal class TextAssetEditor : Editor
    {
        private string m_path;
        private bool m_isBinary;

        private string m_text;
        private bool m_cut;

        private void OnEnable()
        {
            m_path = AssetDatabase.GetAssetPath(target);

            m_isBinary = Path.GetExtension(m_path) == ".bytes";

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
                GUI.enabled = true;

                m_text = EditorGUILayout.TextArea(m_text);

                GUILayout.Space(5f);

                using (new EditorGUILayout.HorizontalScope())
                {
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
                }
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
