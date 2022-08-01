using UnityEngine;

namespace UnityUtility.AiSimulation
{
    public class AiBehavior : MonoBehaviour
    {
        [SerializeField]
        private AiBehaviorSet _behaviorSet;

        private bool _raw = true;

        public AiBehaviorSet BehaviorSet
        {
            get
            {
                if (_raw)
                {
                    _behaviorSet = _behaviorSet.Install();
                    _raw = false;
                }

                return _behaviorSet;
            }
        }

        private void Awake()
        {
            BehaviorSet.SetUp(gameObject);
        }

        private void OnDestroy()
        {
            _behaviorSet.Destroy();
        }

        private void Update()
        {
            _behaviorSet.Refresh(Time.deltaTime);
        }
    }
}
