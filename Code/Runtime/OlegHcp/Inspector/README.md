## InspectorButtonAttribute

```csharp
using OlegHcp.Inspector;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private int _foo;
    [SerializeField]
    private string _bar;

    [InspectorButton("Do something", 30f)]
    private void MyMethod()
    {

    }
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
        if (_data is ClassA a)
        {

        }
        else if (_data is ClassB b)
        {

        }
        else if (_data is ClassC c)
        {

        }
    }
}

[Serializable]
public abstract class Data
{

}

[Serializable]
public class ClassA : Data
{
    public int FiledA;
}

[Serializable]
public class ClassB : Data
{
    public float FiledB;
}

[Serializable]
public class ClassC : Data
{
    public string FiledC;
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

## DrawFlagsAttribute

Is used for IntMask and BitList fields.

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
        if (_data is ClassA a)
        {

        }
        else if (_data is ClassB b)
        {

        }
        else if (_data is ClassC c)
        {

        }
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

[Serializable]
public class Data
{

}

[Serializable]
public class ClassA : Data
{
    public int FiledA;
}

[Serializable]
public class ClassB : Data
{
    public float FiledB;
}

[Serializable]
public class ClassC : Data
{
    public string FiledC;
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
public abstract class Data
{

}

[Serializable]
public class ClassA : Data
{
    public int FiledA;
}

[Serializable]
public class ClassB : Data
{
    public float FiledB;
}

[Serializable]
public class ClassC : Data
{
    public string FiledC;
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Typename.png)
