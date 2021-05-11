using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Window
{
    internal class AboutWindow : EditorWindow
    {
        private string _copyright;
        private string _description;

        private void OnEnable()
        {
            minSize = maxSize = new Vector2(350f, 150f);

            Assembly assembly = Assembly.Load(nameof(UnityUtility)) ?? Assembly.GetExecutingAssembly();

            var descriptionAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            _description = descriptionAttribute.Description;

            var copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            _copyright = copyrightAttribute.Copyright;
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);

            EditorGuiLayout.CenterLabel(_description);
            EditorGuiLayout.CenterLabel(_copyright);
        }
    }
}
