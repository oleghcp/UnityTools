using System;

namespace Tools
{
    public static class Errors
    {
        public static InvalidOperationException NoElements()
        {
            return new InvalidOperationException("Collection is empty.");
        }

        public static ArgumentNullException ArgumentNull(string paramName)
        {
            return new ArgumentNullException(paramName);
        }
    }
}
