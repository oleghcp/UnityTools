using System;
using System.IO;
using OlegHcp.Engine;
using OlegHcpEditor.Utils;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased.Stuff
{
    [Serializable]
    internal class GraphEditorSettings
    {
        private const string GRAPH_SETTINGS_NAME = "GraphSettings";

        [SerializeField]
        private float _cameraPositionX;
        [SerializeField]
        private float _cameraPositionY;

        public Vector2 CameraPosition
        {
            get => new Vector2(_cameraPositionX, _cameraPositionY);
            set => value.Deconstruct(out _cameraPositionX, out _cameraPositionY);
        }

        public static GraphEditorSettings Load(string name)
        {
            string folderPath = $"{LibConstants.TEMP_SETTINGS_FOLDER}{GRAPH_SETTINGS_NAME}";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, name);
            if (!File.Exists(filePath))
                return new GraphEditorSettings();

#if UNITY_6000_4_OR_NEWER
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<GraphEditorSettings>(json);
            }
            catch
            {
                return new GraphEditorSettings();
            }
#else
            GraphEditorSettings graphSettings = OlegHcp.IO.BinaryFileUtility.Load<GraphEditorSettings>(filePath);
            return graphSettings ?? new GraphEditorSettings();
#endif
        }

        public void Save(string name)
        {
            string folderPath = $"{LibConstants.TEMP_SETTINGS_FOLDER}{GRAPH_SETTINGS_NAME}";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string path = Path.Combine(folderPath, name);

#if UNITY_6000_4_OR_NEWER
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(path, json);
#else
            OlegHcp.IO.BinaryFileUtility.Save(path, this);
#endif
        }
    }
}
