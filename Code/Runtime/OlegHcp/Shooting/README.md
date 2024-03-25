## Projectile

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Projectile.png)

```csharp
using OlegHcp.Shooting;
using UnityEngine;

public class ExampleBullet : MonoBehaviour, IProjectileEventListener
{
    [SerializeField]
    private Projectile _projectile;

    private void Awake()
    {
        _projectile.Listener = this;
    }

    public void Launch(in Vector3 direction, float speed)
    {
        _projectile.Play(direction * speed);
    }

    void IProjectileEventListener.PreUpdate(bool isPlaying)
    {
        // Per frame function which called before motion and collision checking
    }

    void IProjectileEventListener.PostUpdate(bool isPlaying)
    {
        // Per frame function which called after motion and collision checking
    }

    void IProjectileEventListener.OnHitReflected(in RaycastHit hitInfo, in Vector3 previousVelocity, float previousSpeed)
    {
        // Play visual effect or something else
    }

    void IProjectileEventListener.OnHitFinal(in RaycastHit hitInfo, in Vector3 velocity, float speed)
    {
        // Destroy or return to object pool
    }

    void IProjectileEventListener.OnTimeOut()
    {
        // Destroy or return to object pool
    }
}
```

## Projectile2D

Same as Projectile but in two dimensions