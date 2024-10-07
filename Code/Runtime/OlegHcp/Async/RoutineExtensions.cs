using System.Collections;
using System.Threading;

namespace OlegHcp.Async
{
    public static class RoutineExtensions
    {
        public static TaskInfo StartAsync(this IEnumerator self, bool unstoppable = false)
        {
            return TaskSystem.StartAsync(self, unstoppable);
        }

        public static TaskInfo StartAsync(this IEnumerator self, in CancellationToken token)
        {
            return TaskSystem.StartAsync(self, token);
        }

        public static TaskInfo StartAsyncLocally(this IEnumerator self, bool unstoppable = false)
        {
            return TaskSystem.StartAsyncLocally(self, unstoppable);
        }

        public static TaskInfo StartAsyncLocally(this IEnumerator self, in CancellationToken token)
        {
            return TaskSystem.StartAsyncLocally(self, token);
        }
    }
}
