## ApplicationUtility

```csharp
using OlegHcp;

public class MyClass
{
    public MyClass()
    {
        ApplicationUtility.OnTick_Event += OnUpdate;
        ApplicationUtility.OnApplicationQuit_Event += OnApplicationQuit;
    }

    private void OnUpdate(float deltaTime)
    {
        // Per frame code
    }

    private void OnApplicationQuit()
    {
        // Do something on quit
    }
}
```

## AssetRef

```csharp
using OlegHcp;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    private AssetRef<MonoBehaviour> _ref;

    private void Start()
    {
        if (_ref.Type == RefType.Async) //For addressables
        {
            AsyncOperationHandle<GameObject> handle = _ref.AsyncRef.InstantiateAsync();
        }
        else
        {
            MonoBehaviour instance = Instantiate(_ref.Asset);
        }
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/AssetRef.png)

## BitMask

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass
{
    private void MyMethod()
    {
        // Uses flag indices from 0 till 31
        int mask32 = BitMask.CreateMask(0, 1, 3);

        if (BitMask.HasFlag(mask32, 3))
        {
            // Do something
        }

        BitMask.AddFlag(ref mask32, 2);

        int anotherMask32 = BitMask.CreateMask(1, 2, 3);

        // The mask size is 4 in this example because of using 4 flags: 0, 1, 2, 3
        // The maximal mask size is 32
        if (BitMask.Intersects(mask32, anotherMask32, 4))
        {
            // Do something
        }

        BitMask.RemoveFlag(ref anotherMask32, 1);

        // And so on
    }
}
```

## BoolInt

```csharp
public class MyClass : MonoBehaviour
{
    private BoolInt _someBoolState = new BoolInt(true);

    public void AddBoolState()
    {
        _someBoolState++;
    }

    public void RemoveBoolState()
    {
        _someBoolState--;
    }

    private void Update()
    {
        if (_someBoolState)
        {
            // Do something
        }
    }
}
```

## CameraFitter

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/CameraFitter1.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/CameraFitter2.png)

## Colours

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private void Start()
    {
        Color color1 = Colours.Orange;
        Color color2 = Colours.Cyan;
        Color color3 = Colours.Random;
    }
}
```

## ComponentUtility

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private Camera _camera;
    private Light _light;

    private void Start()
    {
        // GameObject with name New Light
        _light = ComponentUtility.CreateInstance<Light>("New Light");

        // Create as a child 
        _camera = ComponentUtility.CreateInstance<Camera>(transform);
    }
}
```

## FpsCounter

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private FpsCounter _fpsCounter = new FpsCounter();

    private void Update()
    {
        _fpsCounter.Update(Time.deltaTime);
    }

    private void OnGUI()
    {
        GUILayout.Label($"FPS: {_fpsCounter.FrameRate}");
    }
}
```

## IntervalChecker

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    [SerializeField]
    private float _interval = 1f;

    private IntervalChecker _intervalChecker;

    private void Awake()
    {
        _intervalChecker = new IntervalChecker(_interval);
    }

    private void Update()
    {
        // Returns true each second
        if (_intervalChecker.SmoothCheckDelta(Time.deltaTime))
        {
            // Do something
        }
    }
}
```

## IntMask

```csharp
using OlegHcp;

public class MyClass
{
    private void MyMethod()
    {
        // Uses flag indices from 0 till 31
        IntMask mask32 = BitMask.CreateMask(0, 1, 3);

        if (mask32[0])
        {
            // Do something
        }

        mask32[2] = true;

        IntMask anotherMask32 = BitMask.CreateMask(1, 2, 3);

        // The mask size is 4 in this example because of using 4 flags: 0, 1, 2, 3
        // The maximal mask size is 32
        if (mask32.Intersects(anotherMask32, 4))
        {
            // Do something
        }

        anotherMask32[1] = false;

        // And so on
    }
}
```

## Messaging

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private TestClass _obj = new TestClass();

    private void Start()
    {
        // Call MyMethod()
        _obj.SendMsg("MyMethod");

        // Call of non-existent method doesn't do anything
        _obj.SendMsg("Qwerty");

        // Messaging uses reflection so try not to abuse it
    }
}

public class TestClass
{
    public void MyMethod()
    {
        // Do something
    }
}
```

## Randomizing

#### Default

```csharp
using System;
using OlegHcp;

public class MyClass
{
    private void DoSomething()
    {
        // Unity built-in generator as an instance
        IRng rng = RandomNumberGenerator.Default;

        // Random numbers
        int randomNum1 = rng.Next(0, 100);
        float randomNum2 = rng.Next(0f, 100f);

        // Random bytes
        Span<byte> bytes = stackalloc byte[10];
        rng.NextBytes(bytes);
    }
}
```

#### Alternative

```csharp
using OlegHcp;
using OlegHcp.Rng;

public class MyClass
{
    private void DoSomething()
    {
        // Based on System.Random
        IRng rng1 = new BaseRng();

        // Based on System.Security.Cryptography.RNGCryptoServiceProvider
        IRng rng2 = new CryptoRng();

        // Based on System.Guid
        IRng rng3 = new GuidRng();

        IRng rng4 = new XorshiftRng();
        IRng rng5 = new Xorshift64Rng();
    }
}
```

#### RngExtensions

```csharp
using OlegHcp;
using OlegHcp.Rng;
using UnityEngine;

