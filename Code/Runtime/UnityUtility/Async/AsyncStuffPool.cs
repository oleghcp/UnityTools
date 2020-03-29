using UnityEngine.SceneManagement;
using UU.Collections;
using UU.IDGenerating;

namespace UU.Async
{
    internal static class AsyncStuffPool
    {
        private class Data
        {
            private const string RUNNER_NAME = "Task";

            public readonly ObjectPool<RoutineRunner> RunnersPool;
            public readonly IDGenerator<ulong> IdProvider;

            public Data()
            {
                IdProvider = new UlongIDGenerator();
                RunnersPool = new ObjectPool<RoutineRunner>(f_create);

                SceneManager.sceneUnloaded += _ => RunnersPool.Clear();
            }

            private RoutineRunner f_create()
            {
                return Script.CreateInstance<RoutineRunner>(RUNNER_NAME);
            }
        }

        private static readonly Data s_inst;

        static AsyncStuffPool()
        {
            s_inst = new Data();
        }

        internal static ulong GetNewId()
        {
            return s_inst.IdProvider.GetNewId();
        }

        internal static RoutineRunner GetExecutor()
        {
            return s_inst.RunnersPool.Get();
        }

        internal static void Return(RoutineRunner runner)
        {
            s_inst.RunnersPool.Release(runner);
        }
    }
}
