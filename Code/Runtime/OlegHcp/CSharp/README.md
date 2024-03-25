## ArrayExtensions

```csharp
using System;
using OlegHcp;
using OlegHcp.CSharp;
using UnityEngine;

public class Example
{
    private void DoSomething()
    {
        Vector3[] array = new Vector3[100];

        array.Fill(index => new Vector3(RandomNumberGenerator.Default.Next(100f),
                                        RandomNumberGenerator.Default.Next(100f),
                                        RandomNumberGenerator.Default.Next(100f)));        
        
        array.Sort(item => item.magnitude);

        int index = array.IndexOf(item => item.magnitude > 50f);

        if (index >= 0)
        {
            // Do something
        }

        array.ForEach(item => Debug.Log(item));

        ArraySegment<Vector3> segment = array.Slice(10, 50);
        
        foreach (Vector3 item in segment)
        {
            // Do something
        }

        array.Clear();
    }
}
```

## StringExtensions

```csharp
using OlegHcp.CSharp;

public class Example
{
    private void DoSomething(string str)
    {
        if (str.IsNullOrEmpty())
        {
            // Do something
        }

        if (str.HasAnyData())
        {
            // Do something
        }

        string newString = str.RemoveWhiteSpaces();

        char ch = str.GetRandomChar();
    }
}
```

## CommonExtensions

```csharp
using System;
using OlegHcp.CSharp;

public class Example
{
    private void DoSomething1()
    {
        object obj = new object();

        WeakReference<object> weakRef = obj.ToWeak();

        object target = weakRef.GetTarget();

        if (target != null)
        {
            // Do something
        }
    }

    private void DoSomething2()
    {
        int num = 0xFFFF;

        string hexString = num.ToString(16);
    }

    private void DoSomething3()
    {
        MyEnum myEnum = MyEnum.A;

        string name = myEnum.GetName();
    }

    private void DoSomething4()
    {
        Type type = typeof(int);

        string typeName = type.GetTypeName();

        TypeCode typeCode = type.GetTypeCode();

        int defValue = (int)type.GetDefaultValue();

        if (type.IsAssignableTo(typeof(ValueType)))
        {
            // Do something
        }
    }
}
```

## ConcatToStringExtensions

```csharp
using System.Collections.Generic;
using OlegHcp.CSharp;
using UnityEngine;

public class Example
{
    private void DoSomething1(object[] array)
    {
        string str = array.ConcatToString('.');
    }

    private void DoSomething1(IEnumerable<object> array)
    {
        string str = array.ConcatToString("->");
    }

    private void DoSomething2(Vector2[] array)
    {
        string str = array.ConcatToString(" | ");
    }
}
```

## Enum

```csharp
using OlegHcp.CSharp;
using UnityEngine;

public class Example
{
    private void DoSomething()
    {
        int nameCount = Enum<KeyCode>.Count;
    }
}
```

## SpanExtensions

```csharp
using System;
using System.Collections.Generic;
using OlegHcp;
using OlegHcp.CSharp;
using UnityEngine;

public class Example
{
    private void DoSomething()
    {
        Span<Vector3> span = stackalloc Vector3[20];

        span.Fill(index => new Vector3(RandomNumberGenerator.Default.Next(100f),
                                       RandomNumberGenerator.Default.Next(100f),
                                       RandomNumberGenerator.Default.Next(100f)));

        span.Sort(item => item.magnitude);

        int index = span.IndexOf(item => item.magnitude > 50f);

        if (index >= 0)
        {
            // Do something
        }

        span.ForEach(item => Debug.Log(item));

        span.Shuffle();

        Vector3 vector = span.GetRandomItem();

        List<Vector3> list = span.ToList();
    }
}
```

## CollectionExtensions

