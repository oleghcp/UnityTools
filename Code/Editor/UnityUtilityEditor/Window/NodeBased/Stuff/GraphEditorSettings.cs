#if UNITY_2019_3_OR_NEWER
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    [Serializable]
    internal class GraphEditorSettings
    {
        private float _cameraPositionX;
        private float _cameraPositionY;

        public Vector2 CameraPosition
        {
            get => new Vector2(_cameraPositionX, _cameraPositionY);
            set
            {
                _cameraPositionX = value.x;
                _cameraPositionY = value.y;
            }
        }

        public static GraphEditorSettings Load(RawGraph asset)
        {
            string floderPath = Path.Combine(Application.persistentDataPath, "GraphSettings");
            Directory.CreateDirectory(floderPath);
            string filePath = Path.Combine(floderPath, asset.GetAssetGuid());

            if (!File.Exists(filePath))
                return new GraphEditorSettings();

            GraphEditorSettings graphSettings = BinaryFileUtility.Load<GraphEditorSettings>(filePath);
            return graphSettings ?? new GraphEditorSettings();
        }

        public static void Save(RawGraph asset, GraphEditorSettings settings)
        {
            string path = Path.Combine(Application.persistentDataPath, "GraphSettings");
            Directory.CreateDirectory(path);
            BinaryFileUtility.Save(Path.Combine(path, asset.GetAssetGuid()), settings);
        }
    }
}
#endif
