using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityUtility.Controls
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ScreenTouch
    {
        public Vector2 Position;
        public Vector2 PrevPosition;
        public Vector2 DeltaPosition;
        public float LastTime;
        public float DeltaTime;
        public int FingerId;
        public int TapCount;
        public TouchPhase Phase;
    }

    public static class ScreenTouchExtensions
    {
        public static ScreenTouch ToEmulTouch(this in Touch touch)
        {
            return new ScreenTouch()
            {
                Position = touch.position,
                PrevPosition = touch.position - touch.deltaPosition,
                DeltaPosition = touch.deltaPosition,
                DeltaTime = touch.deltaTime,
                LastTime = Time.time - touch.deltaTime,
                Phase = touch.phase,
                FingerId = touch.fingerId,
                TapCount = touch.tapCount,
            };
        }

        public static ScreenTouch ToEmulTouch(this in Touch touch, in Vector2 prevPos)
        {
            return new ScreenTouch()
            {
                Position = touch.position,
                PrevPosition = prevPos,
                DeltaPosition = touch.position - prevPos,
                DeltaTime = touch.deltaTime,
                LastTime = Time.time - touch.deltaTime,
                Phase = touch.phase,
                FingerId = touch.fingerId,
                TapCount = touch.tapCount,
            };
        }
    }

    public sealed class TouchInputEmulator : IRefreshable
    {
        private ScreenTouch[] _touches;
        private int _touchCount;

        public ScreenTouch[] Touches => _touches;
        public int TouchCount => _touchCount;

        public TouchInputEmulator()
        {
            _touches = new[]
            {
                new ScreenTouch
                {
                    FingerId = 0,
                    Position = Input.mousePosition,
                    PrevPosition = Input.mousePosition,
                    DeltaPosition = new Vector2(0f, 0f),
                    LastTime = Time.time,
                    DeltaTime = 0f,
                    TapCount = 0,
                    Phase = TouchPhase.Canceled,
                }
            };
        }

        public void ResetTouch()
        {
            ref ScreenTouch touch = ref _touches[0];

            touch.DeltaTime = Time.time - _touches[0].LastTime;
            touch.LastTime = Time.time;
            touch.Phase = TouchPhase.Canceled;

            _touchCount = 0;
        }

        public void Refresh()
        {
            Vector2 curMousePos = Input.mousePosition;
            ref ScreenTouch touch = ref _touches[0];

            switch (touch.Phase)
            {
                case TouchPhase.Canceled:
                    if (Input.GetMouseButtonDown(0) == true)
                    {
                        touch.FingerId = 0;
                        touch.Position = curMousePos;
                        touch.PrevPosition = curMousePos;
                        touch.DeltaPosition = new Vector2(0, 0);
                        touch.DeltaTime = 0f;
                        touch.LastTime = Time.time;
                        touch.TapCount = 0;
                        touch.Phase = TouchPhase.Began;
                        _touchCount = 1;
                    }
                    break;

                case TouchPhase.Began:
                    touch.DeltaTime = Time.time - touch.LastTime;
                    touch.LastTime = Time.time;
                    touch.Phase = TouchPhase.Stationary;
                    break;

                case TouchPhase.Stationary:
                    if (touch.Position != curMousePos)
                    {
                        touch.DeltaPosition = curMousePos - touch.Position;
                        touch.PrevPosition = touch.Position;
                        touch.Position = curMousePos;
                        touch.DeltaTime = Time.time - touch.LastTime;
                        touch.LastTime = Time.time;
                        touch.Phase = TouchPhase.Moved;
                    }

                    if (Input.GetMouseButtonUp(0) == true)
                    {
                        touch.DeltaTime = Time.time - touch.LastTime;
                        touch.LastTime = Time.time;
                        touch.Phase = TouchPhase.Ended;
                    }
                    break;

                case TouchPhase.Moved:
                    if (touch.Position != curMousePos)
                    {
                        touch.DeltaPosition = curMousePos - touch.Position;
                        touch.PrevPosition = touch.Position;
                        touch.Position = curMousePos;
                        touch.DeltaTime = Time.time - touch.LastTime;
                        touch.LastTime = Time.time;
                    }
                    else
                    {
                        touch.PrevPosition = touch.Position;
                        touch.DeltaPosition = new Vector2(0, 0);
                        touch.DeltaTime = Time.time - touch.LastTime;
                        touch.LastTime = Time.time;
                        touch.Phase = TouchPhase.Stationary;
                    }

                    if (Input.GetMouseButtonUp(0) == true)
                    {
                        touch.DeltaTime = Time.time - touch.LastTime;
                        touch.LastTime = Time.time;
                        touch.Phase = TouchPhase.Ended;
                    }
                    break;

                case TouchPhase.Ended:
                    touch.DeltaTime = Time.time - touch.LastTime;
                    touch.LastTime = Time.time;
                    touch.Phase = TouchPhase.Canceled;
                    _touchCount = 0;
                    break;
            }
        }
    }
}
