using System;
using UnityEngine;

namespace UnityUtility.Shooting
{
    [Serializable]
    internal class ProjectileMover
    {
        [SerializeField]
        private float _startSpeed;
        [SerializeField]
        private bool _useGravity;
        [SerializeField, Min(-1)]
        private int _ricochets;
        [SerializeField]
        private LayerMask _ricochetMask;
        [SerializeField, Range(0f, 1f)]
        private float _speedRemainder = 1f;

        private float _curTime;
        private int _ricochetsLeft;
        private Arc3 _arc;

        public int Ricochets => _ricochets;
        public LayerMask RicochetMask => _ricochetMask;
        public int RicochetsLeft => _ricochetsLeft;

        public void Init(Transform transform)
        {
            if (_useGravity)
            {
                _curTime = 0f;
                _arc = new Arc3(transform.forward, _startSpeed, Physics.gravity.magnitude, transform.position);
            }
            else
            {
                _arc.StartSpeed = _startSpeed;
                _arc.StartDir = transform.forward;
            }

            _ricochetsLeft = _ricochets;
        }

        public Vector3 GetNextPos(in Vector3 curPos, float deltaTime, float speedScale = 1f)
        {
            if (_useGravity)
                return _arc.Evaluate(_curTime += deltaTime * speedScale);
            else
                return curPos + _arc.StartDir * (_arc.StartSpeed * deltaTime * speedScale);
        }

        public Vector3 Ricochet(in RaycastHit hitInfo, in Vector3 prevPos, in Vector3 lineEndPoint)
        {
            _ricochetsLeft--;

            Vector3 oldDir = lineEndPoint - prevPos;
            Vector3 newDir = Vector3.Reflect(oldDir.normalized, hitInfo.normal);

            if (_useGravity)
            {
                _curTime = 0f;
                _arc = new Arc3(newDir, _startSpeed, Physics.gravity.magnitude, hitInfo.point);
            }
            else
            {
                _arc.StartSpeed *= _speedRemainder;
                _arc.StartDir = newDir;
            }

            float len = Vector3.Distance(hitInfo.point, lineEndPoint) * _speedRemainder;
            if (len <= Vector3.kEpsilon)
                len = 0.1f;

            return newDir * len;
        }
    }
}
