using System.Threading.Tasks;
using UnityEngine;
using UnityUtility.Tools;

namespace Runtime.UnityUtility.Async
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
