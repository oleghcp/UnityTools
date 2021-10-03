using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtility.MathExt;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.Shooting
{
    [DisallowMultipleComponent]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField]
        private bool _playOnAwake;
        [SerializeField, Min(0f)]
        private float _lifeTime = float.PositiveInfinity;
        [SerializeField]
        private bool _autodestruct;
        [SerializeField]
        private ProjectileMover _moving;
        [SerializeField]
        private ProjectileCaster _casting;
#if UNITY_EDITOR
        [SerializeReference, InitToggle]
        private Debugger _debugging;
#endif
        [SerializeReference, InitToggle]
        private ProjectileEvents _events;

        private ITimeProvider _timeProvider;
        private IGravityProvider _gravityProvider;
        private IEventListener _listener;

        private bool _canMove;
        private int _ricochetsLeft;
        private Vector3 _prevPos;
        private Vector3 _velocity;
        private RaycastHit _hitInfo;

        public Vector3 PrevPos => _prevPos;
        public int RicochetsLeft => _ricochetsLeft;

        public Vector3 Velocity
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

        public ProjectileEvents Events => _events;
        public ITimeProvider TimeProvider { get => _timeProvider; set => _timeProvider = value; }
        public IGravityProvider GravityProvider { get => _gravityProvider; set => _gravityProvider = value; }
        public IEventListener Listener { get => _listener; set => _listener = value; }

        private void Start()
        {
            if (_playOnAwake)
                Play();
        }

        private void Update()
        {
            if (!_canMove)
                return;

            Vector3 curPos = transform.position;
            UpdateState(_prevPos, curPos, out _prevPos, out curPos);

            if (_canMove)
            {
                Vector3 newPos = _moving.GetNextPos(curPos, ref _velocity, GetGravity(), GetDeltaTime(), 1f);
                UpdateState(curPos, newPos, out _prevPos, out newPos);
                curPos = newPos;
            }

            Quaternion curRot = Quaternion.LookRotation(UpdateDirection());
            transform.SetPositionAndRotation(curPos, curRot);

            if (!_canMove)
                InvokeHit();

#if UNITY_EDITOR
            _debugging?.Draw(_prevPos, curPos);
#endif
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (GetComponent<Projectile2D>() != null)
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
            _velocity = transform.forward * _moving.StartSpeed;

            Vector3 newPos = _moving.GetNextPos(_prevPos = transform.position, ref _velocity, GetGravity(), GetDeltaTime(), 0.5f);
            UpdateState(_prevPos, newPos, out _prevPos, out newPos);
            Quaternion newRot = Quaternion.LookRotation(UpdateDirection());

            transform.SetPositionAndRotation(newPos, newRot);

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

        private void UpdateState(in Vector3 source, in Vector3 dest, out Vector3 newSource, out Vector3 newDest)
        {
            Vector3 vector = dest - source;
            float magnitude = vector.magnitude;

            if (magnitude > Vector2.kEpsilon)
            {
                Vector3 direction = vector / magnitude;

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
                        Vector3 from = near == 0f ? _hitInfo.point
                                                  : Vector3.LerpUnclamped(_hitInfo.point, reflectionInfo.newDest, near);

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

        private void InvokeHit()
        {
            if (_autodestruct)
                gameObject.Destroy();

            _events?.OnHit.Invoke(_hitInfo);
            _listener?.OnHit(_hitInfo);
        }

        private Vector3 UpdateDirection()
        {
            float length = _velocity.magnitude;

            if (length > Vector3.kEpsilon)
                return _velocity / length;

            return transform.forward;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetDeltaTime()
        {
            return _timeProvider != null ? _timeProvider.GetDeltaTime()
                                         : Time.deltaTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetGravity()
        {
            return _gravityProvider != null ? _gravityProvider.GetGravity()
                                            : Physics.gravity;
        }
    }
}
#endif
