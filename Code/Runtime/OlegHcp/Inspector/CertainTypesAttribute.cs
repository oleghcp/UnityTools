using System;
using UnityEngine;

namespace OlegHcp.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CertainTypesAttribute : PropertyAttribute
    {
        internal Type[] Types;

        public CertainTypesAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
