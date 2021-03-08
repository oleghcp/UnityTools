using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor
{
    internal class GraphAssetMenuUtility : MonoBehaviour
    {
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
                string text = @"using UnityUtility.NodeBased;

namespace Project
{
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
                string text = @"using UnityUtility.NodeBased;
using UnityEngine;

namespace Project
{" +
    "\n    [CreateAssetMenu(menuName = \"Graph (ext.)/#SCRIPTNAME#\", fileName = \"#SCRIPTNAME#\")]\n" +
    @"    public class #SCRIPTNAME# : Graph</*your node type*/, /*your transition type*/>
    {

    }
}
";
                Directory.CreateDirectory(EditorUtilityExt.TEMPLATES_FOLDER);
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyGraph.cs");
        }
    }
}
#endif
