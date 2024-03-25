## Diapason and DiapasonInt

```csharp
using OlegHcp.NumericEntities;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private Diapason _range = (0f, 10f);

    private void Start()
    {
        Debug.Log(_range.Min);
        Debug.Log(_range.Max);
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Diapason.png)

## RngParam

```csharp
using OlegHcp.NumericEntities;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private RngParam _range = (0f, 10f);

    private void Start()
    {
        float random = _range.GetRandomValue();
    }
}
```

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/RngParam.png)

## AccumFloat/AccumInt

Numeric struct with fields incresed each time when changed.  
Serializable.

```csharp
using OlegHcp.NumericEntities;
using UnityEngine;

public class Example : MonoBehaviour
{
    private AccumInt _money;

    public int Money => _money.Value;
    public int TotallyGot => _money.Got;
    public int TotallySpent => _money.Spent;

    public void AddMoney(int value)
    {
        _money.Add(value);
    }

    public void SpendMoney(int value)
    {
        _money.Spend(value);
    }
}
```

## FilledFloat/FilledInt

Numeric struct which has variable value and threshold.  
For example can be used for implementation of overheat mechanics.  
Serializable.

## SpendingFloat/SpendingInt

Numeric struct which has variable value and max value.  
For example can be used for implementation of health mechanics.  
Serializable.

## ModifiableFloat/ModifiableInt

Can be useful for implementation of character's params.

```csharp
using OlegHcp.NumericEntities;
using UnityEngine;

public class Example : MonoBehaviour
{
    private ModifiableFloat _characterPower;

    private void Awake()
    {
        float value = 5f;
        float minValue = 0f;
        float maxValue = float.PositiveInfinity;
        _characterPower = new ModifiableFloat(value, minValue, maxValue);
    }

    public void AddDebuff(IModifier<float> modifier)
    {
        _characterPower.AddModifier(modifier);
    }

    public void RemoveDebuff(IModifier<float> modifier)
    {
        _characterPower.RemoveModifier(modifier);
    }
}
```

```csharp
using OlegHcp.NumericEntities;
using UnityEngine;

public class PowerDebuff : IModifier<float>
{
    private float _value;
    private ModifierType _modifierType;

    public ModifierType Modification => _modifierType;
    public float Value => _value;

    public PowerDebuff(float value, ModifierType modifierType)
    {
        _value = value;
        _modifierType = modifierType;
    }
}
```
