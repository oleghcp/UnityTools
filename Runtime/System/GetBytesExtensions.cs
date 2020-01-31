using System.Runtime.CompilerServices;

namespace System
{
    public static class GetBytesExtensions
    {
        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this char value) => BitConverter.GetBytes(value);

        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this short value) => BitConverter.GetBytes(value);

        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this int value) => BitConverter.GetBytes(value);

        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this long value) => BitConverter.GetBytes(value);

        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this ushort value) => BitConverter.GetBytes(value);

        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this uint value) => BitConverter.GetBytes(value);

        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this ulong value) => BitConverter.GetBytes(value);

        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this float value) => BitConverter.GetBytes(value);

        /// <summary>
        /// Returns the value as an array of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(this double value) => BitConverter.GetBytes(value);
    }
}
