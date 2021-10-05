using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Controls.ControlStuff;

namespace UnityUtility.Controls
{
    [Serializable]
    public struct KeyAxes
    {
        [SerializeField]
        internal int Up;
        [SerializeField]
        internal int Down;
        [SerializeField]
        internal int Left;
        [SerializeField]
        internal int Right;

        public KeyAxes(int up, int down, int left, int right)
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
        }
    }

    [Serializable]
    public sealed class BindLayout
    {
        [SerializeField]
        internal readonly string Name;

        [SerializeField]
        private readonly int[] _keys;
        [SerializeField]
        private readonly int[] _axes;

        [SerializeField]
        internal readonly KeyAxes KeyAxes;
        [SerializeField]
        internal readonly InputType InputType;

        private int? _tmpButton;

        internal IReadOnlyList<int> Keys => _keys;
        internal IReadOnlyList<int> Axes => _axes;

        /// <summary>
        /// Constructor for gamepad layout.
        /// </summary>
        public BindLayout(string name, int[] keyIndices, int[] axisIndices)
        {
            Name = name;
            _keys = keyIndices.GetCopy();
            _axes = axisIndices;
            InputType = InputType.Gamepad;
        }

        /// <summary>
        /// Constructor for keyboard+mouse layout.
        /// </summary>
        public BindLayout(string name, int[] keyIndices, int[] axisIndices, in KeyAxes keyAxes)
        {
            Name = name;
            _keys = keyIndices.GetCopy();
            _axes = axisIndices;
            KeyAxes = keyAxes;
            InputType = InputType.KeyMouse;
        }

        public BindLayout(LayoutConfig config)
        {
            Name = config.name;
            _keys = config.KeyIndices.GetCopy();
            _axes = config.AxisIndices;
            KeyAxes = config.KeyAxes;
            InputType = config.InputType;
        }

        internal void AddTmpButton(int func, int keyCode)
        {
            if (_keys[func] == (int)KeyCode.None)
            {
                _tmpButton = func;
                _keys[func] = keyCode;
            }
        }

        internal void RemoveTmpButton()
        {
            if (_tmpButton.HasValue)
            {
                _keys[_tmpButton.Value] = (int)KeyCode.None;
                _tmpButton = null;
            }
        }
    }
}
