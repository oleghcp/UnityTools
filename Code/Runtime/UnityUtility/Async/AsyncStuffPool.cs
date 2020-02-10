using UU.Collections;
using UU.IDGenerating;
using UnityEngine.SceneManagement;

namespace UU.Async
{
    internal static class AsyncStuffPool
    {
        private class Data
        {
            private const string RUNNER_NAME = "Task";

            public ObjectPool<RoutineExecutor> RunnersPool;
            public UintIDGenerator IDs;

            public Data()
            {
                IDs = new UintIDGenerator();
                RunnersPool = new ObjectPool<RoutineExecutor>(f_create);

                SceneManager.sceneUnloaded += SceneUnloadedHandler;
            }

            private RoutineExecutor f_create()
            {
                return Script.CreateInstance<RoutineExecutor>(RUNNER_NAME);
            }

            private void SceneUnloadedHandler(Scene scene)
            {
                RunnersPool.Clear();
            }
        }

        private static Data s_inst;

        static AsyncStuffPool()
        {
            s_inst = new Data();
        }

        internal static uint GetNewId()
        {
            return s_inst.IDs.GetNewId();
        }

        internal static RoutineExecutor GetExecutor()
        {
            return s_inst.RunnersPool.Get();
        }

        internal static void Return(RoutineExecutor runner)
        {
            s_inst.RunnersPool.Release(runner);
        }
    }
}
