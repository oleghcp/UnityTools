using System;
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
    }
}
