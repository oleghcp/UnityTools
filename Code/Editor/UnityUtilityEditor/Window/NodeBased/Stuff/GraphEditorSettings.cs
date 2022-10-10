#if UNITY_2019_3_OR_NEWER
using System;
using System.IO;
using UnityEngine;

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

        public static GraphEditorSettings Load(string name)
        {
            string floderPath = Path.Combine(Application.persistentDataPath, "GraphSettings");
            Directory.CreateDirectory(floderPath);
            string filePath = Path.Combine(floderPath, name);

            if (!File.Exists(filePath))
                return new GraphEditorSettings();

            GraphEditorSettings graphSettings = BinaryFileUtility.Load<GraphEditorSettings>(filePath);
            return graphSettings ?? new GraphEditorSettings();
        }

        public void Save(string name)
        {
            string path = Path.Combine(Application.persistentDataPath, "GraphSettings");
            Directory.CreateDirectory(path);
            BinaryFileUtility.Save(Path.Combine(path, name), this);
        }
    }
}
#endif
