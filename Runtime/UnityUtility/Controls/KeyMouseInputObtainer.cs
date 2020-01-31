using UnityEngine;
using System.Collections.Generic;
using UU.Controls.ControlStuff;
using System;

namespace UU.Controls
{
    public sealed class KeyMouseInputObtainer : InputObtainer
    {
        private readonly string[] AX_NAMES = { "Mouse X", "Mouse Y", "Mouse ScrollWheel" };

        private ButtonState[] m_buttonStates;
        private float[] m_axisStates;

        private BindLayout m_curLayout;

        public BindLayout CurLayout
        {
            get { return m_curLayout; }
        }

        public KeyMouseInputObtainer(BindLayout bindLayout)
        {
            f_throw(bindLayout);

            m_curLayout = bindLayout;
            m_buttonStates = new ButtonState[bindLayout.Keys.Length];
            m_axisStates = new float[InputEnum.KMAxisCodeCount];
        }

        public KeyMouseInputObtainer(LayoutConfig config) : this(config.ToBindLayout()) { }

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
            return m_buttonStates[keyAction];
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
            f_updateBtnStates();
            f_updateKeyAxes();
        }

        public void Reset()
        {
            m_buttonStates.Clear();
            m_axisStates.Clear();
        }

        // -- //

        private static void f_throw(BindLayout bindLayout)
        {
            if (bindLayout.InputType != InputType.KeyMouse)
                throw new ArgumentException("Given layout is not for keyboard+mouse input type.");
        }

        private void f_updAxisStates()
        {
            for (int i = 0; i < 3; i++)
            {
                m_axisStates[i] = Input.GetAxis(AX_NAMES[i]);
            }
        }

        private void f_updateBtnStates()
        {
            KeyCode keyCode;
            for (int i = 0; i < m_buttonStates.Length; i++)
            {
                keyCode = (KeyCode)m_curLayout.Keys[i];

                if (keyCode < KeyCode.JoystickButton0)
                {
                    if (Input.GetKeyDown(keyCode)) { m_buttonStates[i] = ButtonState.Down; }
                    else if (Input.GetKey(keyCode)) { m_buttonStates[i] = ButtonState.Stay; }
                    else if (Input.GetKeyUp(keyCode)) { m_buttonStates[i] = ButtonState.Up; }
                    else { m_buttonStates[i] = ButtonState.None; }
                }
                else
                {
                    float val = m_axisStates[(int)KMAxisCode.Wheel];
                    bool condition = keyCode == KeyCode.JoystickButton0 ? val > 0f : val < 0f;
                    m_buttonStates[i] = condition ? ButtonState.Down : ButtonState.None;
                }
            }
        }

        private void f_updateKeyAxes()
        {
            Vector2 cross = new Vector2();

            KeyAxes keyAxes = m_curLayout.KeyAxes;

            if (Input.GetKey((KeyCode)m_curLayout.Keys[keyAxes.Left]))
                --cross.x;
            if (Input.GetKey((KeyCode)m_curLayout.Keys[keyAxes.Right]))
                ++cross.x;
            if (Input.GetKey((KeyCode)m_curLayout.Keys[keyAxes.Down]))
                --cross.y;
            if (Input.GetKey((KeyCode)m_curLayout.Keys[keyAxes.Up]))
                ++cross.y;

            cross.Normalize();

            m_axisStates[(int)KMAxisCode.Horizontal] = cross.x;
            m_axisStates[(int)KMAxisCode.Vertical] = cross.y;
        }
    }
}
