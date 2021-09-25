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
        private ProjectileMover _moving;
        [SerializeField]
        private ProjectileCaster _casting;

        private bool _canMove;
        private float _curSpeed;
        private int _ricochetsLeft;
        private Vector3 _prevPos;

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
            Vector3 curDir = UpdateState(_prevPos, curPos, transform.forward, out _prevPos, out curPos);

            if (_canMove)
            {
                UpdateDirection(_prevPos, curPos, ref curDir);
                Vector3 newPos = _moving.GetNextPos(curPos, curDir, _curSpeed, GetDeltaTime(), 1f);
                curDir = UpdateState(curPos, newPos, curDir, out _prevPos, out newPos);
                curPos = newPos;
            }

            transform.SetPositionAndRotation(curPos, Quaternion.LookRotation(curDir));

            if (!_canMove)
                Fin("Hit");

            Debug.DrawLine(_prevPos, curPos, Colours.Red, Time.deltaTime);
        }

        public async void Play()
        {
            if (_canMove)
                return;

            _canMove = true;
            _ricochetsLeft = _moving.Ricochets;
            _curSpeed = _moving.StartSpeed;

            Vector3 newPos = _moving.GetNextPos(_prevPos = transform.position, transform.forward, _curSpeed, GetDeltaTime(), 0.5f);
            Vector3 newDir = UpdateState(_prevPos, newPos, transform.forward, out _prevPos, out newPos);

            transform.SetPositionAndRotation(newPos, Quaternion.LookRotation(newDir));

            if (_lifeTime.IsPosInfinity())
                return;

            await Task.Delay((_lifeTime * 1000).ToInt());

            if (!_canMove)
                return;

            _canMove = false;

            Fin("Time Out");
        }

        private Vector3 UpdateState(in Vector3 source, in Vector3 dest, in Vector3 direction, out Vector3 newSource, out Vector3 newDest)
        {
            if (_casting.Cast(source, direction, Vector3.Distance(dest, source), out RaycastHit hitInfo))
            {
                if (_ricochetsLeft != 0 && hitInfo.CompareLayer(_moving.RicochetMask))
                {
                    _ricochetsLeft--;
                    _curSpeed *= _moving.SpeedRemainder;

                    var r = _moving.Reflect(hitInfo, dest, direction);
                    return UpdateState(hitInfo.point, r.newDest, r.newDir, out newSource, out newDest);
                }

                _canMove = false;
                newSource = source;
                newDest = hitInfo.point;

                return direction;
            }

            newSource = source;
            newDest = dest;

            return direction;
        }

        private void UpdateDirection(in Vector3 source, in Vector3 dest, ref Vector3 direction)
        {
            Vector3 vector = dest - source;
            float length = vector.magnitude;

            if (length > Vector3.kEpsilon)
                direction = vector / length;
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
