using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(ScriptableObject), true)]
    public class ScriptableObjectEditor : Editor
    {
        protected override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                base.OnHeaderGUI();
                EditorGUILayout.BeginVertical();
                {
                    if (GUILayout.Button("Edit\nScript", GUILayout.MinHeight(50f)))
                    {
                        var prop = serializedObject.FindProperty("m_Script");
                        var filePath = AssetDatabase.GetAssetPath(prop.objectReferenceValue);
                        System.Diagnostics.Process.Start("devenv", "/edit " + filePath);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        protected override bool ShouldHideOpenButton()
        {
            return true;
        }
    }
}
