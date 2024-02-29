using System.Threading.Tasks;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.Async
{
    public class WaitForTask : CustomYieldInstruction
    {
        private Task _task;

        public override bool keepWaiting => !_task.IsCompleted;

        public WaitForTask(Task task)
        {
            if (task == null)
                throw ThrowErrors.NullParameter(nameof(task));

            _task = task;
        }
    }
}
