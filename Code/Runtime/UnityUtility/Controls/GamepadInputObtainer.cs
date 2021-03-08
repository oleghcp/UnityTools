using System;
using UnityEngine;
using UnityUtility.Controls.ControlStuff;
using System.Collections.Generic;
using UnityUtility.MathExt;
using System.Runtime.CompilerServices;

namespace UnityUtility.Controls
{
    public sealed class GamepadInputObtainer : IInputObtainer
    {
        private GamepadInputConverter _converter;
        private Func<GPAxisCode, float, float> _correctAxis;

        private BindLayout _curLayout;

        private ButtonState[] _buttonStates;
        private float[] _axisStates;

        public BindLayout CurLayout
        {
            get { return _curLayout; }
        }

        public GamepadInputObtainer(GamepadType type, int num, BindLayout bindLayout)
        {
            CheckAndThrow(bindLayout);

            _curLayout = bindLayout;

            _correctAxis = AxesCorrection.GetAxisCorrectionFunc(type);
            _converter = new GamepadInputConverter(type, num);

            _buttonStates = new ButtonState[InputEnum.GPKeyCodeCount];
            _axisStates = new float[InputEnum.GPAxisCodeCount];
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckAndThrow(BindLayout bindLayout)
        {
            if (bindLayout.InputType != InputType.Gamepad)
                throw new ArgumentException("Given layout is not for gamepad input type.");
        }

        private void UpdAxisStates()
        {
            float axisValue;

            for (int i = 0; i < InputEnum.GPAxisCodeCount; i++)
            {
                axisValue = Input.GetAxisRaw(_converter.AxisNames[i]);
                _axisStates[i] = _correctAxis((GPAxisCode)i, axisValue);
            }
        }

        private void UpdBtnStates()
        {
            KeyCode keyCode;

            for (int i = 0; i < InputEnum.GPKeyCodeCount; i++)
            {
                keyCode = _converter.KeyCodes[i];

                if (keyCode != KeyCode.None)
                {
                    if (Input.GetKeyDown(keyCode)) { _buttonStates[i] = ButtonState.Down; }
                    else if (Input.GetKey(keyCode)) { _buttonStates[i] = ButtonState.Stay; }
                    else if (Input.GetKeyUp(keyCode)) { _buttonStates[i] = ButtonState.Up; }
                    else { _buttonStates[i] = ButtonState.None; }
                }
            }
        }

        private void UpdAxisBtnStates()
        {
            for (int i = 0; i < 4; i++)
            {
                float value = i.IsEven() ? -_axisStates[i / 2] : _axisStates[i / 2];
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
                _buttonStates[btnCode] = prev ? ButtonState.Down : ButtonState.Stay;
            }
            else
            {
                bool prev = _buttonStates[btnCode] == ButtonState.Down || _buttonStates[btnCode] == ButtonState.Stay;
                _buttonStates[btnCode] = prev ? ButtonState.Up : ButtonState.None;
            }
        }
    }
}
