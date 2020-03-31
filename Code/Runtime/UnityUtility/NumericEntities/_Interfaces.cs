using System;

namespace UnityUtility.NumericEntities
{
    public enum ResizeType : byte { NewValue, Increase, Decrease }

    public interface SpendingEntity<T> where T : struct, IFormattable, IConvertible, IComparable, IComparable<T>, IEquatable<T>
    {
        T Capacity { get; }
        T CurValue { get; }
        T Shortage { get; }
        T ReducingExcess { get; }
        float Ratio { get; }
        bool IsFull { get; }
        bool IsEmpty { get; }

        void Spend(T value);
        void Restore(T value);
        void RestoreFull();
        void Resize(T value, ResizeType resizeType = ResizeType.NewValue);
    }

    public interface ForcedEntity<T> where T : struct, IFormattable, IConvertible, IComparable, IComparable<T>, IEquatable<T>
    {
        T CurValue { get; }
        T Threshold { get; }
        bool LimitReached { get; }
        bool Forced { get; }
        float Ratio { get; }

        void Force(T value);
        void Restore(T value);
        void RestoreFull();
        void Resize(T value, ResizeType resizeType = ResizeType.NewValue);
    }

    public interface AccumEntity<T> where T : struct, IFormattable, IConvertible, IComparable, IComparable<T>, IEquatable<T>
    {
        T Value { get; }
        bool IsEmpty { get; }
        T Got { get; }
        T Spent { get; }

        void Add(T value);
        bool Spend(T value);
    }

    public interface AbsoluteModifier<T> where T : struct, IFormattable, IConvertible, IComparable, IComparable<T>, IEquatable<T>
    {
        T Value { get; }
    }

    public interface RelativeModifier<T> where T : struct, IFormattable, IConvertible, IComparable, IComparable<T>, IEquatable<T>
    {
        T Multiplier { get; }
    }

    public interface StaticEntity<T> where T : struct, IFormattable, IConvertible, IComparable, IComparable<T>, IEquatable<T>
    {
        T PureValue { get; }
        T MinValue { get; }
        T MaxValue { get; }
        bool Modified { get; }

        void AddModifier(AbsoluteModifier<T> modifier);
        void AddModifier(RelativeModifier<T> modifier);
        void RemoveModifier(AbsoluteModifier<T> modifier);
        void RemoveModifier(RelativeModifier<T> modifier);
        T GetCurValue();
        void Revalue(T value, ResizeType resizeType = ResizeType.NewValue);
        void Resize(T min, T max);
    }
}
