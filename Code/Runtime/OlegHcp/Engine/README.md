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
