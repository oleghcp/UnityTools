## DisableEditingAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [DisableEditing]
    private string _text = "Qwerty";
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/DisableEditing.png)

## InspectorButtonAttribute

```csharp
using System;
using OlegHcp.Inspector;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    [SerializeField]
    private string _text;

    [InspectorButton("Do something 1", 25f)]
    private void ExampleMethod1()
    {
        Debug.Log(_text);
    }

    [InspectorButton("Do something 2", 25f)]
    private static void ExampleMethod2()
    {
        Debug.Log(DateTime.Now);
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ExampleClass)), UnityEditor.CanEditMultipleObjects]
    private class Editor : OlegHcpEditor.MethodButtonsEditor
    {

    }
#endif
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/InspectorButton.png)

## ReferenceSelectionAttribute

```csharp
using System;
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeReference]
    [ReferenceSelection]
    private Data _data;

    private void Start()
    {
        _data.DoSomething();
    }
}

[Serializable]
public abstract class Data
{
    public abstract DoSomething();
}

[Serializable]
public class ClassA : Data
{
    public int FieldA;
    
    public override DoSomething()
    {
        Debug.Log(FieldA);
    }
}

[Serializable]
public class ClassB : Data
{
    public float FieldB;
    
    public override DoSomething()
    {
        Debug.Log(FieldB);
    }
}

[Serializable]
public class ClassC : Data
{
    public string FieldC;
    
    public override DoSomething()
    {
        Debug.Log(FieldC);
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/ReferenceSelection.png)

## CertainTypesAttribute

Allows to assign objects only of the specified types in inspector

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [CertainTypes(typeof(GameObject), typeof(MonoBehaviour))]
    private UnityEngine.Object _ref;
}
```

## ClampDiapasonAttribute

```csharp
using OlegHcp.Inspector;
using OlegHcp.NumericEntities;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [ClampDiapason(0f, 1f)]
    private Diapason _diapason;
}
```

## ClampCurveAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    [ClampCurve(0f, 0f, 1f, 1f)]
    private AnimationCurve _curve;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/ClampCurve.png)

## DrawFlagsAttribute

Is used for `IntMask` and `BitList` fields.

```csharp
public enum FlagExample
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
}
```

```csharp
using OlegHcp;
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [DrawFlags(typeof(FlagExample))]
    private IntMask _mask;

    public void DoSomething(FlagExample flag)
    {
        if (_mask[(int)flag])
        {
            // Do something
        }
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/IntMask.png)

## DrawObjectFieldsAttribute

```csharp
using UnityEngine;

public class TestSo : ScriptableObject
{
    [SerializeField]
    private int _foo;
    [SerializeField]
    private string _bar;
}
```

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [DrawObjectFields(true)]
    private TestSo _ref;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/DrawObjectFields.png)

## EnumMenuAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public enum MyEnum
{
    A, B, C, D, E, F,
}

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [EnumMenu]
    private MyEnum _foo;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/EnumMenu.png)

## FolderRequiredAttribute

Allows to assign only folder assets in inspector

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [FolderRequired]
    private UnityEngine.Object _folderAsset;
}
```

## IdentifierAttribute

Generates unique string id

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [Identifier(true)]
    private string _id;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Identifier.png)

## InitToggleAttribute

```csharp
using System;
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeReference]
    [InitToggle]
    private Data _data;

    private void Start()
    {
        if (_data != null)
        {
            // Do something
        }
    }

    [Serializable]
    private class Data
    {
        public int Foo;
        public string Bar;
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/InitToggle1.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/InitToggle2.png)

## InitListAttribute

It is similar to `ReferenceSelectionAttribute`, but uses binding to a custom `enum`.

```csharp
using System;
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeReference]
    [InitList(typeof(ListEnum))]
    private Data _data;

    private void Start()
    {
        _data.DoSomething();
    }
}

public enum ListEnum
{
    [BindSubclass(typeof(ClassA))]
    A,
    [BindSubclass(typeof(ClassB))]
    B,
    [BindSubclass(typeof(ClassC))]
    C,
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/InitList.png)

## LayerFieldAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [LayerField]
    private int _layer;

    private void Awake()
    {
        if (gameObject.layer == _layer)
        {
            // Do something
        }
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/LayerField.png)

## TagFieldAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    [TagField]
    private string _tag;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/TagField.png)

## SortingLayerIDAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [SortingLayerID]
    private int _layer;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/SortingLayerID.png)

## SeparatorAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private string _before;

    [SerializeField]
    [Separator]
    private string _after;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Separator.png)

## TypeNameAttribute

```csharp
using System;
using OlegHcp.Inspector;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    [TypeName(typeof(Data))]
    private string _typename;
}

[Serializable]
public abstract class Data { }

[Serializable]
public class ClassA : Data { }

[Serializable]
public class ClassB : Data { }

[Serializable]
public class ClassC : Data { }
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Typename.png)
