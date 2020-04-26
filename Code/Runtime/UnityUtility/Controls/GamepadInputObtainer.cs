using System;
using UnityEngine;
using UnityUtility.Controls.ControlStuff;
using System.Collections.Generic;
using UnityUtility.MathExt;

namespace UnityUtility.Controls
{
    public sealed class GamepadInputObtainer : IInputObtainer
    {
        private GamepadInputConverter m_converter;
        private Func<GPAxisCode, float, float> m_correctAxis;

        private BindLayout m_curLayout;

        private ButtonState[] m_buttonStates;
        private float[] m_axisStates;

        public BindLayout CurLayout
        {
            get { return m_curLayout; }
        }

        public GamepadInputObtainer(GamepadType type, int num, BindLayout bindLayout)
        {
            f_throw(bindLayout);

            m_curLayout = bindLayout;

            m_correctAxis = AxesCorrection.GetAxisCorrectionFunc(type);
            m_converter = new GamepadInputConverter(type, num);

            m_buttonStates = new ButtonState[InputEnum.GPKeyCodeCount];
            m_axisStates = new float[InputEnum.GPAxisCodeCount];
        }

        public GamepadInputObtainer(GamepadType type, int num, LayoutConfig config) : this(type, num, config.ToBindLayout()) { }

        // - Public Funcs - //

        public void ChangeType(GamepadType type, int num)
        {
            Reset();
            m_correctAxis = AxesCorrection.GetAxisCorrectionFunc(type);
            m_converter = new GamepadInputConverter(type, num);
        }

        public void ChangeLayout(BindLayout bindLayout)
        {
            f_throw(bindLayout);

            Reset();
            m_curLayout.RemoveTmpButton();
            m_curLayout = bindLayout;
        }

        public void ChangeLayout(LayoutConfig config)
        {
            ChangeLayout(config.ToBindLayout());
        }

        public ButtonState GetKeyState(int keyAction)
        {
            int key = m_curLayout.Keys[keyAction];
            return key >= 0 ? m_buttonStates[key] : ButtonState.None;
        }

        public ButtonInfo GetButtonInfo(int keyAction)
        {
            return new ButtonInfo
            {
                Function = keyAction,
                KeyCode = m_curLayout.Keys[keyAction]
            };
        }

        public void AddTmpButton(ButtonInfo info)
        {
            m_curLayout.AddTmpButton(info.Function, info.KeyCode);
        }

        public float GetAxisValue(int axisAction)
        {
            int axis = m_curLayout.Axes[axisAction];
            return axis >= 0 ? m_axisStates[axis] : 0f;
        }

        public void Refresh()
        {
            f_updAxisStates();
            f_updAxisBtnStates();
            f_updBtnStates();
        }

        public void Reset()
        {
            m_buttonStates.Clear();
            m_axisStates.Clear();
        }

        // - Inner Funcs - //

        private static void f_throw(BindLayout bindLayout)
        {
            if (bindLayout.InputType != InputType.Gamepad)
                throw new ArgumentException("Given layout is not for gamepad input type.");
        }

        private void f_updAxisStates()
        {
            float axisValue;

            for (int i = 0; i < InputEnum.GPAxisCodeCount; i++)
            {
                axisValue = Input.GetAxisRaw(m_converter.AxisNames[i]);
                m_axisStates[i] = m_correctAxis((GPAxisCode)i, axisValue);
            }
        }

        private void f_updBtnStates()
        {
            KeyCode keyCode;

            for (int i = 0; i < InputEnum.GPKeyCodeCount; i++)
            {
                keyCode = m_converter.KeyCodes[i];

                if (keyCode != KeyCode.None)
                {
                    if (Input.GetKeyDown(keyCode)) { m_buttonStates[i] = ButtonState.Down; }
                    else if (Input.GetKey(keyCode)) { m_buttonStates[i] = ButtonState.Stay; }
                    else if (Input.GetKeyUp(keyCode)) { m_buttonStates[i] = ButtonState.Up; }
                    else { m_buttonStates[i] = ButtonState.None; }
                }
            }
        }

        private void f_updAxisBtnStates()
        {
            for (int i = 0; i < 4; i++)
            {
                float value = i.IsEven() ? -m_axisStates[i / 2] : m_axisStates[i / 2];
                f_checkAxisBtnState(value, i);
            }

            for (int i = 0; i < 2; i++)
            {
                float value = m_axisStates[i + (int)GPAxisCode.LeftTrgr];
                f_checkAxisBtnState(value, i + (int)GPKeyCode.LeftTrgr);
            }
        }

        private void f_checkAxisBtnState(float axisValue, int btnCode)
        {
            if (axisValue > 0f)
            {
                bool prev = m_buttonStates[btnCode] == ButtonState.Up || m_buttonStates[btnCode] == ButtonState.None;
                m_buttonStates[btnCode] = prev ? ButtonState.Down : ButtonState.Stay;
            }
            else
            {
                bool prev = m_buttonStates[btnCode] == ButtonState.Down || m_buttonStates[btnCode] == ButtonState.Stay;
                m_buttonStates[btnCode] = prev ? ButtonState.Up : ButtonState.None;
            }
        }
    }
}
