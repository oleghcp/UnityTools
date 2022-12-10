using System;

namespace UnityUtility.NumericEntities
{
    public interface IAccumEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T Value { get; }
        bool IsEmpty { get; }
        T Got { get; }
        T Spent { get; }

        void Add(T value);
        bool Spend(T value);
    }

    public interface ISpendingEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T Capacity { get; set; }
        T CurValue { get; set; }
        T Shortage { get; }
        T ReducingExcess { get; }
        float Ratio { get; }
        bool IsFull { get; }
        bool IsEmpty { get; }

        void Spend(T delta);
        void RemoveExcess();
        void Restore(T delta);
        void RestoreFull();
    }

    public interface IFilledEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T Threshold { get; set; }
        T CurValue { get; }
        bool FilledFully { get; }
        bool IsEmpty { get; }
        float Ratio { get; }
        T Excess { get; }
        T Shortage { get; }

        void Fill(T delta);
        void FillFully();
        void Remove(T delta);
        void RemoveAll();
        void RemoveExcess();
    }

    public interface IModifier<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        bool Relative { get; }
        T Value { get; }
    }

    public interface IModifiableEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T PureValue { get; set; }
        T MinValue { get; }
        T MaxValue { get; }
        bool Modified { get; }

        void AddModifier(IModifier<T> modifier);
        void RemoveModifier(IModifier<T> modifier);
        T GetModifiedValue();
        void Resize(T min, T max);
    }
}
