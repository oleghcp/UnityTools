using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityUtility.CSharp;

namespace UnityUtilityEditor
{
    internal static class TemplatesUtility
    {
        private const string TEMPLATES_FOLDER = "Templates/";

        public static void CreateScript()
        {
            string templatePath = $"{LibConstants.SETTINGS_FOLDER}{TEMPLATES_FOLDER}C#ScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
            {
                string text = @"using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityUtility;
using UnityObject = UnityEngine.Object;

namespace Project
{
    public class #SCRIPTNAME# : MonoBehaviour
    {

    }
}
";
                Directory.CreateDirectory($"{LibConstants.SETTINGS_FOLDER}{TEMPLATES_FOLDER}");
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyClass.cs");
        }

#if UNITY_2019_3_OR_NEWER
        public static void CreateNodeScript()
        {
            string templatePath = $"{LibConstants.SETTINGS_FOLDER}{TEMPLATES_FOLDER}C#NodeScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
            {
                string text = @"using System;
using UnityUtility.NodeBased;

namespace Project
{
    [Serializable]
    public class #SCRIPTNAME# : Node<#SCRIPTNAME#>
    {

    }
}
";
                Directory.CreateDirectory($"{LibConstants.SETTINGS_FOLDER}{TEMPLATES_FOLDER}");
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyNode.cs");
        }

        public static void CreateGraphScript()
        {
            string templatePath = $"{LibConstants.SETTINGS_FOLDER}{TEMPLATES_FOLDER}C#GraphScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
            {
                string text = "using UnityUtility.NodeBased;\nusing UnityEngine;\n\nnamespace Project\n{\n" +
                              $"\t[CreateAssetMenu(menuName = nameof({LibConstants.LIB_NAME}) + \"/Graph/\" + nameof(#SCRIPTNAME#), fileName = nameof(#SCRIPTNAME#))]\n" +
                              "\tpublic class #SCRIPTNAME# : Graph</*your node type*/>\n\t{\n\n\t}\n}\n";

                Directory.CreateDirectory($"{LibConstants.SETTINGS_FOLDER}{TEMPLATES_FOLDER}");
                File.WriteAllText(templatePath, text);
            }

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
    }
}
