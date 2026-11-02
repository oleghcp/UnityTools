using System;

namespace OlegHcp.Pathfinding
{
    public class PathFinderLockedException : InvalidOperationException
    {
        internal PathFinderLockedException()
            : base($"PathFinder is in use. Avoid concurrent usage.")
        {

        }
    }
}
