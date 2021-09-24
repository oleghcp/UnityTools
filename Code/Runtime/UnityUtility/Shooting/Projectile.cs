using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
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
        private ProjectileMover _mover;
        [SerializeField]
        private ProjectileCaster _caster;

        private Vector3 _prevPos;
        private bool _canMove;

        private void Start()
        {
            if (_playOnAwake)
                Play();
        }

        private void Update()
        {
            if (_canMove)
            {
                _canMove = UpdateState(_prevPos, transform.position, out Vector3 newPos);

                if (_canMove)
                {
                    newPos = _mover.GetNextPos(newPos, GetDeltaTime());
                    _canMove = UpdateState(transform.position, newPos, out newPos);
                }

                _prevPos = transform.position;
                Vector3 dir = newPos - _prevPos;

                if (dir.magnitude > Vector3.kEpsilon)
                    transform.SetPositionAndRotation(newPos, Quaternion.LookRotation(dir));
                else
                    transform.position = newPos;

                if (!_canMove)
                    Fin("Hit");
            }

            Debug.DrawLine(_prevPos, transform.position, Colours.Red, Time.deltaTime);
        }

        public async void Play()
        {
            if (_canMove)
                return;

            _canMove = true;

            _mover.Init(transform);

            transform.position = _mover.GetNextPos(_prevPos = transform.position, GetDeltaTime(), 0.5f);

            if (_lifeTime.IsPosInfinity())
                return;

            await Task.Delay((_lifeTime * 1000).ToInt());

            if (!_canMove)
                return;

            _canMove = false;

            Fin("Time Out");
        }

        private bool UpdateState(in Vector3 from, in Vector3 to, out Vector3 result)
        {
            if (_caster.Cast(from, to, out RaycastHit hitInfo))
            {
                if (hitInfo.CompareLayer(_mover.RicochetMask) && _mover.RicochetsLeft != 0)
                {
                    Vector3 dir = _mover.Ricochet(hitInfo, from, to);
                    return UpdateState(hitInfo.point, hitInfo.point + dir, out result);
                }

                result = hitInfo.point;
                return false;
            }

            result = to;
            return true;
        }

        private void Fin(string type)
        {
            SendMessage("OnHit", type, SendMessageOptions.DontRequireReceiver);

            if (_autodestruct)
                gameObject.Destroy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetDeltaTime()
        {
            //return m_config.Hostile ? Session.Run.FoeDeltaTime : Time.deltaTime;
            return Time.deltaTime;
        }
    }
}
