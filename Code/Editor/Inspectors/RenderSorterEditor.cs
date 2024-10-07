using OlegHcp;
using OlegHcpEditor.Gui;
using OlegHcpEditor.MenuItems;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
{
    [CustomEditor(typeof(RenderSorter))]
    internal class RenderSorterEditor : Editor<RenderSorter>
    {
        private SortingLayerDrawTool _drawer;
        private Renderer _renderer;

        private void OnEnable()
        {
            _drawer = new SortingLayerDrawTool();

            SerializedProperty rendererProp = serializedObject.FindProperty(RenderSorter.RendererFieldName);
            _renderer = rendererProp.objectReferenceValue as Renderer;

            if (_renderer == null || _renderer.gameObject != target.gameObject)
            {
                rendererProp.objectReferenceValue = _renderer = target.GetComponent<Renderer>();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public override void OnInspectorGUI()
        {
            _drawer.Draw(_renderer);
        }

        [MenuItem(MenuItemsUtility.CONTEXT_MENU_NAME + nameof(RenderSorter) + "/" + MenuItemsUtility.RESET_ITEM_NAME)]
        private static void ResetMenuItem(MenuCommand command)
        {
            Renderer renderer = (command.context as RenderSorter).Renderer;

            Undo.RegisterCompleteObjectUndo(renderer, "Renderer sorting");

            renderer.sortingLayerID = 0;
            renderer.sortingOrder = 0;

            EditorUtility.SetDirty(renderer);
        }
    }
}
