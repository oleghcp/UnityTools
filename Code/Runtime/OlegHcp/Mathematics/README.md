## MathExtensions

Extension methods for numbers

```csharp
using OlegHcp.Mathematics;

public class Example
{
    private void MyMethod()
    {
        float value = -5f;

        float newValue = value.Abs()
                              .Pow(5f)
                              .Sqrt()
                              .ToInt(RoundingWay.Ceiling)
                              .ToFloat();
    }
}
```

## MathUtility

Some useful math functions

```csharp
using OlegHcp.Mathematics;
using UnityEngine;

public class Example
{
    private void MyMethod1()
    {
        Vector3 vector = Vector3.right;
        Vector3 axis = Vector3.forward;
        float angle = 90f;

        Vector3 rotated = MathUtility.RotateVector(vector, axis, angle);
    }

    private void MyMethod2()
    {
        Vector3 vector = Vector3.one;
        Vector3 normalized = MathUtility.Normalize(vector, out float prevMagnitude);
        Debug.Log(prevMagnitude);
    }

    private void MyMethod2(Vector3 a, Vector3 b)
    {
        float precision = 0.1f;

        if (MathUtility.Equals(a, b, precision))
        {
            // Do something
        }
    }
}
```

## Circle and Sphere

```csharp
using OlegHcp.Mathematics;
using UnityEngine;

public class Example
{
    public void MyMethod(Sphere a, Sphere b)
    {
        if (a.Overlaps(b))
        {
            // Do something
        }

        if (a.Raycast(new Ray(Vector3.zero, Vector3.right), out Vector3 point) == RaycastResult.Hit)
        {
            // Do something
        }

        Debug.Log(b.GetVolume());
    }
}
```

## Arc2/Arc3

Represents motion of a body thrown at an angle to the horizontal.

```csharp
using OlegHcp.Mathematics;
using UnityEngine;

public class Example : MonoBehaviour
{
    private Arc3 _arc;

    private void Awake()
    {
        _arc = new Arc3(Vector3.one.normalized, 30f, Physics.gravity.magnitude);
    }

    private void Update()
    {
        Vector3 newPosition = _arc.Evaluate(Time.time);
    }
}
```

## Curves

Implementation of Bezier curve and Catmull–Rom spline. Represented by `Bezier2`, `Bezier3`, `CatmullRom2`, `CatmullRom3` classes.

```csharp
using System.Collections;
using OlegHcp.Mathematics;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private Vector3[] _points;
    [SerializeField]
    private float _speed;

    private ICurve<Vector3> _curve;

    private void Awake()
    {
        _curve = new Bezier3(_points);
    }

    private void Start()
    {
        StartCoroutine(GetRoutine());
    }

    private IEnumerator GetRoutine()
    {
        float ratio = 0f;

        while (ratio < 1f)
        {
            ratio += Time.deltaTime * _speed;

            transform.position = _curve.Evaluate(ratio);

            yield return null;
        }
    }
}
```
