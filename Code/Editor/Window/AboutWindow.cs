using System;
using System.IO;
using System.Reflection;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window
{
    internal class AboutWindow : EditorWindow
    {
        private string _copyright;
        private string _version;
        private string _description1;
        private string _description2;

        private void OnEnable()
        {
            minSize = maxSize = new Vector2(350f, 150f);

            Assembly assembly = Assembly.Load(LibConstants.LIB_NAME) ?? Assembly.GetExecutingAssembly();

            _description1 = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            _copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;

            string path = AssetDatabase.GUIDToAssetPath(LibConstants.PACKAGE_INFO_GUID);
            string json = File.ReadAllText(path);
            VersionInfo package = JsonUtility.FromJson<VersionInfo>(json);
            _version = package.version;
            _description2 = $"Supports Unity {package.unity} or higher";
        }

        private void OnGUI()
        {
            Vector2 offset = Vector2.one * 3f;
            GUILayout.BeginArea(new Rect(offset, position.size - offset * 2f), EditorStyles.helpBox);

            using (new EditorGuiLayout.HorizontalCenteringScope())
            {
                EditorGUILayout.BeginVertical();

                GUILayout.Space(20f);

                GUILayout.Label(_description1);
                GUILayout.Label(_description2);
                GUILayout.Label($"Version {_version}");

                EditorGUILayout.Space();

                GUILayout.Label(_copyright);

                EditorGUILayout.EndVertical();
            }

            GUILayout.EndArea();
        }

        [Serializable]
        private struct VersionInfo
        {
#pragma warning disable IDE1006
            public string version;
            public string unity;
#pragma warning restore
        }
    }
}
