﻿using UnityUtility.Controls.ControlStuff;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        internal int[] Keys;
        [SerializeField]
        internal int[] Axes;

        [SerializeField]
        internal KeyAxes KeyAxes;

        [SerializeField]
        internal InputType InputType;

        private List<int> _tmpButtons;

        /// <summary>
        /// Constructor for gamepad layout.
        /// </summary>
        public BindLayout(int[] keyIndices, int[] axisIndices)
        {
            Keys = keyIndices.GetCopy();
            Axes = axisIndices.GetCopy();
            InputType = InputType.Gamepad;
        }

        /// <summary>
        /// Constructor for keyboard+mouse layout.
        /// </summary>
        public BindLayout(int[] keyIndices, int[] axisIndices, in KeyAxes keyAxes)
        {
            Keys = keyIndices.GetCopy();
            Axes = axisIndices.GetCopy();
            KeyAxes = keyAxes;
            InputType = InputType.KeyMouse;
        }

        public BindLayout(LayoutConfig config)
        {
            Keys = config.KeyIndices.GetCopy();
            Axes = config.AxisIndices.GetCopy();
            KeyAxes = config.KeyAxes;
            InputType = config.InputType;
        }

        internal void AddTmpButton(int func, int keyCode)
        {
            if (Keys[func] == (int)KeyCode.None)
            {
                if (_tmpButtons == null)
                    _tmpButtons = new List<int>();

                _tmpButtons.Add(func);
                Keys[func] = keyCode;
            }
        }

        internal void RemoveTmpButton()
        {
            if (_tmpButtons != null && _tmpButtons.Count > 0)
            {
                for (int i = 0; i < _tmpButtons.Count; i++)
                {
                    Keys[_tmpButtons[i]] = (int)KeyCode.None;
                }

                _tmpButtons.Clear();
            }
        }
    }
}
