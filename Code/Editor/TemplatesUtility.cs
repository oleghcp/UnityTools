using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityUtility.CSharp;
using UnityUtility.IO;
using UnityUtilityEditor.Engine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor
{
    internal static class TemplatesUtility
    {
        private const string TEMPLATES_FOLDER = LibConstants.SETTINGS_FOLDER + "Templates/";

        public static void CreateScript()
        {
            string templatePath = $"{TEMPLATES_FOLDER}C#ScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
                CreateEditableTemplate(templatePath, "12b677268a71e8945b0b6e35e15d6983");

            templatePath = GetTempScriptTemplateFileWithNamespace(templatePath);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyClass.cs");
        }

#if UNITY_2019_3_OR_NEWER
        public static void CreateNodeScript()
        {
            string templatePath = $"{TEMPLATES_FOLDER}C#NodeScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
                CreateEditableTemplate(templatePath, "ffebfee6d47ee2d479894c0f294b7033");

            templatePath = GetTempScriptTemplateFileWithNamespace(templatePath);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyNode.cs");
        }

        public static void CreateGraphScript()
        {
            string templatePath = $"{TEMPLATES_FOLDER}C#GraphScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
                CreateEditableTemplate(templatePath, "917cf2d9a454951439fd980a95828bec");

            templatePath = GetTempScriptTemplateFileWithNamespace(templatePath);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyGraph.cs");
        }
#endif

        public static void CreateMetaFile(string targetAsset, bool isFolder, string assetGuid = null)
        {
            const string key = "guid:";

            string metaFileTemplate = AssetDatabase.GUIDToAssetPath("499c3a6e7a0f04c438e6d0f45ed563bb");
            string[] lines = File.ReadAllLines(metaFileTemplate);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(key))
                {
                    lines[i] = $"{key} {(assetGuid.HasAnyData() ? assetGuid : GUID.Generate().ToString())}";
                    break;
                }
            }

            string path = $"{targetAsset}.meta";

            if (isFolder)
            {
                List<string> list = new List<string>(lines);
                list.Insert(2, "folderAsset: yes");
                File.WriteAllLines(path, list);
            }
            else
            {
                File.WriteAllLines(path, lines);
            }
        }

        private static void CreateEditableTemplate(string templatePath, string baseTemplateGuid)
        {
            string sorceTemplatePath = AssetDatabase.GUIDToAssetPath(baseTemplateGuid);
            string text = File.ReadAllText(sorceTemplatePath);
            Directory.CreateDirectory(TEMPLATES_FOLDER);
            File.WriteAllText(templatePath, text);
        }

        private static string GetTempScriptTemplateFileWithNamespace(string templatePath)
        {
            string text = File.ReadAllText(templatePath)
                              .Replace("#NAMESPACE#", GetNameSpace());

            string tempFile = $"{"Temp/"}{Path.GetFileName(templatePath)}";
            File.WriteAllText(tempFile, text, Encoding.UTF8);

            return tempFile;
        }

        private static string GetNameSpace()
        {
            UnityObject selected = Selection.activeObject;
            string selectedPath = selected.GetAssetPath();

            if (selectedPath.IsNullOrEmpty() || selectedPath == "Assets")
                return "Project";

            if (!selected.IsFolder())
                selectedPath = PathUtility.GetParentPath(selectedPath);

            return selectedPath.Replace(AssetDatabaseExt.ASSET_FOLDER, string.Empty)
                               .Replace('/', '.')
                               .RemoveWhiteSpaces();
        }
    }
}
