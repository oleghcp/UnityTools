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
    [SerializeField]
    private Character _character;
    [SerializeField]
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

## StateMachine

```csharp
using OlegHcp.Collections;
using UnityEngine;

public class SMState : IState
{
    private string _name;

    public string Name => _name;

    public SMState(string name)
    {
        _name = name;
    }

    void IState.OnEnter()
    {
        Debug.Log($"On enter {_name}");
    }

    void IState.OnExit()
    {
        Debug.Log($"On exit {_name}");
    }
}
```

```csharp
using OlegHcp.Collections;

public class SMCondition : ICondition<int>
{
    private int _code;

    public SMCondition(int code)
    {
        _code = code;
    }

    public bool Check(int data)
    {
        return _code == data;
    }
}
```

```csharp
using OlegHcp.Collections;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    private StateMachine<SMState, int> _machine = new StateMachine<SMState, int>();

    private void Awake()
    {
        SMState a = new SMState("A");
        SMState b = new SMState("B");

        _machine.SetAsStart(a); //Set a as the beginning state
        _machine.AddTransition(a, b, new SMCondition(0)); //Transition a -> b
        _machine.AddTransition(b, a, new SMCondition(0)); //Transition b -> a
        _machine.AddTransition(b, new SMCondition(1)); //Exit from b

        _machine.OnStateChanged_Event += (state1, state2) =>
        {
            Debug.Log($"Transition: {state1.Name} -> {state2.Name}");
        };

        _machine.OnFinished_Event += state =>
        {
            Debug.Log($"Finish from {state.Name}");
        };
    }

    private void Start()
    {
        _machine.Run();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _machine.CheckConditions(0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _machine.CheckConditions(1);
        }
    }
}
```

Also the state machine can be configured with `OlegHcp.NodeBased.Graph<TNode>`.

```csharp
public class ExampleClass : MonoBehaviour
{
    [SerializeField]
    private CustomGraph _graph;

    private StateMachine<SMState, int> _machine;

    private void Awake()
    {
        _machine = new StateMachine<SMState, int>(_graph);
    }
}
```

It requires implementation of `CreateState` and `CreateCondition` methods in the Node class.

```csharp
[Serializable]
public class StateNode : Node<StateNode>, IState
{
    public override TState CreateState<TState>()
    {
         return this as TState;
    }

    public override ICondition<TData> CreateCondition<TData>(TransitionInfo<StateNode> transition)
    {
        //Node condition has to implement OlegHcp.Collections.ICondition<TData> in this case
        //Or you can create a wrapper for the transition parameter
        return (ICondition<TData>)transition.Condition;
    }

    void IState.OnEnter()
    {

    }

    void IState.OnExit()
    {

    }
}
```
