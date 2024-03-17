using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window
{
    internal class AddressablesAnalysisInfo : EditorWindow
    {
        private void OnEnable()
        {
            minSize = maxSize = new Vector2(250f, 100f);
        }

        private void OnGUI()
        {
            Vector2 offset = Vector2.one * 3f;
            GUILayout.BeginArea(new Rect(offset, position.size - offset * 2f), EditorStyles.helpBox);

            EditorGUILayout.Space();

            GUILayout.Label("Packages required:");
            GUILayout.Label(" - com.unity.addressables");
            GUILayout.Label(" - com.unity.nuget.newtonsoft-json");

            GUILayout.EndArea();
        }
    }
}
