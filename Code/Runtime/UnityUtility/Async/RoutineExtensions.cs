using System.Collections;
using System.Threading;

namespace OlegHcp.Async
{
    public static class RoutineExtensions
    {
        public static TaskInfo StartAsync(this IEnumerator self)
        {
            return TaskSystem.StartAsync(self);
        }

        public static TaskInfo StartAsync(this IEnumerator self, in CancellationToken token)
        {
            return TaskSystem.StartAsync(self, token);
        }

        public static TaskInfo StartAsyncLocally(this IEnumerator self)
        {
            return TaskSystem.StartAsyncLocally(self);
        }

        public static TaskInfo StartAsyncLocally(this IEnumerator self, in CancellationToken token)
        {
            return TaskSystem.StartAsyncLocally(self, token);
        }
    }
}
