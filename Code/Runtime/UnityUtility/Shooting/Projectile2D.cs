#if INCLUDE_PHYSICS_2D
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Engine;
using UnityUtility.Mathematics;

namespace UnityUtility.Shooting
{
    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(UnityUtility) + "/Projectile2D")]
    public sealed class Projectile2D : MonoBehaviour
    {
        [SerializeField]
        private bool _playOnAwake;
        [SerializeField, Min(0f)]
        private float _timer = float.PositiveInfinity;
        [SerializeField]
        private bool _autodestruct;
        [SerializeField]
        private bool _autoFlippingX = true;
        [SerializeField]
        private ProjectileCaster _casting;
        [SerializeField]
        private ProjectileMover2D _moving;
        [SerializeField]
        private RicochetOptions[] _ricochets;

        private IRotationProvider _rotationProvider;
        private ITimeProvider _timeProvider;
        private IGravityProvider2D _gravityProvider;
        private IProjectile2DEventListener _listener;

        private bool _isPlaying;
        private float _currentTime;
        private Vector2 _prevPos;
        private Vector2 _prevVelocity;
        private float _prevSpeed;
        private Vector2 _velocity;
        private float _speed;
        private RaycastHit2D _hitInfo;

        public bool IsPlaying => _isPlaying;
        public float Speed => _speed;
        public Vector2 PrevPos => _prevPos;
        public RicochetOptions[] Ricochets => _ricochets;

        public float Timer
        {
            get => _timer;
            set => _timer = value;
        }

        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                if (!_isPlaying)
                    throw new InvalidOperationException("Projectile is stopped.");

                _velocity = value;
                _speed = value.magnitude;
            }
        }

        public bool Autodestruct
        {
            get => _autodestruct;
            set => _autodestruct = value;
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

        public float InitialPrecastOffset
        {
            get => _casting.InitialPrecastOffset;
            set => _casting.InitialPrecastOffset = value;
        }

        public IRotationProvider RotationProvider { get => _rotationProvider; set => _rotationProvider = value; }
        public ITimeProvider TimeProvider { get => _timeProvider; set => _timeProvider = value; }
        public IGravityProvider2D GravityProvider { get => _gravityProvider; set => _gravityProvider = value; }
        public IProjectile2DEventListener Listener { get => _listener; set => _listener = value; }

        private void Start()
        {
            if (_playOnAwake)
                Play();
        }

        private void Update()
        {
            _listener?.PreUpdate();

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

                    if (!_isPlaying)
                        InvokeHit();
                }
            }

            _listener?.PostUpdate();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _casting.InitialPrecastOffset = _casting.InitialPrecastOffset;
        }

        private void Reset()
        {
#if INCLUDE_PHYSICS
            if (gameObject.TryGetComponent<Projectile>(out _))
            {
                DestroyImmediate(this);
                return;
            }
#endif

            _casting.HitMask = LayerMask.GetMask("Default");
            _casting.ReflectedCastNear = 0.1f;
        }
#endif

        public void Play()
        {
            if (_isPlaying)
                return;

            PlayInternal(transform.right);
        }

        public void Play(float startSpeed)
        {
            if (_isPlaying)
                return;

            _moving.StartSpeed = startSpeed;
            PlayInternal(transform.right);
        }

        public void Play(in Vector2 velocity)
        {
            if (_isPlaying)
                return;

            Vector3 direction = velocity.GetNormalized(out float magnitude);
            _moving.StartSpeed = magnitude;

            if (magnitude <= MathUtility.kEpsilon)
                direction = transform.right;
            else if (_moving.MoveInInitialFrame == 0f)
                transform.right = direction;

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

            _prevPos = currentPosition + currentDirection * _casting.InitialPrecastOffset;
            _speed = _moving.StartSpeed;
            _velocity = currentDirection * _speed;

            if (_moving.MoveInInitialFrame > 0f)
            {
                UpdateState(currentPosition, GetDeltaTime(), _moving.MoveInInitialFrame);

                if (!_isPlaying)
                {
                    InvokeHit();
                    return;
                }
            }
        }

        private void UpdateState(Vector2 currentPosition, float deltaTime, float speedScale)
        {
            CheckMovement(_prevPos, currentPosition, out _prevPos, out currentPosition);

            if (_isPlaying)
            {
                UpdatePrevState();
                Vector2 newPos = _moving.GetNextPos(currentPosition, ref _velocity, GetGravity(), deltaTime, speedScale);
                _speed = _velocity.magnitude;
                CheckMovement(currentPosition, newPos, out _prevPos, out currentPosition);
            }

            transform.SetPositionAndRotation(currentPosition.To_XYz(transform.position.z), GetRotation());
        }

        private void CheckMovement(in Vector2 source, in Vector2 dest, out Vector2 newSource, out Vector2 newDest)
        {
            Vector2 vector = dest - source;
            float magnitude = vector.magnitude;

            if (magnitude > MathUtility.kEpsilon)
            {
                Vector2 direction = vector / magnitude;

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

                            _listener?.OnReflect(_hitInfo, _prevVelocity, _prevSpeed);

                            float near = _casting.ReflectedCastNear;
                            Vector2 from = near == 0f ? _hitInfo.point
                                                      : Vector2.LerpUnclamped(_hitInfo.point, reflectionInfo.newDest, near);

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

            _listener?.OnHit(_hitInfo, _prevVelocity, _prevSpeed);
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

            Vector2 right = _speed > MathUtility.kEpsilon ? _velocity : transform.right.XY();
            Vector3 forward = _autoFlippingX ? new Vector3(0f, 0f, _velocity.x.Sign()) : Vector3.forward;
            return forward.ToLookRotation(right.GetRotated(90f * forward.z));
        }

        private float GetDeltaTime()
        {
            return _timeProvider != null ? _timeProvider.GetDeltaTime()
                                         : Time.deltaTime;
        }

        private Vector2 GetGravity()
        {
            return _gravityProvider != null ? _gravityProvider.GetGravity()
                                            : Physics2D.gravity;
        }
    }
}
#endif
