## InstantiationExtensions

```csharp
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private GameObject _asset;

    private GameObject _instance;

    private void Start()
    {
        _instance = _asset.Install();
        _instance = _asset.Install(transform);
        _instance = _asset.Install(new Vector3());
        _instance = _asset.Install(transform, Vector3.zero, Quaternion.identity, true);
    }
}
```

## TransformExtensions

```csharp
using System.Collections.Generic;
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private Transform _other;

    private void DoSomething1()
    {
        _other.SetParent(gameObject);

        foreach (Transform child in transform.EnumerateChildren(true))
        {
            // Do something
        }

        Transform[] topChildren = transform.GetTopChildren();
        List<Transform> allChildren = transform.GetAllChildren();

        foreach (Transform parent in transform.EnumerateParents())
        {
            // Do something
        }

        Transform otherParent = transform.GetParent(item => item.gameObject.name == "Name");

        Vector3 back = transform.Back();
        Vector3 left = transform.Left();
        Vector3 down = transform.Down();

        _other.Free();

        _other.SetParent(transform, new Vector3());

        _other.SetLocalPositionAndRotation(new Vector3(), Quaternion.identity);
        _other.SetParams(transform);

        _other.IncreaseSiblingIndex();
        _other.DecreaseSiblingIndex();
        _other.MoveSiblingIndex(-2);

        _other.OrderChildren(item => item.gameObject.name);

        transform.DestroyChildren();
    }

    private void DoSomething2()
    {
        RectTransform rectTransform = gameObject.GetRectTransform();

        rectTransform.Move(Vector2.one);

        rectTransform.SetSizeWithCurrentAnchors(Vector2.one * 100f);

        rectTransform.SetAnchor(OlegHcp.RectTransformStretch.MiddleHorizontal, true);
        rectTransform.SetAnchor(TextAnchor.UpperLeft, true);

        rectTransform.SetPivot(TextAnchor.UpperCenter);

        rectTransform.SetLeft(0f);
        rectTransform.SetTop(1f);
    }
}
```

## GameObjectExtensions

```csharp
using System.Collections.Generic;
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private GameObject _other;

    private void Start()
    {
        RectTransform rectTransform = gameObject.GetRectTransform();

        _other.SetParent(gameObject);

        GameObject parent = gameObject.GetParent();

        foreach (GameObject child in gameObject.EnumerateChildren(true))
        {
            // Do something
        }

        GameObject[] topChildren = gameObject.GetTopChildren();
        List<GameObject> allChildren = gameObject.GetAllChildren();

        gameObject.DestroyChildren();
    }
}
```

## LayerMaskExtensions

```csharp
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private LayerMask _mask;

    private void Start()
    {
        if (_mask.HasLayer(4))
        {
            // Do something
        }

        if (_mask.HasLayer("Layer name"))
        {
            // Do something
        }
    }
}
```

## PhysicsExtensions

```csharp
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private LayerMask _mask;

    private void Start()
    {
        Physics.Raycast(new Vector3(), Vector3.right, out RaycastHit hitInfo);

        if (hitInfo.Hit())
        {
            // Do something
        }

        int layer = hitInfo.GetLayer();
        var component1 = hitInfo.GetComponent<MonoBehaviour>();
        var component2 = hitInfo.GetComponentInParent<MonoBehaviour>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        int layer = hit.GetLayer();

        if (_mask.HasLayer(layer))
        {
            // Do something
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layer = collision.GetLayer();
    }
}
```

## CameraExtensions

```csharp
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    private void Start()
    {
        Vector3 size1 = _camera.GetOrthographicSize();

        float remoteness = 100f;
        Vector3 size2 = _camera.GetPerspectiveSize(remoteness);

        float horizontal = _camera.GetHorizontalFov();

        Plane[] planes = _camera.CalculateFrustumPlanes();

        if (_camera.CanSee(renderer))
        {
            // Do something
        }
    }
}
```

## QuaternionExtensions

```csharp
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    private void Start()
    {
        Quaternion rotation = transform.rotation;

        Vector3 right = rotation.Right();
        Vector3 left = rotation.Left();
        Vector3 up = rotation.Up();

        // Deconstruction
        var (x, y, z, w) = rotation;
    }
}
```

## ColorExtensions

```csharp
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private Color _color;

    private void Start()
    {
        Color newColor1 = _color.AlterR(0.5f);
        Color newColor2 = _color.AlterG(0.5f);
        Color newColor3 = _color.AlterB(0.5f);

        // Deconstruction
        var (r, g, b, a) = _color;
    }
}
```

## RectExtensions

```csharp
using OlegHcp;
using OlegHcp.Engine;
using UnityEngine;

public class Example : MonoBehaviour
{
    private void Start()
    {
        RectTransform rectTransform = gameObject.GetRectTransform();
        Rect rect = rectTransform.rect;

        float diagonal = rect.GetDiagonal();

        Rect newRect1 = rect.GetMultiplied(new Vector2(2f, 2f));
        Rect newRect2 = rect.GetExpanded(new Vector2(2f, 2f), new Vector2(0.5f, 0.5f));

        RectInt intRect = RectUtility.MinMaxRectInt(0, 0, 100, 100);
        Rect newRect = intRect.ToRect();

        // Deconstruction
        var (x, y, width, height) = rect;
    }
}
```
