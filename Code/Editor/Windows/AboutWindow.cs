using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UUEditor.Windows
{
    internal class AboutWindow : EditorWindow
    {
        private const string m_CR = "(C) Oleg Pulkin";
        private string m_descr;

        private void Awake()
        {
            minSize = new Vector2(350f, 150f);
            maxSize = new Vector2(350f, 150f);

            Assembly assembly = Assembly.Load("UnityUtility") ?? Assembly.GetExecutingAssembly();

            var descriptionAttribute = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            m_descr = (descriptionAttribute[0] as AssemblyDescriptionAttribute).Description;
        }

        private void OnGUI()
        {
            GUILayout.Space(10f);

            EditorScriptUtility.DrawCenterLabel(m_descr, 235f);
            EditorScriptUtility.DrawCenterLabel(m_CR, 140f);
        }
    }
}
