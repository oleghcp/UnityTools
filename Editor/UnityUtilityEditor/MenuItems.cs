using UUEditor.Windows;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UUEditor
{
    internal static class MenuItems
    {
        [MenuItem(EditorScriptUtility.Category + "/Sound/Create Sounds Preset")]
        private static void CreateSoundsPreset()
        {
            EditorScriptUtility.CreateAsset("SoundsPreset");
        }

        [MenuItem(EditorScriptUtility.Category + "/Sound/Create Music Preset")]
        private static void CreateMusicPreset()
        {
            EditorScriptUtility.CreateAsset("MusicPreset");
        }

        [MenuItem(EditorScriptUtility.Category + "/Input/Create Input Layout Config")]
        private static void CreateLayoutSet()
        {
            EditorScriptUtility.CreateAsset("LayoutConfig", "LayoutConfig");
        }

        [MenuItem(EditorScriptUtility.Category + "/Input/Gamepad Axes")]
        private static void GetAxisCreateWindow()
        {
            EditorWindow.GetWindow(typeof(AxisCreateWindow), true, "Gamepad Axes");
        }

        [MenuItem(EditorScriptUtility.Category + "/GameObjects/Create Rect Plane")]
        private static void GetCreateRectPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Rect Plane", typeof(CreateRectPlaneWizard));
        }

        [MenuItem(EditorScriptUtility.Category + "/GameObjects/Create Figured Plane")]
        private static void GetCreateFiguredPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Figured Plane", typeof(CreateFiguredPlaneWizard));
        }

        [MenuItem(EditorScriptUtility.Category + "/GameObjects/Create Shape")]
        private static void GetCreateShapeWizard()
        {
            ScriptableWizard.DisplayWizard("Create Shape", typeof(CreateShapeWizard));
        }

        [MenuItem(EditorScriptUtility.Category + "/Assets/Create Scriptable Object")]
        private static void GetScriptableObjectWindow()
        {
            EditorWindow.GetWindow(typeof(ScriptableObjectWindow), true, "Scriptable Objects");
        }

        // ------------- //

        [MenuItem(EditorScriptUtility.Category + "/About", priority = 50)]
        private static void GetAboutWindow()
        {
            EditorWindow.GetWindow(typeof(AboutWindow), true, "About");
        }

        // ------------- //

        [MenuItem("Assets/Find References In Project (ext.)", priority = 25)]
        private static void FindRefs()
        {
            string guid = Selection.assetGUIDs[0];
            string assetsFolderPath = Application.dataPath;
            bool hasRefs = false;

            void check(string ext)
            {
                string[] files = Directory.GetFiles(assetsFolderPath, ext, SearchOption.AllDirectories);

                foreach (var filePath in files)
                {
                    string text = File.ReadAllText(filePath);

                    if (text.Contains(guid))
                    {
                        hasRefs = true;
                        Debug.Log(filePath.Remove(0, assetsFolderPath.Length));
                    }
                }
            }

            check("*.prefab");
            check("*.unity");
            check("*.asset");

            if (!hasRefs)
                Debug.Log("There is no references.");
        }

        [MenuItem("Assets/Show Guid (ext.)", priority = 20)]
        private static void ShowGUID()
        {
            Debug.Log(Selection.assetGUIDs[0]);
        }

        [MenuItem("Assets/Find References In Project (ext.)", true)]
        [MenuItem("Assets/Show Guid (ext.)", true)]
        private static bool f_validateObjects()
        {
            return Selection.assetGUIDs.Length == 1;
        }

#if UNITY_2019_1_OR_NEWER
        private static string s_templatePath;
        [MenuItem("Assets/Create/C# Script (ext.)", priority = 81)]
        private static void CreateScript()
        {
            if (s_templatePath == null)
                s_templatePath = Path.Combine(PathExt.GetParentPath(Application.dataPath), "C#ScriptTemplate.cs.txt");

            if (!File.Exists(s_templatePath))
            {
                string text = @"using UnityObject = UnityEngine.Object;

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UU;
using UU.MathExt;

namespace Project
{
    public class #SCRIPTNAME# : Script
    {

    }
}
";
                File.WriteAllText(s_templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(s_templatePath, "MyClass.cs");
        }
#endif
    }
}
