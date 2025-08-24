#if INCLUDE_PHYSICS
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
        [SerializeField, FormerlySerializedAs("_ricochets")]
        private HitOptions[] _hitOptions;

        private ProjectileRunner _performer;
        private IRotationProvider _rotationProvider;
        private ITimeProvider _timeProvider;
        private IGravityProvider _gravityProvider;
        private IProjectileEventListener _listener;
        private HashSet<Component> _penetratedHits;
        private bool _isPlaying;
        private float _currentTime;
        private Vector3 _currentPosition;
        private Vector3 _prevPos;
        private Vector3 _prevVelocity;
        private float _prevSpeed;
        private Vector3 _velocity;
        private float _speed;
        private RaycastHit _hitInfo;

        public bool IsPlaying => _isPlaying;
        public float Speed => _speed;
        public Vector3 PrevPos => _prevPos;
        public HitOptions[] HitOptions => _hitOptions;

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

#if UNITY_EDITOR
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
        }
#endif

        private void OnEnable()
        {
            _performer = ProjectileRunner.I;
            _performer.Add(this);
        }

        private void OnDisable()
        {
            _performer.Remove(this);
            _performer.ReleaseSet(ref _penetratedHits);
        }

        private void Start()
        {
            if (_playOnAwake)
                Play();
        }

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

        private void PlayInternal(in Vector3 currentDirection)
        {
            _isPlaying = true;

            for (int i = 0; i < _hitOptions.Length; i++)
            {
                _hitOptions[i].Reset();
            }

            _currentPosition = transform.position;

            _prevPos = _currentPosition - (currentDirection * _casting.InitialPrecastBackOffset);
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
                    _performer.Hits.Invoke(_listener);
                    InvokeHit();
                    return;
                }
                _penetratedHits.CleanUp(true);
            }

            UpdatePrevSpeed();
            _prevPos = _currentPosition;
            _currentPosition = _moving.GetNextPos(_currentPosition, ref _velocity, GetGravity(), deltaTime, speedScale);
            _speed = _velocity.magnitude;
            bool canPlay = ProcessMovement(_prevPos, _currentPosition, false);
            _penetratedHits.CleanUp(!_doubleCollisionCheck);
            ApplyMovement();
            _performer.Hits.Invoke(_listener);

            if (!canPlay)
            {
                InvokeHit();
            }
        }

        private bool ProcessMovement(in Vector3 source, in Vector3 destination, bool additional)
        {
            Vector3 direction = (destination - source).GetNormalized(out float magnitude);

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

                        _penetratedHits.CleanUp(true);

                        hitOptionRef.UpdateHit();
                        var (newDest, newDir, hitPos) = _moving.Reflect(_hitInfo, destination, direction, _casting.CastRadius, hitOptionRef.SpeedMultiplier);

                        UpdatePrevSpeed();
                        _speed *= hitOptionRef.SpeedMultiplier;
                        Vector3 newVelocity = newDir * _speed;

                        HitInfo hits = new HitInfo()
                        {
                            Reaction = hitOptionRef.Reaction,
                            HitData = _hitInfo,
                            HitPosition = hitPos,
                            PreviousVelocity = _velocity,
                            NewVelocity = newVelocity,
                        };
                        _performer.Hits.Add(hits);

                        _velocity = newVelocity;
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

                        Vector3 newDestination = _moving.Penetrate(_hitInfo, destination, direction, _casting.CastRadius, hitOptionRef.SpeedMultiplier);

                        UpdatePrevSpeed();
                        _speed *= hitOptionRef.SpeedMultiplier;
                        Vector3 newVelocity = direction * _speed;

                        HitInfo hits = new HitInfo()
                        {
                            Reaction = hitOptionRef.Reaction,
                            HitData = _hitInfo,
                            HitPosition = _moving.GetHitPosition(_hitInfo, _casting.CastRadius),
                            PreviousVelocity = _velocity,
                            NewVelocity = newVelocity,
                        };
                        _performer.Hits.Add(hits);

                        _velocity = newVelocity;
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
            _listener?.OnHitFinal(_hitInfo, _prevVelocity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyMovement()
        {
            if (_rotationProvider != null)
                transform.SetPositionAndRotation(_currentPosition, _rotationProvider.GetRotation());
            else if (_speed > MathUtility.kEpsilon)
                transform.SetPositionAndRotation(_currentPosition, _velocity.ToLookRotation());
            else
                transform.position = _currentPosition;
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
