using UnityEngine;

namespace UnityUtility.Async
{
    internal class TaskYieldInstruction : CustomYieldInstruction
    {
        private TaskInfo _task;

        public override bool keepWaiting => _task.IsAlive;

        public TaskYieldInstruction(TaskInfo task)
        {
            _task = task;
        }
    }
}
