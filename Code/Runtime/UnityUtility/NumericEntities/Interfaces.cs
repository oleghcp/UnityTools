using System;

namespace UnityUtility.NumericEntities
{
    public enum ResizeType : byte { NewValue, Increase, Decrease }

    public interface IMergeable<T>
    {
        void Merge(T other);
    }

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

    public interface IAbsoluteModifier<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T Value { get; }
    }

    public interface IRelativeModifier<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T Value { get; }
    }

    public interface IStaticEntity<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        T PureValue { get; }
        T MinValue { get; }
        T MaxValue { get; }
        bool Modified { get; }

        void AddModifier(IAbsoluteModifier<T> modifier);
        void AddModifier(IRelativeModifier<T> modifier);
        void RemoveModifier(IAbsoluteModifier<T> modifier);
        void RemoveModifier(IRelativeModifier<T> modifier);
        T GetCurValue();
        void Revalue(T value, ResizeType resizeType = ResizeType.NewValue);
        void Resize(T min, T max);
    }
}
