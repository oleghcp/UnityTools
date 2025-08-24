#if INCLUDE_PHYSICS_2D
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace OlegHcp.Shooting
{
    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(OlegHcp) + "/Projectile2D")]
    public sealed class Projectile2D : MonoBehaviour, IProjectile
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
        private ProjectileMover2D _moving;
        [SerializeField, FormerlySerializedAs("_ricochets")]
        private HitOptions[] _hitOptions;

        private ProjectileRunner _performer;
        private IRotationProvider _rotationProvider;
        private ITimeProvider _timeProvider;
        private IGravityProvider2D _gravityProvider;
        private IProjectile2DEventListener _listener;
        private HashSet<Component> _penetratedHits;
        private bool _isPlaying;
        private float _currentTime;
        private Vector2 _prevPos;
        private Vector2 _currentPosition;
        private Vector2 _prevVelocity;
        private float _prevSpeed;
        private Vector2 _velocity;
        private float _speed;
        private RaycastHit2D _hitInfo;

        public bool IsPlaying => _isPlaying;
        public float Speed => _speed;
        public Vector2 PrevPos => _prevPos;
        public HitOptions[] HitOptions => _hitOptions;

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

        public LayerMask HitMask
        {
            get => _casting.HitMask;
            set => _casting.HitMask = value;
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

        public IGravityProvider2D GravityProvider
        {
            get => _gravityProvider;
            set => _gravityProvider = value;
        }

        public IProjectile2DEventListener Listener
        {
            get => _listener;
            set => _listener = value;
        }

        private void Awake()
        {
            _performer = ProjectileRunner.I;

            if (_playOnAwake)
                Play();
        }

        private void OnEnable()
        {
            _performer.Add(this);
        }

        private void OnDisable()
        {
            _performer.Remove(this);
            _performer.ReleaseSet(ref _penetratedHits);
        }

#if UNITY_EDITOR
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
        }
