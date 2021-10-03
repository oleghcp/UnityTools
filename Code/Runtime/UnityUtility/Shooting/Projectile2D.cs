using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtility.MathExt;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.Shooting
{
    [DisallowMultipleComponent]
    public sealed class Projectile2D : MonoBehaviour
    {
        [SerializeField]
        private bool _playOnAwake;
        [SerializeField, Min(0f)]
        private float _lifeTime = float.PositiveInfinity;
        [SerializeField]
        private bool _autodestruct;
        [SerializeField]
        private bool _autoFlippingX = true;
        [SerializeField]
        private ProjectileMover _moving;
        [SerializeField]
        private ProjectileCaster _casting;
#if UNITY_EDITOR
        [SerializeReference, InitToggle]
        private Debugger _debugging;
#endif
        [SerializeReference, InitToggle]
        private ProjectileEvents2D _events;

        private ITimeProvider _timeProvider;
        private IGravityProvider2D _gravityProvider;
        private IEventListener2D _listener;

        private bool _canMove;
        private int _ricochetsLeft;
        private Vector2 _prevPos;
        private Vector2 _velocity;
        private RaycastHit2D _hitInfo;

        public Vector2 PrevPos => _prevPos;
        public int RicochetsLeft => _ricochetsLeft;

        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public bool Autodestruct
        {
            get => _autodestruct;
            set => _autodestruct = value;
        }

        public ProjectileCaster Caster
        {
            get => _casting;
            set => _casting = value;
        }

        public ProjectileMover Mover
        {
            get => _moving;
            set => _moving = value;
        }

        public ProjectileEvents2D Events { get => _events; set => _events = value; }
        public ITimeProvider TimeProvider { get => _timeProvider; set => _timeProvider = value; }
        public IGravityProvider2D GravityProvider { get => _gravityProvider; set => _gravityProvider = value; }
        public IEventListener2D Listener { get => _listener; set => _listener = value; }

        private void Start()
        {
            if (_playOnAwake)
                Play();
        }

        private void Update()
        {
            if (!_canMove)
                return;

            Vector2 curPos = transform.position;
            UpdateState(_prevPos, curPos, out _prevPos, out curPos);

            if (_canMove)
            {
                Vector2 newPos = _moving.GetNextPos(curPos, ref _velocity, GetGravity(), GetDeltaTime(), 1f);
                UpdateState(curPos, newPos, out _prevPos, out newPos);
                curPos = newPos;
            }

            transform.SetPositionAndRotation(curPos.To_XYz(transform.position.z), GetRotation());

            if (!_canMove)
                InvokeHit();

#if UNITY_EDITOR
            _debugging?.Draw(_prevPos.To_XYz(transform.position.z), transform.position);
#endif
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (GetComponent<Projectile>() != null)
            {
                DestroyImmediate(this);
                return;
            }

            _moving.SpeedRemainder = 1f;
            _casting.HitMask = LayerMask.GetMask("Default");
            _casting.ReflectedCastNear = 0.1f;
        }
#endif

        public async void Play()
        {
            if (_canMove)
                return;

            _canMove = true;
            _ricochetsLeft = _moving.Ricochets;
            _velocity = transform.right * _moving.StartSpeed;

            Vector2 newPos = _moving.GetNextPos(_prevPos = transform.position, ref _velocity, GetGravity(), GetDeltaTime(), 0.5f);
            UpdateState(_prevPos, newPos, out _prevPos, out newPos);

            transform.SetPositionAndRotation(newPos.To_XYz(transform.position.z), GetRotation());

            if (!_canMove)
            {
                InvokeHit();
                return;
            }

            if (_lifeTime.IsPosInfinity())
                return;

            await Task.Delay((_lifeTime * 1000).ToInt());

            _canMove = false;

            if (_autodestruct)
                gameObject.Destroy();

            _events?.OnTimeOut.Invoke();
            _listener?.OnTimeOut();
        }

        public void Stop()
        {
            _canMove = false;
        }

        private void UpdateState(in Vector2 source, in Vector2 dest, out Vector2 newSource, out Vector2 newDest)
        {
            Vector2 vector = dest - source;
            float magnitude = vector.magnitude;

            if (magnitude > Vector2.kEpsilon)
            {
                Vector2 direction = vector / magnitude;

                if (_casting.Cast(source, direction, magnitude, out _hitInfo))
                {
                    if (_ricochetsLeft != 0 && _moving.RicochetMask.HasLayer(_hitInfo.GetLayer()))
                    {
                        _ricochetsLeft--;

                        var reflectionInfo = _moving.Reflect(_hitInfo, dest, direction);
                        _velocity = reflectionInfo.newDir * (_velocity.magnitude * _moving.SpeedRemainder);

                        _events?.OnReflect.Invoke(_hitInfo);
                        _listener?.OnReflect(_hitInfo);

                        float near = _casting.ReflectedCastNear;
                        Vector2 from = near == 0f ? _hitInfo.point
                                                  : Vector2.LerpUnclamped(_hitInfo.point, reflectionInfo.newDest, near);

                        UpdateState(from, reflectionInfo.newDest, out newSource, out newDest);
                        return;
                    }

                    _canMove = false;
                    newSource = source;
                    newDest = _hitInfo.point;

                    return;
                }
            }

            newSource = source;
            newDest = dest;
        }

        private Quaternion GetRotation()
        {
            Vector2 right = _velocity.magnitude > Vector2.kEpsilon ? _velocity
                                                                   : transform.right.XY();

            Vector3 forward = _autoFlippingX ? new Vector3(0f, 0f, _velocity.x.Sign())
                                             : Vector3.forward;

            return Quaternion.LookRotation(forward, right.GetRotated(90f * forward.z));
        }

        private void InvokeHit()
        {
            if (_autodestruct)
                gameObject.Destroy();

            _events?.OnHit.Invoke(_hitInfo);
            _listener?.OnHit(_hitInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetDeltaTime()
        {
            return _timeProvider != null ? _timeProvider.GetDeltaTime()
                                         : Time.deltaTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2 GetGravity()
        {
            return _gravityProvider != null ? _gravityProvider.GetGravity()
                                            : Physics2D.gravity;
        }
    }
}
#endif
