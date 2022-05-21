using System;

namespace UnityUtility.NumericEntities
{
    public enum ResizeType : byte { NewValue, Delta }

    public interface ISpendingEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T Capacity { get; }
        T CurValue { get; }
        T Shortage { get; }
        T ReducingExcess { get; }
        float Ratio { get; }
        bool IsFull { get; }
        bool IsEmpty { get; }

        void Spend(T value);
        void RemoveExcess();
        void Restore(T value);
        void RestoreFull();
        void Resize(T value, ResizeType resizeType = ResizeType.NewValue);
    }

    public interface IFilledEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T CurValue { get; }
        T Threshold { get; }
        bool FilledFully { get; }
        bool IsEmpty { get; }
        float Ratio { get; }
        T Excess { get; }

        void Fill(T addValue);
        void FillFully();
        void Remove(T removeValue);
        void RemoveAll();
        void RemoveTillExcess();
        void Resize(T value, ResizeType resizeType = ResizeType.NewValue);
    }

    public interface IAccumEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T Value { get; }
        bool IsEmpty { get; }
        T Got { get; }
        T Spent { get; }

        void Add(T value);
        bool Spend(T value);
    }

    public interface IModifier<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        bool Relative { get; }
        T Value { get; }
    }

    public interface IStaticEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T PureValue { get; }
        T MinValue { get; }
        T MaxValue { get; }
        bool Modified { get; }

        void AddModifier(IModifier<T> modifier);
        void RemoveModifier(IModifier<T> modifier);
        T GetModifiedValue();
        void Revalue(T value, ResizeType resizeType = ResizeType.NewValue);
        void Resize(T min, T max);
    }
}
