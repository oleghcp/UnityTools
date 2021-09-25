using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility.MathExt;

namespace UnityUtility.Shooting
{
    public class Projectile : MonoBehaviour
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
        [Space]
        [SerializeField]
        private UnityEvent<ProjectileEventType> _onFinal;
        [SerializeField]
        private UnityEvent<Vector3> _onReflect;

        private ITimeProvider _timeProvider;
        private IGravityProvider _gravityProvider;

        private bool _canMove;
        private int _ricochetsLeft;
        private Vector3 _prevPos;
        private Vector3 _velocity;

        public UnityEvent<ProjectileEventType> OnFinal => _onFinal;
        public UnityEvent<Vector3> OnReflect => _onReflect;

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
                Fin(ProjectileEventType.Hit);
        }

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

            if (_lifeTime.IsPosInfinity())
                return;

            await Task.Delay((_lifeTime * 1000).ToInt());

            if (!_canMove)
                return;

            _canMove = false;

            Fin(ProjectileEventType.TimeOut);
        }

        public void OverrideTimeProvider(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public void OverrideGravityProvider(IGravityProvider gravityProvider)
        {
            _gravityProvider = gravityProvider;
        }

        private void UpdateState(in Vector3 source, in Vector3 dest, out Vector3 newSource, out Vector3 newDest)
        {
            Vector3 vector = dest - source;
            Vector3 direction = vector.normalized;

            if (_casting.Cast(source, direction, vector.magnitude, out RaycastHit hitInfo))
            {
                if (_ricochetsLeft != 0 && hitInfo.CompareLayer(_moving.RicochetMask))
                {
                    _ricochetsLeft--;

                    var reflectionInfo = _moving.Reflect(hitInfo, dest, direction);
                    _velocity = reflectionInfo.newDir * (_velocity.magnitude * _moving.SpeedRemainder);
                    _onReflect.Invoke(hitInfo.point);
                    UpdateState(hitInfo.point, reflectionInfo.newDest, out newSource, out newDest);
                    return;
                }

                _canMove = false;
                newSource = source;
                newDest = hitInfo.point;

                return;
            }

            newSource = source;
            newDest = dest;
        }

        private Vector3 UpdateDirection()
        {
            float length = _velocity.magnitude;

            if (length > Vector3.kEpsilon)
                return _velocity / length;

            return transform.forward;
        }

        private void Fin(ProjectileEventType type)
        {
            if (_autodestruct)
                gameObject.Destroy();

            _onFinal.Invoke(type);
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
