#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Controls.ControlStuff;
using UnityUtility.CSharp;

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
        private int[] _keys;
        [SerializeField]
        private int[] _axes;

        [SerializeField]
        private KeyAxes _keyAxes;
        [SerializeField]
        private InputType _inputType;

        private int? _tmpButton;

        internal IReadOnlyList<int> Keys => _keys;
        internal IReadOnlyList<int> Axes => _axes;
        internal KeyAxes KeyAxes => _keyAxes;
        public InputType InputType => _inputType;

        /// <summary>
        /// Constructor for gamepad layout.
        /// </summary>
        public BindLayout(int[] keyIndices, int[] axisIndices)
        {
            _keys = keyIndices.GetCopy();
            _axes = axisIndices;
            _inputType = InputType.Gamepad;
        }

        /// <summary>
        /// Constructor for keyboard+mouse layout.
        /// </summary>
        public BindLayout(int[] keyIndices, int[] axisIndices, in KeyAxes keyAxes)
        {
            _keys = keyIndices.GetCopy();
            _axes = axisIndices;
            _keyAxes = keyAxes;
            _inputType = InputType.KeyMouse;
        }

        public BindLayout(LayoutConfig config)
        {
            _keys = config.KeyIndices.GetCopy();
            _axes = config.AxisIndices;
            _keyAxes = config.KeyAxes;
            _inputType = config.InputType;
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
#endif
