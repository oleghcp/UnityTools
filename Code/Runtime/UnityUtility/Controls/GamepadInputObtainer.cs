#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Controls.ControlStuff;

namespace UnityUtility.Controls
{
    public sealed class GamepadInputObtainer : IInputObtainer
    {
        private GamepadInputConverter _converter;
        private Func<GPAxisCode, float, float> _correctAxis;

        private BindLayout _curLayout;

        private ButtonState[] _buttonStates;
        private float[] _axisStates;

        public BindLayout CurLayout => _curLayout;

        public GamepadInputObtainer(GamepadType type, int num, BindLayout bindLayout)
        {
            CheckAndThrow(bindLayout);

            _curLayout = bindLayout;

            _correctAxis = AxesCorrection.GetAxisCorrectionFunc(type);
            _converter = new GamepadInputConverter(type, num);

            _buttonStates = new ButtonState[InputEnumUtility.GPKeyCodeCount];
            _axisStates = new float[InputEnumUtility.GPAxisCodeCount];
        }

        public GamepadInputObtainer(GamepadType type, int num, LayoutConfig config) : this(type, num, config.ToBindLayout()) { }

        // - Public Funcs - //

        public void ChangeType(GamepadType type, int num)
        {
            Reset();
            _correctAxis = AxesCorrection.GetAxisCorrectionFunc(type);
            _converter = new GamepadInputConverter(type, num);
        }

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
            int key = _curLayout.Keys[keyAction];
            return key >= 0 ? _buttonStates[key] : ButtonState.None;
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
            UpdAxisStates();
            UpdAxisBtnStates();
            UpdBtnStates();
        }

        public void Reset()
        {
            _buttonStates.Clear();
            _axisStates.Clear();
        }

        // - Inner Funcs - //

        private static void CheckAndThrow(BindLayout bindLayout)
        {
            if (bindLayout.InputType != InputType.Gamepad)
                throw new ArgumentException("Given layout is not for gamepad input type.");
        }

        private void UpdAxisStates()
        {
            float axisValue;

            for (int i = 0; i < InputEnumUtility.GPAxisCodeCount; i++)
            {
                axisValue = Input.GetAxisRaw(_converter.AxisNames[i]);
                _axisStates[i] = _correctAxis((GPAxisCode)i, axisValue);
            }
        }

        private void UpdBtnStates()
        {
            KeyCode keyCode;

            for (int i = 0; i < InputEnumUtility.GPKeyCodeCount; i++)
            {
                keyCode = _converter.KeyCodes[i];

                if (keyCode != KeyCode.None)
                {
                    if (Input.GetKeyDown(keyCode)) { _buttonStates[i] = ButtonState.Down; }
                    else if (Input.GetKey(keyCode)) { _buttonStates[i] = ButtonState.Hold; }
                    else if (Input.GetKeyUp(keyCode)) { _buttonStates[i] = ButtonState.Up; }
                    else { _buttonStates[i] = ButtonState.None; }
                }
            }
        }

        private void UpdAxisBtnStates()
        {
            for (int i = 0; i < 4; i++)
            {
                float value = i % 2 == 0 ? -_axisStates[i / 2] : _axisStates[i / 2];
                CheckAxisBtnState(value, i);
            }

            for (int i = 0; i < 2; i++)
            {
                float value = _axisStates[i + (int)GPAxisCode.LeftTrgr];
                CheckAxisBtnState(value, i + (int)GPKeyCode.LeftTrgr);
            }
        }

        private void CheckAxisBtnState(float axisValue, int btnCode)
        {
            if (axisValue > 0f)
            {
                bool prev = _buttonStates[btnCode] == ButtonState.Up || _buttonStates[btnCode] == ButtonState.None;
                _buttonStates[btnCode] = prev ? ButtonState.Down : ButtonState.Hold;
            }
            else
            {
                bool prev = _buttonStates[btnCode] == ButtonState.Down || _buttonStates[btnCode] == ButtonState.Hold;
                _buttonStates[btnCode] = prev ? ButtonState.Up : ButtonState.None;
            }
        }
    }
}
#endif
