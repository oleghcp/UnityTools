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
        IRng rng = RandomNumberGenerator.Default;

        array.Fill(index => new Vector3(rng.Next(100f),
                                        rng.Next(100f),
                                        rng.Next(100f)));        
        
        array.Sort(item => item.magnitude);

        int index = array.IndexOf(item => item.magnitude > 50f);

        if (index >= 0)
        {
            // Do something
        }

        array.ForEach(item => Debug.Log(item));

        ArraySegment<Vector3> segment = array.Slice(10, 50);

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

