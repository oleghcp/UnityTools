#if INCLUDE_PHYSICS
using System;
using System.Runtime.CompilerServices;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp.Shooting
{
    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(OlegHcp) + "/Projectile")]
    public sealed class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField]
        private bool _playOnAwake;
        [SerializeField, Min(0f)]
        private float _timer = float.PositiveInfinity;
        [SerializeField]
        private bool _autodestruct;
        [SerializeField]
        private bool _doubleCollisionCheck = true;
        [SerializeField]
        private ProjectileCaster _casting;
        [SerializeField]
        private ProjectileMover _moving;
        [SerializeField]
        private RicochetOptions[] _ricochets;

        private IRotationProvider _rotationProvider;
        private ITimeProvider _timeProvider;
        private IGravityProvider _gravityProvider;
        private IProjectileEventListener _listener;

        private bool _isPlaying;
        private float _currentTime;
        private Vector3 _prevPos;
        private Vector3 _prevVelocity;
        private float _prevSpeed;
        private Vector3 _velocity;
        private float _speed;
        private RaycastHit _hitInfo;

        public bool IsPlaying => _isPlaying;
        public float Speed => _speed;
        public Vector3 PrevPos => _prevPos;
        public RicochetOptions[] Ricochets => _ricochets;

        public float Timer
        {
            get => _timer;
            set => _timer = value;
        }

        public Vector3 Velocity
        {
            get => _velocity;
            set
            {
                if (!_isPlaying)
                    throw new InvalidOperationException("Projectile is stopped.");

                _velocity = value;
                _moving.LockVelocity(ref _velocity);
                _speed = _velocity.magnitude;
            }
        }

        public bool Autodestruct
        {
            get => _autodestruct;
            set => _autodestruct = value;
        }

        public bool DoubleCollisionCheck
        {
            get => _doubleCollisionCheck;
            set => _doubleCollisionCheck = value;
        }

        public bool UseGravity
        {
            get => _moving.UseGravity;
            set => _moving.UseGravity = value;
        }

        public float StartSpeed
        {
            get => _moving.StartSpeed;
            set => _moving.StartSpeed = value;
        }

        public bool FreezeX
        {
            get => _moving.FreezePosition[0];
            set => _moving.LockAxis(0, value);
        }

        public bool FreezeY
        {
            get => _moving.FreezePosition[1];
            set => _moving.LockAxis(1, value);
        }

        public bool FreezeZ
        {
            get => _moving.FreezePosition[2];
            set => _moving.LockAxis(2, value);
        }

        public float MoveInInitialFrame
        {
            get => _moving.MoveInInitialFrame;
            set => _moving.MoveInInitialFrame = value;
        }

        public DragMethod DragMethod
        {
            get => _moving.DragMethod;
            set => _moving.DragMethod = value;
        }

        public float DragValue
        {
            get => _moving.Drag;
            set => _moving.Drag = value;
        }

        public float CastRadius
        {
            get => _casting.CastRadius;
            set => _casting.CastRadius = value;
        }

        public bool HighPrecision
        {
            get => _casting.HighPrecision;
            set => _casting.HighPrecision = value;
        }

        public LayerMask HitMask
        {
            get => _casting.HitMask;
            set => _casting.HitMask = value;
        }

        public float ReflectedCastNear
        {
            get => _casting.ReflectedCastNear;
            set => _casting.ReflectedCastNear = value;
        }

        public float InitialPrecastBackOffset
        {
            get => _casting.InitialPrecastBackOffset;
            set => _casting.InitialPrecastBackOffset = value;
        }

        public IRotationProvider RotationProvider
        {
            get => _rotationProvider;
            set => _rotationProvider = value;
        }

        public ITimeProvider TimeProvider
        {
            get => _timeProvider;
            set => _timeProvider = value;
        }

        public IGravityProvider GravityProvider
        {
            get => _gravityProvider;
            set => _gravityProvider = value;
        }

        public IProjectileEventListener Listener
        {
            get => _listener;
            set => _listener = value;
        }

        private void Start()
        {
            if (_playOnAwake)
                Play();
        }

        private void OnEnable()
        {
            ProjectileRunner.I.Add(this);
        }

        private void OnDisable()
        {
            ProjectileRunner.I.Remove(this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _casting.InitialPrecastBackOffset = _casting.InitialPrecastBackOffset;
        }

        private void Reset()
        {
#if INCLUDE_PHYSICS_2D
            if (gameObject.TryGetComponent<Projectile2D>(out _))
            {
                DestroyImmediate(this);
                return;
            }
#endif

            _casting.HitMask = LayerMask.GetMask("Default");
            _casting.ReflectedCastNear = 0.1f;
        }
#endif

        void IProjectile.OnTick()
        {
            _listener?.PreUpdate(_isPlaying);

            if (_isPlaying)
            {
                if (_currentTime >= _timer)
                {
                    _isPlaying = false;
                    InvokeTimeOut();
                }
                else
                {
                    float deltaTime = GetDeltaTime();
                    _currentTime += deltaTime;
                    UpdateState(transform.position, deltaTime, 1f);
                }
            }

            _listener?.PostUpdate(_isPlaying);
        }

        public void Play()
        {
            if (_isPlaying)
                return;

            PlayInternal(transform.forward);
        }

        public void Play(float startSpeed)
        {
            if (_isPlaying)
                return;

            _moving.StartSpeed = startSpeed;
            PlayInternal(transform.forward);
        }

        public void Play(in Vector3 velocity)
        {
            if (_isPlaying)
                return;

            Vector3 direction = velocity.GetNormalized(out float magnitude);
            _moving.StartSpeed = magnitude;

            if (magnitude <= MathUtility.kEpsilon)
                direction = transform.forward;
            else if (_moving.MoveInInitialFrame == 0f)
                transform.forward = direction;

            PlayInternal(direction);
        }

        public void Stop()
        {
            _isPlaying = false;
            _currentTime = 0f;
            _speed = 0f;
            _velocity = default;
        }

        public void SetRicochetOptionsCount(int count)
        {
            ProjectileHelper.SetRicochetOptionsCount(ref _ricochets, count);
        }

        public void AddRicochetOptions(in RicochetOptions options)
        {
            ProjectileHelper.AddRicochetOptions(ref _ricochets, options);
        }

        public void RemoveRicochetOptionsAt(int index)
        {
            ProjectileHelper.RemoveRicochetOptionsAt(ref _ricochets, index);
        }

        private void PlayInternal(in Vector3 currentDirection)
        {
            _isPlaying = true;

            for (int i = 0; i < _ricochets.Length; i++)
            {
                _ricochets[i].ResetRicochets();
            }

            Vector3 currentPosition = transform.position;

            _prevPos = currentPosition - (currentDirection * _casting.InitialPrecastBackOffset);
            _velocity = currentDirection * _moving.StartSpeed;

            if (_moving.HasLocks)
            {
                _moving.LockVelocity(ref _velocity);
                _speed = _velocity.magnitude;
            }
            else
            {
                _speed = _moving.StartSpeed;
            }

            if (_moving.MoveInInitialFrame > 0f)
            {
                UpdateState(currentPosition, GetDeltaTime(), _moving.MoveInInitialFrame);
            }
        }

        private void UpdateState(Vector3 currentPosition, float deltaTime, float speedScale)
        {
            if (_doubleCollisionCheck)
            {
                CheckMovement(_prevPos, currentPosition, out _prevPos, out currentPosition);
                if (!_isPlaying)
                {
                    transform.SetPositionAndRotation(currentPosition, GetRotation());
                    InvokeHit();
                    return;
                }
            }

            UpdatePrevState();
            Vector3 newPos = _moving.GetNextPos(currentPosition, ref _velocity, GetGravity(), deltaTime, speedScale);
            _speed = _velocity.magnitude;
            CheckMovement(currentPosition, newPos, out _prevPos, out currentPosition);
            transform.SetPositionAndRotation(currentPosition, GetRotation());

            if (!_isPlaying)
                InvokeHit();
        }

        private void CheckMovement(in Vector3 source, in Vector3 dest, out Vector3 newSource, out Vector3 newDest)
        {
            Vector3 vector = dest - source;
            float magnitude = vector.magnitude;

            if (magnitude > MathUtility.kEpsilon)
            {
                Vector3 direction = vector / magnitude;

                if (_casting.Cast(source, direction, magnitude, out _hitInfo))
                {
                    for (int i = 0; i < _ricochets.Length; i++)
                    {
                        ref RicochetOptions ricochetOption = ref _ricochets[i];

                        if (ricochetOption.RicochetsLeft > 0 && ricochetOption.RicochetMask.HasLayer(_hitInfo.GetLayer()))
                        {
                            ricochetOption.DecreaseCounter();

                            UpdatePrevState();
                            var reflectionInfo = _moving.Reflect(_hitInfo, dest, direction, _casting.CastRadius, ricochetOption.SpeedRemainder);
                            _velocity = reflectionInfo.newDir * (_speed * ricochetOption.SpeedRemainder);
                            _speed = _velocity.magnitude;

                            _listener?.OnHitReflected(_hitInfo, _prevVelocity, _prevSpeed);

                            float near = _casting.ReflectedCastNear;
                            Vector3 from = near == 0f ? _hitInfo.point
                                                      : Vector3.LerpUnclamped(_hitInfo.point, reflectionInfo.newDest, near);

                            CheckMovement(from, reflectionInfo.newDest, out newSource, out newDest);
                            return;
                        }
                    }

                    _isPlaying = false;
                    newSource = source;
                    newDest = _moving.GetHitPosition(_hitInfo, _casting.CastRadius);
                    return;
                }
            }

            newSource = source;
            newDest = dest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePrevState()
        {
            _prevVelocity = _velocity;
            _prevSpeed = _speed;
        }

        private void InvokeHit()
        {
            if (_autodestruct)
                gameObject.Destroy();

            _currentTime = 0f;
            _velocity = default;
            _speed = 0f;

            _listener?.OnHitFinal(_hitInfo, _prevVelocity, _prevSpeed);
        }

        private void InvokeTimeOut()
        {
            _currentTime = 0f;

            if (_autodestruct)
                gameObject.Destroy();

            _listener?.OnTimeOut();
        }

        private Quaternion GetRotation()
        {
            if (_rotationProvider != null)
                return _rotationProvider.GetRotation();

            if (_speed > MathUtility.kEpsilon)
                return (_velocity / _speed).ToLookRotation();

            return transform.forward.ToLookRotation();
        }

        private float GetDeltaTime()
        {
            return _timeProvider != null ? _timeProvider.GetDeltaTime()
                                         : Time.deltaTime;
        }

        private Vector3 GetGravity()
        {
            return _gravityProvider != null ? _gravityProvider.GetGravity()
                                            : Physics.gravity;
        }
    }
}
#endif
