using System.IO;
using UnityEditor;

namespace UnityUtilityEditor
{
    internal static class TemplatesUtility
    {
#if UNITY_2019_1_OR_NEWER
        public static void CreateScript()
        {
            string templatePath = EditorUtilityExt.TEMPLATES_FOLDER + "C#ScriptTemplate.cs.txt";

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
                Directory.CreateDirectory(EditorUtilityExt.TEMPLATES_FOLDER);
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyClass.cs");
        }
#endif

#if UNITY_2019_3_OR_NEWER
        public static void CreateNodeScript()
        {
            string templatePath = EditorUtilityExt.TEMPLATES_FOLDER + "C#NodeScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
            {
                string text = @"using UnityUtility.NodeBased;

namespace Project
{
    public class #SCRIPTNAME# : Node
    {

    }
}
";
                Directory.CreateDirectory(EditorUtilityExt.TEMPLATES_FOLDER);
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyNode.cs");
        }

        public static void CreateTransitionScript()
        {
            string templatePath = EditorUtilityExt.TEMPLATES_FOLDER + "C#TransitionScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
            {
                string text = @"using System;
using UnityUtility.NodeBased;

namespace Project
{
    [Serializable]
    public class #SCRIPTNAME# : Transition</*your node type*/>
    {

    }
}
";
                Directory.CreateDirectory(EditorUtilityExt.TEMPLATES_FOLDER);
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyTransition.cs");
        }

        public static void CreateGraphScript()
        {
            string templatePath = EditorUtilityExt.TEMPLATES_FOLDER + "C#GraphScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
            {
                string text = "using UnityUtility.NodeBased;\nusing UnityEngine;\n\nnamespace Project\n{\n" +
                              $"\t[CreateAssetMenu(menuName = nameof({nameof(UnityUtility)}) + \" (ext.)/Graph/\" + nameof(#SCRIPTNAME#), fileName = nameof(#SCRIPTNAME#))]\n" +
                              "\tpublic class #SCRIPTNAME# : Graph</*your node type*/, /*your transition type*/>\n\t{\n\n\t}\n}\n";
                
                Directory.CreateDirectory(EditorUtilityExt.TEMPLATES_FOLDER);
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyGraph.cs");
        }
#endif
    }
}
