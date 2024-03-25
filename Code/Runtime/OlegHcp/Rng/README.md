## Random Number Generators

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
