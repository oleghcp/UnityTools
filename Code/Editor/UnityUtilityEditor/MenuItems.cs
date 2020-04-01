using UnityUtilityEditor.Window;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace UnityUtilityEditor
{
    internal static class MenuItems
    {
        [MenuItem(EditorScriptUtility.CATEGORY + "/Sound/Create Sounds Preset")]
        private static void CreateSoundsPreset()
        {
            EditorUtilityExt.CreateScriptableObjectAsset("SoundsPreset");
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/Sound/Create Music Preset")]
        private static void CreateMusicPreset()
        {
            EditorUtilityExt.CreateScriptableObjectAsset("MusicPreset");
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/Input/Create Input Layout Config")]
        private static void CreateLayoutSet()
        {
            EditorUtilityExt.CreateScriptableObjectAsset("LayoutConfig", "LayoutConfig");
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/Input/Gamepad Axes")]
        private static void GetAxisCreateWindow()
        {
            EditorWindow.GetWindow(typeof(AxisCreateWindow), true, "Gamepad Axes");
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/GameObjects/Create Rect Plane")]
        private static void GetCreateRectPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Rect Plane", typeof(CreateRectPlaneWizard));
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/GameObjects/Create Figured Plane")]
        private static void GetCreateFiguredPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Figured Plane", typeof(CreateFiguredPlaneWizard));
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/GameObjects/Create Shape")]
        private static void GetCreateShapeWizard()
        {
            ScriptableWizard.DisplayWizard("Create Shape", typeof(CreateShapeWizard));
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/Assets/Create Scriptable Object")]
        private static void GetScriptableObjectWindow()
        {
            EditorWindow.GetWindow(typeof(ScriptableObjectWindow), true, "Scriptable Objects");
        }

        // ------------- //

        [MenuItem(EditorScriptUtility.CATEGORY + "/About", priority = 50)]
        private static void GetAboutWindow()
        {
            EditorWindow.GetWindow(typeof(AboutWindow), true, "About");
        }

        // ------------- //

        [MenuItem("Assets/Find References In Project (ext.)", priority = 25)]
        private static void FindReferences()
        {
            var targetGuid = Selection.assetGUIDs[0];
            var progress = new EditorUtilityExt.SearchProgress();
            var isCanseled = false;
            IEnumerator iterator;

            if (EditorSettings.serializationMode == SerializationMode.ForceText)
            {
                iterator = EditorUtilityExt.FindReferencesByText(targetGuid, progress);
            }
            else
            {
                iterator = EditorUtilityExt.FindReferences(targetGuid, progress);
            }

            EditorApplication.update += Proceed;

            void Proceed()
            {
                if (!isCanseled && iterator.MoveNext())
                {
                    isCanseled = EditorUtility.DisplayCancelableProgressBar("Searching references", "That could take a while...", progress.Progress);
                    return;
                }

                EditorApplication.update -= Proceed;

                EditorUtility.ClearProgressBar();

                if (progress.FoundObjects.Count == 0)
                {
                    Debug.Log("There are no dependencies.");
                    return;
                }

                ReferencesWindow.Create(targetGuid, progress.FoundObjects);
            }
        }

        [MenuItem("Assets/Find References In Project (ext.)", true)]
        private static bool FindRefsValidation()
        {
            return Selection.assetGUIDs.Length == 1;
        }

        [MenuItem("Assets/Show Guid (ext.)", priority = 20)]
        private static void ShowGUID()
        {
            var guids = Selection.assetGUIDs;

            for (int i = 0; i < guids.Length; i++)
                Debug.Log(guids[i]);
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
using UnityUtility;

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
