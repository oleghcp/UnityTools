using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Configs;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.SettingsProviders
{
    internal class ProjectBaseSettingsProvider : SettingsProvider
    {
        public ProjectBaseSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new ProjectBaseSettingsProvider($"{SettingsProviderUtility.PROJECT_SECTION_NAME}/{LibConstants.LIB_NAME}", SettingsScope.Project);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space();

            DrawNamespaceRootFolder();
        }

        private void DrawNamespaceRootFolder()
        {
            GUILayoutOption buttonWidth = GUILayout.Width(60f);
            GUILayoutOption slashWidth = GUILayout.Width(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Namespace Root Folder", GUILayout.Width(SettingsProviderUtility.LABEL_WIDTH));

            int c = LibrarySettings.I.NamespaceFolderRootSkipSteps + 1;
            for (int i = 0; i < c; i++)
            {
                string text = i == 0 ? "Assets" : "...";
                bool shouldBeToggled = i < c - 1;
                bool toggled = EditorGuiLayout.ToggleButton(text, shouldBeToggled, buttonWidth);
                GUILayout.Label("/", slashWidth);

                if (toggled != shouldBeToggled)
                {
                    if (toggled)
                    {
                        LibrarySettings.I.SetNamespaceFolderRootSkipSteps(i + 1);
                    }
                    else
                    {
                        LibrarySettings.I.SetNamespaceFolderRootSkipSteps(i);
                        break;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