public class MyClass
{
    private void DoSomething()
    {
        IRng rng = new BaseRng();

        if (rng.Chance(0.5f))
        {
            // Do something with chance 0.5f
        }

        float[] weights = { 2f, 5f, 100f };
        int index = rng.Random(weights);

        IntMask mask = BitMask.CreateMask(0, 1, 3);
        int flagIndex = rng.RandomFlag((int)mask, 4); // Returns 0, 1 or 3

        // Returns a random number with chance offset to max values.
        float num1 = rng.Ascending(0f, 100f, 1f);

        // Returns a random number with chance offset to min values.
        float num2 = rng.Descending(0f, 100f, 1f);

        // Returns a random number with chance offset to average values.
        float num3 = rng.Average(0f, 100f, 1f);

        // Returns a random number with chance offset to min and max values.
        float num4 = rng.MinMax(0f, 100f, 1f);

        int evenNum = rng.RandomEven(0, 100);

        int oddNum = rng.RandomOdd(0, 100);

        Vector2 point1 = rng.GetOnUnitCircle();
        Vector3 point2 = rng.GetOnUnitSphere();

        Vector2 point3 = rng.GetInsideUnitCircle();
        Vector3 point4 = rng.GetInsideUnitSphere();

        Vector2 point5 = rng.GetInBounds(Rect.MinMaxRect(0f, 0f, 100f, 100f));
        Vector3 point6 = rng.GetInBounds(new Bounds(Vector3.zero, Vector3.one * 100f));

        int stringSize = 25;
        string line = rng.GetAlphanumeric(stringSize);
    }
}
```

## RectUtility

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private void DoSomething1()
    {
        RectTransform rectTransform = transform as RectTransform;

        Vector2 anchor = RectUtility.GetAnchor(TextAnchor.UpperCenter);
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
    }

    private void DoSomething2()
    {
        RectTransform rectTransform = transform as RectTransform;

        Rect rect = RectUtility.GetAnchor(RectTransformStretch.MiddleHorizontal, out Vector2 pivot);

        rectTransform.anchorMin = rect.min;
        rectTransform.anchorMax = rect.max;
        rectTransform.pivot = pivot;
    }

    private void DoSomething()
    {
        // It is like Rect.MinMaxRect();
        RectInt rectInt = RectUtility.MinMaxRectInt(0, 0, 100, 100);
    }
}
```

## RenderSorter

![](https://github.com/oleghcp/UnityTools/blob/master/_images/RenderSorter.png?raw=true)

## ScreenUtility

```csharp
public static class ScreenUtility
{
    public static Vector2 ScreenToUi(in Vector2 screenPosition, Vector2 canvasSize);
    public static Vector2 ScreenToEnvelopeUi(in Vector2 screenPosition, Vector2 canvasSize, Vector2 canvasReferenceResolution);
    public static Vector2 WorldToUi(in Vector3 worldPos, Vector2 canvasSize, Camera camera);
    public static Vector2 WorldToEnvelopeUi(in Vector3 worldPos, Vector2 canvasSize, Vector2 canvasReferenceResolution, Camera camera);
    public static float GetPathScreenFactor(float cameraOrthographicSize);
    public static float GetPathScreenFactor(float verticalFieldOfView, float distance);
    public static Vector2 GetOrthographicSize(float cameraOrthographicSize);
    public static Vector2 GetPerspectiveSize(float verticalFieldOfView, float remoteness);
    public static float GetAspectAngle(float fieldOfView, float aspectRatio);
}
```

## TransformUtility

```csharp
using System.Collections.Generic;
using System.Linq;
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private void Start()
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        children = children.OrderBy(item => item.name)
                           .ToList();

        TransformUtility.OrderSiblingsByList(children);
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/OrderSiblingsByList.png)

```csharp
using System.Collections.Generic;
using System.Linq;
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    private void Start()
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        Vector3[] positions = children.Select(item => item.position)
                                      .ToArray();

        TransformUtility.AlignPositions(positions, transform.position, Vector3.right, 0.5f);

        for (int i = 0; i < positions.Length; i++)
        {
            children[i].position = positions[i];
        }
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/AlignPositions.png)

## UiMonoBehaviour

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : UiMonoBehaviour
{
    private void Start()
    {
        // Inherited property
        rectTransform.anchoredPosition = Vector3.zero;
    }
}
```

## UnityObjectUtility

```csharp
using OlegHcp;
using UnityEngine;

public class MyClass : MonoBehaviour
{
    public void DoSomething1(GameObject obj)
    {
        // Check whether it's asset reference
        if (!UnityObjectUtility.IsAsset(obj))
            Destroy(obj);
    }

    public void DoSomething2(ISomeInterface obj)
    {
        // Object can be instance of MonoBehaviour but null checking is useless because of interface reference
        if (!UnityObjectUtility.IsNullOrDead(obj))
        {
            // Do something
        }
    }
}
```

## XmlUtility

```csharp
using OlegHcp;

public class MyClass
{
    public void DoSomething(int[] nums)
    {
        string xml = XmlUtility.ToXml(nums);

        int[] nums2 = XmlUtility.FromXml<int[]>(xml);
    }
}
```