```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections;
using UnityEngine;

public class Example
{
    private void DoSomething1(List<Vector3> list)
    {
        if (list.IsNullOrEmpty())
        {
            return;
        }

        list.DisplaceRight();
        list.DisplaceLeft();

        list.Sort(item => item.magnitude);

        list.Expand(10, () => new Vector3());

        int index = list.IndexOf(item => item.magnitude > 50f);

        if (index >= 0)
        {
            // Do something
        }

        int indexOfMax = list.IndexOfMax(item => item.magnitude);
        int indexOfMin = list.IndexOfMin(item => item.magnitude);

        Vector3[] array = list.GetSubArray(10, 50);

        ListSegment<Vector3> segment = list.Slice(10, 50);

        foreach (Vector3 item in segment)
        {
            // Do something
        }

        list.Shuffle();

        Vector3 randomItem = list.GetRandomItem();

        Vector3 newItem1 = list.Place(new Vector3());
        Vector3 newItem2 = list.Push(20, new Vector3());

        Vector3 oldItem1 = list.PullOut(20);
        Vector3 oldItem2 = list.Pop();
    }

    private void DoSomething2(Dictionary<int, Vector3> dict)
    {
        Vector3 newValue = dict.Place(100, new Vector3());

        if (dict.TryAdd(200, new Vector3()))
        {
            // Do something
        }

        (int key, Vector3 value) pair = (20, Vector3.right);
        dict.Add(pair);

        ReadOnlyDictionary<int, Vector3> readonlyDict = dict.AsReadOnly();

        readonlyDict.ForEach(item => Debug.Log($"{item.Key} : {item.Value}"));

        Vector3 value = readonlyDict.GetOrCreateValue(150, key => new Vector3());
    }
}
```

## EnumerableExtensions

```csharp
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using UnityEngine;

public class Example
{
    private void DoSomething(IEnumerable<Vector3> collection)
    {
        if (collection.IsNullOrEmpty())
        {
            return;
        }

        Vector3 min = collection.GetWithMin(item => item.magnitude);
        Vector3 max = collection.GetWithMax(item => item.magnitude);

        collection.InsertItem(new Vector3(), new Vector3(), new Vector3())
                  .AppendItem(new Vector3(), new Vector3())
                  .ForEach(item => Debug.Log(item)); 
    }
}
```

## ReadOnlyCollectionExtensions

```csharp
using System.Collections.Generic;
using OlegHcp.CSharp.Collections.ReadOnly;
using UnityEngine;

public class Example
{
    private void DoSomething1(IReadOnlyList<Vector3> list)
    {
        if (list.IsNullOrEmpty_())
        {
            return;
        }

        int index = list.IndexOf_(item => item.magnitude > 50f);

        if (index >= 0)
        {
            // Do something
        }

        list.ForEach_(item => Debug.Log(item));

        int indexOfMax = list.IndexOfMax_(item => item.magnitude);
        int indexOfMin = list.IndexOfMin_(item => item.magnitude);

        Vector3[] array = list.GetSubArray_(10, 50);

        ReadOnlySegment<Vector3> segment = list.Slice_(10, 50);

        foreach (Vector3 item in segment)
        {
            // Do something
        }

        Vector3 randomItem = list.GetRandomItem_();

    }

    private void DoSomething2(IReadOnlyDictionary<int, Vector3> dict)
    {
        dict.ForEach_(item => Debug.Log($"{item.Key} : {item.Value}"));
    }
}
```

## Iterators

Useful for collection wrappers.

```csharp
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections.Iterators;

public class ExampleWrapper<T>
{
    private T[] _items;

    public ExampleWrapper(T[] items)
    {
        _items = items.GetCopy();
    }

    public ArrayEnumerator<T> GetEnumerator()
    {
        return new ArrayEnumerator<T>(_items);
    }
}
```

```csharp
public class Example
{
    private void DoSomething()
    {
        ExampleWrapper<int> wrapper = new ExampleWrapper<int>(new[] { 0, 1, 2, 3, 4, 5 });

        foreach (int item in wrapper)
        {
            UnityEngine.Debug.Log(item);
        }
    }
}
```

## StringBuilderExtensions

```csharp
using System.Text;
using OlegHcp.CSharp.Text;

public class Example
{
    private void DoSomething()
    {
        StringBuilder builder = new StringBuilder();

        string str = builder.Append("Hello")
                            .Append(' ')
                            .Append("World!")
                            .Cut(); // Return and clear
    }
}
```

## IOExtensions

```csharp
using System.IO;
using OlegHcp.CSharp.IO;

public class Example
{
    private void DoSomething1(DirectoryInfo directory)
    {
        string parentPath = directory.GetParentPath();
    }

    private void DoSomething2(FileInfo file)
    {
        string parentPath = file.GetParentPath();
    }
}
```

## WeakTableExtensions

```csharp
using System.Runtime.CompilerServices;
using OlegHcp.CSharp.Runtime.CompilerServices;

public class Example
{
    private void DoSomething(ConditionalWeakTable<object, ObjectData> weakTable)
    {
        object obj = new object();

        ObjectData data = weakTable.Place(obj, new ObjectData());

        data = weakTable.GetOrCreateValue(obj);
    }
}
```
