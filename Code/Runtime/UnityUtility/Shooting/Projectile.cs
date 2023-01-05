﻿using System;
using UnityEngine;
using UnityUtility.Engine;
using UnityUtility.Inspector;
using UnityUtility.Mathematics;

#if UNITY_2019_3_OR_NEWER && INCLUDE_PHYSICS
namespace UnityUtility.Shooting
{
    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(UnityUtility) + "/Projectile")]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField]
        private bool _playOnAwake;
        [SerializeField, Min(0f)]
        private float _timer = float.PositiveInfinity;
        [SerializeField]
        private bool _autodestruct;
        [SerializeField]
        private ProjectileMover _moving;
        [SerializeField]
        private ProjectileCaster _casting;
        [SerializeReference, InitToggle]
        private ProjectileEvents _events;

        private IRotationProvider _rotationProvider;
        private ITimeProvider _timeProvider;
        private IGravityProvider _gravityProvider;
        private IProjectileEventListener _listener;

        private bool _canMove;
        private float _currentTime;
        private int _ricochetsLeft;
        private Vector3 _prevPos;
        private Vector3 _velocity;
        private RaycastHit _hitInfo;

        public Vector3 PrevPos => _prevPos;
        public int RicochetsLeft => _ricochetsLeft;

        public float Timer
        {
            get => _timer;
            set => _timer = value;
        }

        public Vector3 Velocity
        {
            get => _canMove ? _velocity : Vector3.zero;
            set
            {
                if (!_canMove)
                    throw new InvalidOperationException("Projectile is stopped.");

                _velocity = value;
            }
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
        public IRotationProvider RotationProvider { get => _rotationProvider; set => _rotationProvider = value; }
        public ITimeProvider TimeProvider { get => _timeProvider; set => _timeProvider = value; }
        public IGravityProvider GravityProvider { get => _gravityProvider; set => _gravityProvider = value; }
        public IProjectileEventListener Listener { get => _listener; set => _listener = value; }

        private void Start()
        {
            if (_playOnAwake)
                Play();
        }

        private void Update()
        {
            if (_canMove)
            {
                if (_currentTime >= _timer)
                {
                    _canMove = false;
                    InvokeTimeOut();
                }
                else
                {
                    float deltaTime = GetDeltaTime();
                    _currentTime += deltaTime;

                    UpdateState(transform.position, deltaTime, 1f);

                    if (!_canMove)
                        InvokeHit();
                }
            }

            _listener?.OnUpdate();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _casting.InitialPrecastOffset = _casting.InitialPrecastOffset;
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

            LayerMask mask = LayerMask.GetMask("Default");
            _moving.RicochetMask = mask;
            _casting.HitMask = mask;
            _moving.SpeedRemainder = 1f;
            _casting.ReflectedCastNear = 0.1f;
            _casting.InitialPrecastOffset = 0f;
        }
#endif

        public void Play()
        {
            if (_canMove)
                return;

            PlayInternal(transform.forward);
        }

        public void Play(float startSpeed)
        {
            if (_canMove)
                return;

            _moving.StartSpeed = startSpeed;
            PlayInternal(transform.forward);
        }

        public void Play(in Vector3 velocity)
        {
            if (_canMove)
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
            _canMove = false;
            _currentTime = 0f;
        }

        private void PlayInternal(in Vector3 currentDirection)
        {
            _canMove = true;
            _ricochetsLeft = _moving.Ricochets;

            Vector3 currentPosition = transform.position;

            _prevPos = currentPosition + currentDirection * _casting.InitialPrecastOffset;
            _velocity = currentDirection * _moving.StartSpeed;

            if (_moving.MoveInInitialFrame > 0f)
            {
                UpdateState(currentPosition, GetDeltaTime(), _moving.MoveInInitialFrame);

                if (!_canMove)
                {
                    InvokeHit();
                    return;
                }
            }
        }

        private void UpdateState(Vector3 currentPosition, float deltaTime, float speedScale)
        {
            CheckMovement(_prevPos, currentPosition, out _prevPos, out currentPosition);

            if (_canMove)
            {
                Vector3 newPos = _moving.GetNextPos(currentPosition, ref _velocity, GetGravity(), deltaTime, speedScale);
                CheckMovement(currentPosition, newPos, out _prevPos, out currentPosition);
            }

            transform.SetPositionAndRotation(currentPosition, GetRotation());
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
                    if (_ricochetsLeft > 0 && _moving.RicochetMask.HasLayer(_hitInfo.GetLayer()))
                    {
                        _ricochetsLeft--;

                        var reflectionInfo = _moving.Reflect(_hitInfo, dest, direction, _casting.CastRadius);
                        _velocity = reflectionInfo.newDir * (_velocity.magnitude * _moving.SpeedRemainder);

                        _events?.OnReflect.Invoke(_hitInfo);
                        _listener?.OnReflect(_hitInfo);

                        float near = _casting.ReflectedCastNear;
                        Vector3 from = near == 0f ? _hitInfo.point
                                                  : Vector3.LerpUnclamped(_hitInfo.point, reflectionInfo.newDest, near);

                        CheckMovement(from, reflectionInfo.newDest, out newSource, out newDest);
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
            _currentTime = 0f;

            if (_autodestruct)
                gameObject.Destroy();

            _events?.OnHit.Invoke(_hitInfo);
            _listener?.OnHit(_hitInfo);
        }

        private void InvokeTimeOut()
        {
            _currentTime = 0f;

            if (_autodestruct)
                gameObject.Destroy();

            _events?.OnTimeOut.Invoke();
            _listener?.OnTimeOut();
        }

        private Quaternion GetRotation()
        {
            if (_rotationProvider != null)
                return _rotationProvider.GetRotation();

            float magnitude = _velocity.magnitude;

            if (magnitude > MathUtility.kEpsilon)
                return (_velocity / magnitude).ToLookRotation();

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