#endif

        void IProjectile.OnUpdate()
        {
            _listener?.PreUpdate(_isPlaying);

            if (_isPlaying)
            {
                if (_currentTime >= _timer)
                {
                    if (_autodestruct)
                        gameObject.Destroy();

                    StopInternal();
                    _listener?.OnTimeOut();
                }
                else
                {
                    float deltaTime = GetDeltaTime();
                    _currentTime += deltaTime;
                    UpdateState(deltaTime, 1f);
                }
            }

            _listener?.PostUpdate(_isPlaying);
        }

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

            Vector2 direction = velocity.GetNormalized(out float magnitude);
            _moving.StartSpeed = magnitude;

            if (magnitude <= MathUtility.kEpsilon)
                direction = transform.right;
            else if (_moving.MoveInInitialFrame == 0f)
                transform.right = direction;

            PlayInternal(direction);
        }

        public void Stop()
        {
            if (_isPlaying)
            {
                if (_autodestruct)
                    gameObject.Destroy();

                StopInternal();
            }
        }

        public void SetHitOptionCount(int count)
        {
            ProjectileHelper.SetHitOptionCount(ref _hitOptions, count);
        }

        public void AddHitOption(in HitOptions options)
        {
            ProjectileHelper.AddHitOption(ref _hitOptions, options);
        }

        public void RemoveHitOptionAt(int index)
        {
            ProjectileHelper.RemoveHitOption(ref _hitOptions, index);
        }

        private void PlayInternal(in Vector2 currentDirection)
        {
            _isPlaying = true;

            for (int i = 0; i < _hitOptions.Length; i++)
            {
                _hitOptions[i].Reset();
            }

            _currentPosition = transform.position;

            _prevPos = _currentPosition - (currentDirection * _casting.InitialPrecastBackOffset);
            _speed = _moving.StartSpeed;
            _velocity = currentDirection * _speed;

            if (_moving.MoveInInitialFrame > 0f)
                UpdateState(GetDeltaTime(), _moving.MoveInInitialFrame);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StopInternal()
        {
            _isPlaying = false;
            _currentTime = 0f;
            _speed = 0f;
            _velocity = default;
            _penetratedHits?.Clear();
        }

        private void UpdateState(float deltaTime, float speedScale)
        {
            if (_doubleCollisionCheck)
            {
                if (!ProcessMovement(_prevPos, _currentPosition, true))
                {
                    ApplyMovement();
                    InvokeHit();
                    return;
                }
                _penetratedHits.CleanUp2D(true);
            }

            UpdatePrevSpeed();
            _prevPos = _currentPosition;
            _currentPosition = _moving.GetNextPos(_currentPosition, ref _velocity, GetGravity(), deltaTime, speedScale);
            _speed = _velocity.magnitude;
            bool canPlay = ProcessMovement(_prevPos, _currentPosition, false);
            _penetratedHits.CleanUp2D(!_doubleCollisionCheck);
            ApplyMovement();

            if (!canPlay)
            {
                InvokeHit();
            }
        }

        private bool ProcessMovement(in Vector2 source, in Vector2 destination, bool additional)
        {
            Vector2 direction = (destination - source).GetNormalized(out float magnitude);

            if (magnitude <= MathUtility.kEpsilon || !_casting.Cast(source, direction, magnitude, out _hitInfo))
                return true;

            for (int i = 0; i < _hitOptions.Length; i++)
            {
                ref HitOptions hitOptionRef = ref _hitOptions[i];

                if (!hitOptionRef.HasLayer(_hitInfo.GetLayer()))
                    continue;

                switch (hitOptionRef.Reaction)
                {
                    case HitReactionType.Ricochet:
                    {
                        if (hitOptionRef.Left <= 0)
                            goto ExitLabel;

                        _penetratedHits.CleanUp2D(true);

                        hitOptionRef.UpdateHit();
                        var (newDest, newDir, hitPos) = _moving.Reflect(_hitInfo, destination, direction, _casting.CastRadius, hitOptionRef.SpeedMultiplier);

                        UpdatePrevSpeed();
                        _speed *= hitOptionRef.SpeedMultiplier;
                        _velocity = newDir * _speed;

                        _listener?.OnHitModified(_hitInfo, _prevSpeed, direction, hitOptionRef.Reaction);
                        return ProcessMovement(_prevPos = hitPos, _currentPosition = newDest, additional);
                    }

                    case HitReactionType.MoveThrough:
                    {
                        _hitInfo.collider.enabled = false;

                        if (additional)
                        {
                            if (_penetratedHits.Has(_hitInfo.collider))
                                return ProcessMovement(source, destination, additional);
                        }
                        else
                        {
                            if (_penetratedHits == null)
                                _penetratedHits = _performer.GetSet();
                            _penetratedHits.Add(_hitInfo.collider);
                        }

                        if (hitOptionRef.Left <= 0)
                            goto ExitLabel;

                        hitOptionRef.UpdateHit();

                        Vector2 newDestination = _moving.Penetrate(_hitInfo, destination, direction, _casting.CastRadius, hitOptionRef.SpeedMultiplier);

                        UpdatePrevSpeed();
                        _speed *= hitOptionRef.SpeedMultiplier;
                        _velocity = direction * _speed;

                        _listener?.OnHitModified(_hitInfo, _prevSpeed, direction, hitOptionRef.Reaction);

                        return ProcessMovement(source, _currentPosition = newDestination, additional);
                    }
                }
            }

        ExitLabel:
            _currentPosition = _moving.GetHitPosition(_hitInfo, _casting.CastRadius);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePrevSpeed()
        {
            _prevVelocity = _velocity;
            _prevSpeed = _speed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InvokeHit()
        {
            if (_autodestruct)
                gameObject.Destroy();

            StopInternal();
            _listener?.OnHitFinal(_hitInfo, _prevVelocity, _prevSpeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyMovement()
        {
            Vector3 newPosition = _currentPosition.To_XYz(transform.position.z);

            if (_rotationProvider != null)
            {
                transform.SetPositionAndRotation(newPosition, _rotationProvider.GetRotation());
            }
            else if (_speed > MathUtility.kEpsilon)
            {
                Vector3 forward = new Vector3(0f, 0f, _velocity.x.Sign());
                Quaternion rotation = forward.ToLookRotation(_velocity.GetRotated(90f * forward.z));
                transform.SetPositionAndRotation(newPosition, rotation);
            }
            else
            {
                transform.position = newPosition;
            }
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
