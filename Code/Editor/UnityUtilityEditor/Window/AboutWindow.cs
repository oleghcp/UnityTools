using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Window
{
    internal class AboutWindow : EditorWindow
    {
        private string _copyright;
        private string _description;

        private void Awake()
        {
            minSize = new Vector2(350f, 150f);
            maxSize = new Vector2(350f, 150f);

            Assembly assembly = Assembly.Load(nameof(UnityUtility)) ?? Assembly.GetExecutingAssembly();

            var descriptionAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            _description = descriptionAttribute.Description;

            var copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            _copyright = copyrightAttribute.Copyright;
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);

            GUIExt.DrawCenterLabel(_description, 235f);
            GUIExt.DrawCenterLabel(_copyright, 140f);
        }
    }
}
