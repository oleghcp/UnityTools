using System.Runtime.CompilerServices;
using OlegHcp.Rng;
using UnityEngine;

namespace OlegHcp
{
    public static class Colours
    {
        public static Color Yellow
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.yellow;
        }

        public static Color Grey
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.grey;
        }

        public static Color Magenta
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.magenta;
        }

        public static Color Cyan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.cyan;
        }

        public static Color Red
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.red;
        }

        public static Color Black
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.black;
        }

        public static Color White
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.white;
        }

        public static Color Blue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.blue;
        }

        public static Color Sky
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0.2f, 0.5f, 1f, 1f);
        }

        public static Color Lime
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Color.green;
        }

        public static Color Green
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0f, 0.5f, 0f, 1f);
        }

        public static Color Maroon
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0.5f, 0f, 0f, 1f);
        }

        public static Color Cherry
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0.75f, 0f, 0f, 1f);
        }

        public static Color Olive
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0.5f, 0.5f, 0f, 1f);
        }

        public static Color Navy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0f, 0f, 0.5f, 1f);
        }

        public static Color Teal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0f, 0.5f, 0.5f, 1f);
        }

        public static Color Purple
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0.5f, 0f, 0.5f, 1f);
        }

        public static Color Silver
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0.75f, 0.75f, 0.75f, 1f);
        }

        public static Color Orange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(1f, 0.5f, 0f, 1f);
        }

        public static Color Violet
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0.5f, 0f, 1f, 1f);
        }

        public static Color Random
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => RandomNumberGenerator.Default.GetRandomColor();
        }
    }
}
