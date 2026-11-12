## Projectile

![](https://drive.google.com/uc?export=view&id=1GaPLMQbzSGM4qgYeoIIjfCNLonJcofdB)

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
        // Per frame function which called before motion and collision check
    }

    void IProjectileEventListener.PostUpdate(bool isPlaying)
    {
        // Per frame function which called after motion and collision check
    }

    void IProjectileEventListener.OnHitModified(in RaycastHit hitInfo, float previousSpeed, in Vector3 previousDirection, HitReactionType reaction)
    {
        // Called on modified hit: ricochet or moving through target object
    }

    void IProjectileEventListener.OnHitFinal(in RaycastHit hitInfo, in Vector3 velocity, float speed)
    {
        // Called on final hit without modifications
    }

    void IProjectileEventListener.OnTimeOut()
    {
        // Called when timer elapsed
    }
}
```

## Projectile2D

Same as Projectile but in two dimensions  

Warning: for correct work of `Projectile2D` starting in colliders queries should be disabled:

![](https://drive.google.com/uc?export=view&id=1S0XIBsfeecQGuYRPKMPIX7TYN83LyH8D)
