#if LIBRARY_EDIT
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Window
{
    public class LibraryEditorWindow : EditorWindow
    {
        private const string PACKAGE_INFO_GUID = "d14a26acf7afb104d86cc3c1d49007e1";
        private const string RUNTIME_INFO_GUID = "2a65d3c05ddab8b43ba7a7214de640db";
        private const string EDITOR_INFO_GUID = "d1131f8aadf09f145bd3a7155560b417";

        private string _packageVersion;
        private string _runtimeVersion;
        private string _editorVersion;

        private void OnFocus()
        {
            _packageVersion = GetVersion(AssetDatabase.GUIDToAssetPath(PACKAGE_INFO_GUID), FileType.PackageInfo);
            _runtimeVersion = GetVersion(AssetDatabase.GUIDToAssetPath(RUNTIME_INFO_GUID), FileType.AssemblyInfo);
            _editorVersion = GetVersion(AssetDatabase.GUIDToAssetPath(EDITOR_INFO_GUID), FileType.AssemblyInfo);
        }

        private void OnGUI()
        {
            _packageVersion = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_packageVersion)), _packageVersion);
            _runtimeVersion = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_runtimeVersion)), _runtimeVersion);
            _editorVersion = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(_editorVersion)), _editorVersion);

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply"))
            {
                SetVersion(AssetDatabase.GUIDToAssetPath(PACKAGE_INFO_GUID), FileType.PackageInfo, _packageVersion);
                SetVersion(AssetDatabase.GUIDToAssetPath(RUNTIME_INFO_GUID), FileType.AssemblyInfo, _runtimeVersion);
                SetVersion(AssetDatabase.GUIDToAssetPath(EDITOR_INFO_GUID), FileType.AssemblyInfo, _editorVersion);
                AssetDatabase.Refresh();
            }
        }

        private string GetVersion(string filePath, FileType fileType)
        {
            string searchText = GetSearchText(fileType);

            foreach (string line in File.ReadLines(filePath))
            {
                int index = line.IndexOf(searchText);

                if (index >= 0)
                    return GetSubstring(line, index + searchText.Length);
            }

            return string.Empty;
        }

        private void SetVersion(string filePath, FileType fileType, string newVersion)
        {
            string searchText = GetSearchText(fileType);

            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                int index = lines[i].IndexOf(searchText);

                if (index >= 0)
                {
                    string version = GetSubstring(lines[i], index + searchText.Length);
                    lines[i] = lines[i].Replace(version, newVersion);
                    break;
                }
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        private static string GetSubstring(string text, int index)
        {
            const char separator = '\"';
            string substring = text.Substring(index);
            int from = substring.IndexOf(separator) + 1;
            int to = substring.LastIndexOf(separator);
            return substring.Substring(from, to - from);
        }

        private string GetSearchText(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.PackageInfo: return "\"version\":";
                case FileType.AssemblyInfo: return "AssemblyVersion";
                default: throw new UnsupportedValueException(fileType);
            }
        }

        private enum FileType
        {
            PackageInfo,
            AssemblyInfo,
        }
    }
}
#endif
