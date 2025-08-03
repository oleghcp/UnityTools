using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReferenceSelectionAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RemoveFromSelectionAttribute : PropertyAttribute { }
}
