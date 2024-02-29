using System;
using System.IO;
using OlegHcp.IO;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased.Stuff
{
    [Serializable]
    internal class GraphEditorSettings
    {
        private const string GRAPH_SETTINGS_NAME = "GraphSettings";

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
            string folderPath = $"{LibConstants.TEMP_SETTINGS_FOLDER}{GRAPH_SETTINGS_NAME}";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, name);
            if (!File.Exists(filePath))
                return new GraphEditorSettings();

            GraphEditorSettings graphSettings = BinaryFileUtility.Load<GraphEditorSettings>(filePath);
            return graphSettings ?? new GraphEditorSettings();
        }

        public void Save(string name)
        {
            string folderPath = $"{LibConstants.TEMP_SETTINGS_FOLDER}{GRAPH_SETTINGS_NAME}";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            BinaryFileUtility.Save(Path.Combine(folderPath, name), this);
        }
    }
}
