using UnityEngine;
using System.Collections.Generic;
using UnityUtility.Controls.ControlStuff;
using System;
using System.Runtime.CompilerServices;

namespace UnityUtility.Controls
{
    public sealed class KeyMouseInputObtainer : IInputObtainer
    {
        private readonly string[] _axesNames = { "Mouse X", "Mouse Y", "Mouse ScrollWheel" };

        private ButtonState[] _buttonStates;
        private float[] _axisStates;

        private BindLayout _curLayout;

        public BindLayout CurLayout
        {
            get { return _curLayout; }
        }

        public KeyMouseInputObtainer(BindLayout bindLayout)
        {
            CheckAndThrow(bindLayout);

            _curLayout = bindLayout;
            _buttonStates = new ButtonState[bindLayout.Keys.Length];
            _axisStates = new float[InputEnum.KMAxisCodeCount];
        }

        public KeyMouseInputObtainer(LayoutConfig config) : this(config.ToBindLayout()) { }

        public void ChangeLayout(BindLayout bindLayout)
        {
            CheckAndThrow(bindLayout);

            Reset();
            _curLayout.RemoveTmpButton();
            _curLayout = bindLayout;
        }

        public void ChangeLayout(LayoutConfig config)
        {
            ChangeLayout(config.ToBindLayout());
        }

        public ButtonState GetKeyState(int keyAction)
        {
            return _buttonStates[keyAction];
        }

        public ButtonInfo GetButtonInfo(int keyAction)
        {
            return new ButtonInfo
            {
                Function = keyAction,
                KeyCode = _curLayout.Keys[keyAction]
            };
        }

        public void AddTmpButton(ButtonInfo info)
        {
            _curLayout.AddTmpButton(info.Function, info.KeyCode);
        }

        public float GetAxisValue(int axisAction)
        {
            int axis = _curLayout.Axes[axisAction];
            return axis >= 0 ? _axisStates[axis] : 0f;
        }

        public void Refresh()
        {
            UpdateAxisStates();
            UpdateBtnStates();
            UpdateKeyAxes();
        }

        public void Reset()
        {
            _buttonStates.Clear();
            _axisStates.Clear();
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckAndThrow(BindLayout bindLayout)
        {
            if (bindLayout.InputType != InputType.KeyMouse)
                throw new ArgumentException("Given layout is not for keyboard+mouse input type.");
        }

        private void UpdateAxisStates()
        {
            for (int i = 0; i < 3; i++)
            {
                _axisStates[i] = Input.GetAxis(_axesNames[i]);
            }
        }

        private void UpdateBtnStates()
        {
            KeyCode keyCode;
            for (int i = 0; i < _buttonStates.Length; i++)
            {
                keyCode = (KeyCode)_curLayout.Keys[i];

                if (keyCode < KeyCode.JoystickButton0)
                {
                    if (Input.GetKeyDown(keyCode)) { _buttonStates[i] = ButtonState.Down; }
                    else if (Input.GetKey(keyCode)) { _buttonStates[i] = ButtonState.Stay; }
                    else if (Input.GetKeyUp(keyCode)) { _buttonStates[i] = ButtonState.Up; }
                    else { _buttonStates[i] = ButtonState.None; }
                }
                else
                {
                    float val = _axisStates[(int)KMAxisCode.Wheel];
                    bool condition = keyCode == KeyCode.JoystickButton0 ? val > 0f : val < 0f;
                    _buttonStates[i] = condition ? ButtonState.Down : ButtonState.None;
                }
            }
        }

        private void UpdateKeyAxes()
        {
            Vector2 cross = new Vector2();

            KeyAxes keyAxes = _curLayout.KeyAxes;

            if (Input.GetKey((KeyCode)_curLayout.Keys[keyAxes.Left]))
                --cross.x;
            if (Input.GetKey((KeyCode)_curLayout.Keys[keyAxes.Right]))
                ++cross.x;
            if (Input.GetKey((KeyCode)_curLayout.Keys[keyAxes.Down]))
                --cross.y;
            if (Input.GetKey((KeyCode)_curLayout.Keys[keyAxes.Up]))
                ++cross.y;

            cross.Normalize();

            _axisStates[(int)KMAxisCode.Horizontal] = cross.x;
            _axisStates[(int)KMAxisCode.Vertical] = cross.y;
        }
    }
}
