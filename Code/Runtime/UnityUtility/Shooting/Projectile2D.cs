using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility.MathExt;

namespace UnityUtility.Shooting
{
    public class Projectile2D : MonoBehaviour
    {
        [SerializeField]
        private bool _playOnAwake;
        [SerializeField, Min(0f)]
        private float _lifeTime = float.PositiveInfinity;
        [SerializeField]
        private bool _autodestruct;
        [SerializeField]
        private ProjectileMover2D _moving;
        [SerializeField]
        private ProjectileCaster2D _casting;
        [Space]
        [SerializeField]
        private UnityEvent<ProjectileEventType> _onFinal;
        [SerializeField]
        private UnityEvent<Vector2> _onReflect;

        private ITimeProvider _timeProvider;
        private IGravityProvider2D _gravityProvider;

        private bool _canMove;
        private int _ricochetsLeft;
        private Vector2 _prevPos;
        private Vector2 _velocity;

        public UnityEvent<ProjectileEventType> OnFinal => _onFinal;
        public UnityEvent<Vector2> OnReflect => _onReflect;

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

            Quaternion curRot = Quaternion.FromToRotation(Vector3.right, UpdateDirection());
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
            _velocity = transform.right * _moving.StartSpeed;

            Vector2 newPos = _moving.GetNextPos(_prevPos = transform.position, ref _velocity, GetGravity(), GetDeltaTime(), 0.5f);
            UpdateState(_prevPos, newPos, out _prevPos, out newPos);
            Quaternion newRot = Quaternion.FromToRotation(Vector3.right, UpdateDirection());

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

        public void OverrideGravityProvider(IGravityProvider2D gravityProvider)
        {
            _gravityProvider = gravityProvider;
        }

        private void UpdateState(in Vector2 source, in Vector2 dest, out Vector2 newSource, out Vector2 newDest)
        {
            Vector2 vector = dest - source;
            Vector2 direction = vector.normalized;

            if (_casting.Cast(source, direction, vector.magnitude, out RaycastHit2D hitInfo))
            {
                if (_ricochetsLeft != 0 && hitInfo.CompareLayer(_moving.RicochetMask))
                {
                    _ricochetsLeft--;

                    var reflectionInfo = _moving.Reflect(hitInfo, dest, direction);
                    _velocity = reflectionInfo.newDir * (_velocity.magnitude * _moving.SpeedRemainder);
                    _onReflect.Invoke(hitInfo.point);
                    UpdateState(Vector2.LerpUnclamped(hitInfo.point, reflectionInfo.newDest, 0.1f), reflectionInfo.newDest, out newSource, out newDest);
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

        private Vector2 UpdateDirection()
        {
            float length = _velocity.magnitude;

            if (length > Vector2.kEpsilon)
                return _velocity / length;

            return transform.right;
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
        private Vector2 GetGravity()
        {
            return _gravityProvider != null ? _gravityProvider.GetGravity()
                                            : Physics2D.gravity;
        }
    }
}
