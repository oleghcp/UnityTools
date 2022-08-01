using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using Selection = UnityEditor.Selection;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor
{
    internal static class ComponentMenuItems
    {
        public const string MENU_PARENT = "Component/" + nameof(UnityUtility) + "/";

        [MenuItem(MENU_PARENT + nameof(CameraFitter))]
        private static void AddCameraFitter()
        {
            AddComponent<CameraFitter>();
        }

        [MenuItem(MENU_PARENT + nameof(RenderSorter))]
        private static void AddRenderSorter()
        {
            AddComponent<RenderSorter>();
        }

#if UNITY_2019_3_OR_NEWER
        [MenuItem(MENU_PARENT + nameof(UnityUtility.AiSimulation.AiBehavior))]
        private static void AddAiBehavior()
        {
            AddComponent<UnityUtility.AiSimulation.AiBehavior>();
        }
#endif       

#if !UNITY_2019_1_OR_NEWER || INCLUDE_AUDIO
        [MenuItem(MENU_PARENT + nameof(UnityUtility.Sound.SoundStuff.SoundInfo))]
        private static void AddSoundInfo()
        {
            AddComponent<UnityUtility.Sound.SoundStuff.SoundInfo>();
        }

        [MenuItem(MENU_PARENT + nameof(UnityUtility.Sound.SoundStuff.MusicInfo))]
        private static void AddMusicInfo()
        {
            AddComponent<UnityUtility.Sound.SoundStuff.MusicInfo>();
        }
#endif

#if UNITY_2019_3_OR_NEWER && INCLUDE_PHYSICS
        [MenuItem(MENU_PARENT + nameof(UnityUtility.Shooting.Projectile))]
        private static void AddProjectile()
        {
            AddComponent<UnityUtility.Shooting.Projectile>();
        }

        [MenuItem(MENU_PARENT + nameof(UnityUtility.Shooting.Projectile2D))]
        private static void AddProjectile2D()
        {
            AddComponent<UnityUtility.Shooting.Projectile2D>();
        }
#endif

        [MenuItem(MENU_PARENT + nameof(CameraFitter), true)]
        [MenuItem(MENU_PARENT + nameof(RenderSorter), true)]
#if UNITY_2019_3_OR_NEWER
        [MenuItem(MENU_PARENT + nameof(UnityUtility.AiSimulation.AiBehavior), true)]
#endif
#if !UNITY_2019_1_OR_NEWER || INCLUDE_AUDIO
        [MenuItem(MENU_PARENT + nameof(UnityUtility.Sound.SoundStuff.MusicInfo), true)]
        [MenuItem(MENU_PARENT + nameof(UnityUtility.Sound.SoundStuff.SoundInfo), true)]
#endif
#if UNITY_2019_3_OR_NEWER && INCLUDE_PHYSICS
        [MenuItem(MENU_PARENT + nameof(UnityUtility.Shooting.Projectile), true)]
        [MenuItem(MENU_PARENT + nameof(UnityUtility.Shooting.Projectile2D), true)]
#endif
        private static bool AddComponentValidation()
        {
            UnityObject[] objects = Selection.objects;

            return objects.Length > 0 && objects.All(item => item is GameObject);
        }

        private static void AddComponent<T>() where T : MonoBehaviour
        {
            foreach (GameObject item in Selection.objects)
            {
                Undo.AddComponent<T>(item);
            }
        }
    }
}
