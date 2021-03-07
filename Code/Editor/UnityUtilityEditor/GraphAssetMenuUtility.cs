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
            string templatePath = "C#NodeScriptTemplate.cs.txt";

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
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyNode.cs");
        }

        public static void CreateTransitionScript()
        {
            string templatePath = "C#TransitionScriptTemplate.cs.txt";

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
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyTransition.cs");
        }

        public static void CreateGraphScript()
        {
            string templatePath = "C#GraphScriptTemplate.cs.txt";

            if (!File.Exists(templatePath))
            {
                string text = @"using UnityUtility.NodeBased;
using UnityEngine;

namespace Project
{" +
    "\n    [CreateAssetMenu(menuName = \"Graph (ext.)/Graph\", fileName = \"MyGraph\")]\n" +
    @"    public class #SCRIPTNAME# : Graph</*your node type*/, /*your transition type*/>
    {

    }
}
";
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyGraph.cs");
        }
    }
}
#endif
