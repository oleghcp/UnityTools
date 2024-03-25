## BitList

Based on `System.Collections.BitArray` and supports Unity serialization

```csharp
using OlegHcp.Collections;

public class MyClass
{
    private void MyMethod()
    {
        // Uses flag indices from 0 till 31
        BitList bits = new BitList(new bool[] { true, false, true, false, true });

        if (bits[0])
        {
            // Do something
        }

        bits[1] = true;

        BitList anotherBits = new BitList(new bool[] { false, true, false, true, false });

        // The mask size is 4 in this example because of using 4 flags: 0, 1, 2, 3
        // The maximal mask size is 32
        if (bits.Intersects(anotherBits))
        {
            // Do something
        }

        anotherBits[1] = false;

        // And so on
    }
}
```

## Tracker

```csharp
using OlegHcp.Collections;
using UnityEngine;

public class GameHud : MonoBehaviour
{
    private Character _character;
    private Tracker _tracker;

    private void Awake()
    {
        _tracker = new Tracker();
        _tracker.AddNodeForValueType(() => _character.Health, UpdateHealthBar);
        _tracker.AddNodeForValueType(() => _character.Money, UpdateMoneyView);
        _tracker.ForceInvoke();
    }

    private void LateUpdate()
    {
        _tracker.Refresh();
    }

    private void UpdateHealthBar(int health)
    {
        // Update healthbar
    }

    private void UpdateMoneyView(int money)
    {
        // Update money label
    }
}
```
