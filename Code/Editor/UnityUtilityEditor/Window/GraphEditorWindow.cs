#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased.Stuff;
using UnityUtilityEditor.Window.NodeBased.Stuff.NodeDrawing;

namespace UnityUtilityEditor.Window
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
        public TransitionViewType TransitionView => _toolbar.TransitionView;
        public GraphEditorSettings Settings => _settings;
        public GraphMap Map => _map;
        public bool FullDrawing => !_toolbar.HideContentToggle;

        public Vector2 WinSize
        {
            get
            {
                if (_winSizeVersion != _onGuiCounter)
                {
                    _winSize.x = position.width;
                    _winSize.y = position.height;
                    _winSizeVersion = _onGuiCounter;
                }

                return _winSize;
            }
        }

        public Vector2 MapSize
        {
            get
            {
                if (_mapSizeVersion != _onGuiCounter)
                {
                    _mapSize = WinSize - new Vector2(_sidePanel.Width, GraphToolbar.HEIGHT);
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

            _toolbar.Draw();
            _sidePanel.Draw(_toolbar.SidePanelToggle, MapSize.y, WinSize.x, Event.current);

            Rect mapRect = new Rect(new Vector2(_sidePanel.Width, 0f), MapSize);
            GUI.BeginGroup(mapRect);

            _camera.ProcessEvents(Event.current);
            if (_camera.IsDragging)
                EditorGUIUtility.AddCursorRect(mapRect, MouseCursor.Pan);

            _map.Draw(Event.current);

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

            Save();
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
                Save();

                _settings = GraphEditorSettings.Load(graphAsset.GetAssetGuid());
                _serializedGraph.InitAssetReference(graphAsset);
                _camera.Position = _settings.CameraPosition;
                _map.Clear();
            }
            else
            {
                return;
            }

            _map.Fill();
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
            EditorUtilityExt.SaveProject();
        }
    }
}
#endif
