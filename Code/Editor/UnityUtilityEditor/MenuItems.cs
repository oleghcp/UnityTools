using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor
{
    internal static class MenuItems
    {
        [MenuItem(EditorScriptUtility.CATEGORY + "/Remove Empty Folders")]
        private static void RemoveEmptyFolders()
        {
            removeEmptyFolders(Application.dataPath);

            Debug.Log("Done");

            void removeEmptyFolders(string path)
            {
                IEnumerable<string> directories = Directory.EnumerateDirectories(path);

                foreach (string directory in directories)
                {
                    removeEmptyFolders(directory);
                }

                IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(path);

                if (entries.Any()) { return; }

                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                AssetDatabase.DeleteAsset(relativePath);

                Debug.Log("Deleted: " + relativePath);
            }
        }

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

        [MenuItem(EditorScriptUtility.CATEGORY + "/Objects/Meshes/Create Rect Plane")]
        private static void GetCreateRectPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Rect Plane", typeof(CreateRectPlaneWizard));
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/Objects/Meshes/Create Figured Plane")]
        private static void GetCreateFiguredPlaneWizard()
        {
            ScriptableWizard.DisplayWizard("Create Figured Plane", typeof(CreateFiguredPlaneWizard));
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/Objects/Meshes/Create Shape")]
        private static void GetCreateShapeWizard()
        {
            ScriptableWizard.DisplayWizard("Create Shape", typeof(CreateShapeWizard));
        }

        [MenuItem(EditorScriptUtility.CATEGORY + "/Objects/Create Scriptable Object")]
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
            string targetGuid = Selection.assetGUIDs[0];
            EditorUtilityExt.SearchProgress progress = new EditorUtilityExt.SearchProgress();
            bool isCanseled = false;
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
            string[] guids = Selection.assetGUIDs;

            for (int i = 0; i < guids.Length; i++)
            {
                Debug.Log(guids[i]);
            }
        }

#if UNITY_2020_2_OR_NEWER
        private const int CREATE_CS_SCRIPT_PRIORITY = 80;
#elif UNITY_2019_1_OR_NEWER
        private const int CREATE_CS_SCRIPT_PRIORITY = 81;
#endif

#if UNITY_2019_1_OR_NEWER
        [MenuItem("Assets/Create/C# Script (ext.)", priority = CREATE_CS_SCRIPT_PRIORITY)]
        private static void CreateScript()
        {
            string templatePath = Path.Combine(PathUtility.GetParentPath(Application.dataPath), "C#ScriptTemplate.cs.txt");

            if (!File.Exists(templatePath))
            {
                string text = @"using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityUtility;
using UnityObject = UnityEngine.Object;

namespace Project
{
    public class #SCRIPTNAME# : MonoBehaviour
    {

    }
}
";
                File.WriteAllText(templatePath, text);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "MyClass.cs");
        }
#endif
    }
}
