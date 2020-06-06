using System;
using UnityUtility.Async;

namespace Tools
{
    public static class Errors
    {
        public static InvalidOperationException NoElements()
        {
            return new InvalidOperationException("Collection is empty.");
        }

        public static InvalidOperationException CollectionChanged()
        {
            return new InvalidOperationException("Collection has been changed.");
        }

        public static ArgumentOutOfRangeException NegativeParameter(string nameOfLength)
        {
            return new ArgumentOutOfRangeException(nameOfLength, $"{nameOfLength} cannot be negative.");
        }

        public static ArgumentOutOfRangeException ZeroParameter(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName, "The parameter must be greater than zero.");
        }

        public static ArgumentException DifferentArrayLengths()
        {
            return new ArgumentException("Array lengths are not equal.");
        }

        public static IndexOutOfRangeException IndexOutOfRange()
        {
            return new IndexOutOfRangeException("The index is out of range.");
        }

        public static InvalidOperationException CannotStopTask()
        {
            return new InvalidOperationException($"Task cannot be stopped. Check {TaskSystem.SYSTEM_NAME} settings.");
        }

        public static ArgumentOutOfRangeException NegativeTime(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName, "Time cannot be negative.");
        }

        public static InvalidOperationException DisposingNonEditable()
        {
            return new InvalidOperationException("Non-editable objects cannot be disposed. Probably it is a prefab reference.");
        }

        public static ArgumentOutOfRangeException MinMax(string minName, string maxName)
        {
            return new ArgumentOutOfRangeException(minName, $"{minName} cannot be greater than {maxName}.");
        }

        public static InvalidOperationException RangeDoesNotContain(string valuesName)
        {
            return new InvalidOperationException($"The range does not contain {valuesName} values.");
        }

        public static ArgumentOutOfRangeException RadixOutOfRange(string paramName, int symbolsLength)
        {
            return new ArgumentOutOfRangeException(paramName, $"The radix must be >= 2 and <= {symbolsLength}");
        }
    }
}
