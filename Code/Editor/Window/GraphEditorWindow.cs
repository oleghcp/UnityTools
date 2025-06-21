using OlegHcp.NodeBased.Service;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Window.NodeBased;
using OlegHcpEditor.Window.NodeBased.NodeDrawing;
using OlegHcpEditor.Window.NodeBased.Stuff;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window
{
    internal class GraphEditorWindow : EditorWindow
    {
        private SerializedGraph _serializedGraph;
        private GraphEditorSettings _settings;
        private GraphCamera _camera;
        private GraphSidePanel _sidePanel;
        private GraphMap _map;
        private GraphToolbar _toolbar;

        private int _onGuiCounter;

        private Vector2 _mapSize;
        private Vector2 _winSize;
        private int _rootNodeId;

        private int _mapSizeVersion;
        private int _winSizeVersion;
        private int _rootNodeVersion;

        public SerializedGraph SerializedGraph => _serializedGraph;
        public int OnGuiCounter => _onGuiCounter;
        public GraphCamera Camera => _camera;
        public bool GridSnapping => _toolbar.GridToggle;
        public bool ShowPorts => _toolbar.ShowPorts;
        public GraphEditorSettings Settings => _settings;
        public GraphMap Map => _map;
        public bool FullDrawing => !_toolbar.HideContentToggle;
        public GraphSidePanel SidePanel => _sidePanel;

        public Vector2 MapSize
        {
            get
            {
                if (_mapSizeVersion != _onGuiCounter)
                {
                    _mapSize = new Vector2(position.width - _sidePanel.Width, position.height - GraphToolbar.HEIGHT);
                    _mapSizeVersion = _onGuiCounter;
                }

                return _mapSize;
            }
        }

        public int RootNodeId
        {
            get
            {
                if (_rootNodeVersion != _onGuiCounter)
                {
                    _rootNodeId = _serializedGraph.RootNodeProperty.intValue;
                    _rootNodeVersion = _onGuiCounter;
                }

                return _rootNodeId;
            }
        }

        private void OnGUI()
        {
            if (_serializedGraph.GraphAsset == null)
            {
                Close();
                return;
            }

            _onGuiCounter++;

            _serializedGraph.SerializedObject.Update();

            Event guiEvent = Event.current;
            Vector2 mapSize = MapSize;

            _toolbar.Draw(guiEvent);
            _sidePanel.Draw(_toolbar.SidePanelToggle, mapSize.y, position.width, guiEvent);

            Rect mapRect = new Rect(0f, 0f, mapSize.x, mapSize.y);
            GUI.BeginGroup(mapRect);
            _camera.ProcessEvents(guiEvent, mapRect);
            _map.Draw(guiEvent);
            GUI.EndGroup();

            _serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void OnLostFocus()
        {
            if (_serializedGraph.GraphAsset == null)
                return;

            Save();
        }

        private void OnDestroy()
        {
            if (_serializedGraph.GraphAsset == null)
                return;

            OnClose();
        }

        public static void Open(RawGraph graphAsset)
        {
            GraphEditorWindow window = GetWindow<GraphEditorWindow>(true, $"{graphAsset.name} Editor");
            window.minSize = new Vector2(800f, 600f);
            window.SetUp(graphAsset);
        }

        public void SetAsRoot(NodeViewer node)
        {
            _serializedGraph.SerializedObject.Update();
            _serializedGraph.SetAsRoot(node);
            _serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void SetUp(RawGraph graphAsset)
        {
            if (_serializedGraph == null)
            {
                _settings = GraphEditorSettings.Load(graphAsset.GetAssetGuid());
                _serializedGraph = new SerializedGraph(graphAsset);
                _sidePanel = new GraphSidePanel(this);
                _map = new GraphMap(this);
                _toolbar = new GraphToolbar(this);
                _camera = new GraphCamera(this);
            }
            else if (_serializedGraph.GraphAsset != graphAsset)
            {
                OnClose();

                _settings = GraphEditorSettings.Load(graphAsset.GetAssetGuid());
                _serializedGraph.InitAssetReference(graphAsset);
                _camera.Position = _settings.CameraPosition;
                _map.Clear();
            }
            else
            {
                return;
            }

            _sidePanel.OnOpen();
            _map.Fill();
        }

        private void OnClose()
        {
            _sidePanel.OnClose();
            Save();
        }

        private void Save()
        {
            _map.Save();
            _serializedGraph.Save();
            _toolbar.Save();
            _sidePanel.Save();
            _serializedGraph.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
            _settings.CameraPosition = _camera.Position;
            _settings.Save(_serializedGraph.GraphAsset.GetAssetGuid());
            AssetDatabase.SaveAssets();
        }
    }
}
