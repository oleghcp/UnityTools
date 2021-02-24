using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Window
{
    internal class AboutWindow : EditorWindow
    {
        private string m_cr;
        private string m_descr;

        private void Awake()
        {
            minSize = new Vector2(350f, 150f);
            maxSize = new Vector2(350f, 150f);

            Assembly assembly = Assembly.Load(nameof(UnityUtility)) ?? Assembly.GetExecutingAssembly();

            var descriptionAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            m_descr = descriptionAttribute.Description;

            var copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            m_cr = copyrightAttribute.Copyright;
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);

            EditorScriptUtility.DrawCenterLabel(m_descr, 235f);
            EditorScriptUtility.DrawCenterLabel(m_cr, 140f);
        }
    }
}
