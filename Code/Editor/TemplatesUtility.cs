using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using OlegHcp.CSharp;
using OlegHcp.IO;
using OlegHcpEditor.Configs;
using OlegHcpEditor.Engine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor
{
    internal static class TemplatesUtility
    {
        private const string TEMPLATES_FOLDER = AssetDatabaseExt.USER_SETTINGS_FOLDER + "Templates/";

        public static void CreateScript()
        {
            string templatePath = $"{TEMPLATES_FOLDER}C#ScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
                CreateEditableTemplate(templatePath, "12b677268a71e8945b0b6e35e15d6983");

            templatePath = GetTempScriptTemplateFileWithNamespace(templatePath);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyClass.cs");
        }

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

            if (selected == null)
                return "Assets";

            string targetPath = selected.GetAssetPath();

            if (!selected.IsFolder())
                targetPath = PathUtility.GetParentPath(targetPath);

            int steps = LibrarySettings.NamespaceFolderRootSkipSteps;
            string trimmedPath = PathUtility.SkipRootSteps(targetPath, steps);

            return trimmedPath.HasAnyData() ? pathToNamespace(trimmedPath)
                                            : pathToNamespace(targetPath);

            string pathToNamespace(string path)
            {
                string[] names = path.Split('/');

                int index = names.IndexOf(nameof(Editor));
                if (index >= 0)
                    names[index] = LibrarySettings.EditorFolderNamespace;

                return names.ConcatToString('.')
                            .RemoveWhiteSpaces();
            }
        }
    }
}
