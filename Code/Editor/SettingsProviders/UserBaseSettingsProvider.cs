using System.Collections.Generic;
using OlegHcp.CSharp;
using OlegHcpEditor.Configs;
using OlegHcpEditor.Utils;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.SettingsProviders
{
    internal class UserBaseSettingsProvider : SettingsProvider
    {
        public UserBaseSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new UserBaseSettingsProvider($"{SettingsProviderUtility.USER_SECTION_NAME}/{LibConstants.LIB_NAME}", SettingsScope.User);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = SettingsProviderUtility.LABEL_WIDTH;

            LibrarySettings.OpenFoldersByDoubleClick = DrawToggle("Open Folders by Double Click", LibrarySettings.OpenFoldersByDoubleClick);
            if (LibrarySettings.OpenFoldersByDoubleClick)
                EditorGUILayout.HelpBox("Folders opening  by double click works only with one column layout of Project window.", MessageType.Info);

            LibrarySettings.OpenScriptableAssetsAsCode = DrawToggle("Open Scriptable Assets as Code", LibrarySettings.OpenScriptableAssetsAsCode);

            LibrarySettings.SuppressedWarningsInIde = DrawText("Suppressed Warnings in IDE",
                                                               LibrarySettings.SuppressedWarningsInIde,
                                                               "If you specify multiple warning numbers, separate them with a comma. \n Example: CS0649,CS0169");

            EditorGUILayout.Space();

            GUILayout.Label("Creating script from template via menu item \"Create/C# Script (ext.)\"", EditorStyles.boldLabel);
            DrawNamespaceRootFolder();
            string editorFolderNamespace = EditorGUILayout.TextField("Editor Folder Namespace", LibrarySettings.EditorFolderNamespace);
            if (editorFolderNamespace.HasUsefulData())
                LibrarySettings.EditorFolderNamespace = editorFolderNamespace;
        }

        private bool DrawToggle(string label, bool curValue)
        {
            return EditorGUILayout.Toggle(EditorGuiUtility.TempContent(label), curValue);
        }

        private string DrawText(string label, string curValue, string tooltip = null)
        {
            return EditorGUILayout.TextField(EditorGuiUtility.TempContent(label, tooltip), curValue);
        }

        private void DrawNamespaceRootFolder()
        {
            GUILayoutOption buttonWidth = GUILayout.Width(60f);
            GUILayoutOption slashWidth = GUILayout.Width(10f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Namespace Root Folder", GUILayout.Width(SettingsProviderUtility.LABEL_WIDTH));

            int c = LibrarySettings.NamespaceFolderRootSkipSteps + 1;
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
                        LibrarySettings.NamespaceFolderRootSkipSteps = i + 1;
                    }
                    else
                    {
                        LibrarySettings.NamespaceFolderRootSkipSteps = i;
                        break;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
