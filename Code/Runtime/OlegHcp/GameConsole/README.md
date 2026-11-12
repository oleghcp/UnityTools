## Terminal

In-game terminal for commands input and log messages output.  
Supports command substitution by Tab key and keeps list of previous commands.  
Default openning by Backtick key like in Quake but it can be overridden.

![](https://drive.google.com/uc?export=view&id=1vpeBM5O-idMr5Broxefd40uvNOm-aL2R)

```csharp
using OlegHcp.GameConsole;
using UnityEngine;
using UnityEngine.Scripting;

internal class Commands : TerminalCommands
{
    [TerminalCommand, Preserve]
    private bool test_msg()
    {
        Debug.Log("This is a test message.");

        // Return false value if you need to prevent command name output
        // Otherwise return true or make void method instead
        return false;
    }

    [TerminalCommand, Preserve]
    private bool test_error(string[] opt)
    {
        return CommandsHelper.ParseOnOff(opt, run);

        void run(bool value)
        {
            if (value)
            {
                Debug.LogError("This is a test error.");
            }
            else
            {
                Debug.LogWarning("This is a test warning.");
            }
        }
    }
}
```

```csharp
public class Example : MonoBehaviour
{
    private void Start()
    {
        Terminal.CreateTerminal(new Commands(), true);
    }
}
```

![](https://drive.google.com/uc?export=view&id=1ZxADPwvCP1WvFFzbRVHkg2HueGNTzexM)

## Terminal Switch Override

```csharp
internal class Terminal : ITerminalSwitchTrigger
{
    public bool SwitchThisFrame => Input.GetKeyDown(KeyCode.Escape);
}
```
