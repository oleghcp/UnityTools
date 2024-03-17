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
