#if UNITY_2019_3_OR_NEWER
using UnityEngine;

namespace UnityUtility.AiSimulation
{
    public class AiBehavior : MonoBehaviour
    {
        [SerializeField]
        private AiBehaviorSet _behaviorSet;

        private bool _raw = true;
        private AiBehaviorSet _behaviorSetInstance;

        public AiBehaviorSet BehaviorSet
        {
            get
            {
                if (_raw)
                {
                    _behaviorSetInstance = _behaviorSet.Install();
                    _behaviorSetInstance.SetUp(gameObject);
                    _raw = false;
                }

                return _behaviorSetInstance;
            }
        }

        private void OnDestroy()
        {
            if (_behaviorSetInstance != null)
                _behaviorSetInstance.Destroy();
        }

        private void Update()
        {
            BehaviorSet.Refresh(Time.deltaTime);
        }
    }
}
#endif
