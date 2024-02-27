using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisableEditingAttribute : PropertyAttribute { }
}
