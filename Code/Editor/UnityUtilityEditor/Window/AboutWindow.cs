using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Window
{
    internal class AboutWindow : EditorWindow
    {
        private string _copyright;
        private string _description1;
        private string _description2 = "Supports Unity 2018.3 or higher.";

        private void OnEnable()
        {
            minSize = maxSize = new Vector2(350f, 150f);

            Assembly assembly = Assembly.Load(nameof(UnityUtility)) ?? Assembly.GetExecutingAssembly();

            var descriptionAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            _description1 = descriptionAttribute.Description;

            var copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            _copyright = copyrightAttribute.Copyright;
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);

            EditorGuiLayout.CenterLabel(_description1);
            EditorGuiLayout.CenterLabel(_description2);

            EditorGUILayout.Space();

            EditorGuiLayout.CenterLabel(_copyright);
        }
    }
}
