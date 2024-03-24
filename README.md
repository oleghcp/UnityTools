# OlegHCP Unity Tools

[![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Logo.png)](https://github.com/oleghcp/UnityTools)

Helpful code stuff for Unity.  
Supports 2019.4 or newer.  
Discussion [thread](https://forum.unity.com/threads/my-unity-tools.641218/).

## Usage

For using the library as a unity package add next line to dependencies in \Packages\manifest.json:

```
"com.oleghcp.unitytools": "https://github.com/oleghcp/UnityTools.git",
```

Also it can be [downloaded](https://github.com/oleghcp/UnityTools/archive/refs/heads/master.zip) manually from github and placed into assets folder.  

Mind that assembly definitions are used.

## Code Overview

### Runtime Code

* [Base stuff](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp). Common tools like `ApplicationUtility`, `BitMask`, `RandomNumberGenerator`, `CameraFitter`, etc.
* [Coroutine running](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Async). Allows to run coroutines in non-MonoBehaviour classes.
* [Collections](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Collections). Some specific collections.
* [.Net Extesions](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/CSharp). Extensions for base .Net api stuff (arrays, collections, strings, etc).
* [Unity Extesions](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Engine). Extensions for base Unity api stuff (game objects, transforms, vectors, etc).
* [Terminal](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/GameConsole). Simple in-game console for commands and messages.
* [Inspector](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Inspector). Attribute classes for Unity inspector.
* [Input/Output](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/IO). A few classes for work with files and paths.
* [Mathematics](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Mathematics). Math tools and structs.
* [Graph editor](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/NodeBased). Tool for creating graphs based on linked nodes. Useful for creating dialogues.
* [Numeric entities](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/NumericEntities). Structs and classes for work with numeric parameters like ranges or character stats.
* [ObjectPool](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Pool). ObjectPool implementation.
* [SignalBus](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Events). SignalBus implementation.
* [PostProcessing](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/PostProcessing). Fog effect for built-in render pipeline
* [Randomization](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Rng). Custom random number generators.
* [Game saving](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/SaveLoad). Save/load system.
* [Projectiles](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Shooting). Projectile implementation.
* [Singletons](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/SingleScripts). Singletons based on `MonoBehaviour` and `ScriptableObject`.
* [Strings](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Strings). Alphanumeric sorting.
* [Timer](https://github.com/oleghcp/UnityTools/tree/master/Code/Runtime/OlegHcp/Timers). Simple timer.

### Editor Code

* [Editor classes](https://github.com/oleghcp/UnityTools/tree/master/Code/Editor). Editor and gui utilities.
* [Extesions](https://github.com/oleghcp/UnityTools/tree/master/Code/Editor/Engine). Extensions for base unity editor stuff.
* [Enum generator](https://github.com/oleghcp/UnityTools/tree/master/Code/Editor/CodeGenerating). Code generating for creating enums. Useful for generating enums based on serialized assets.
* [Node drawer](https://github.com/oleghcp/UnityTools/tree/master/Code/Editor/NodeBased). Custom node drawing for the graph editor.

## Editor Stuff Overview

### Preferences

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Preferences.png)

### Base Menu

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/BaseMenu.png)

| Menu Item | Description |
| - | - |
| Tools/OlegHcp/Addressables/Analysis Results | Converts analysis results from bundle-duplicates to duplicate-bundles. ![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Addressables1.png) |
| Tools/OlegHcp/Assets/Create Scriptable Object Asset | Allows to create assets based on `ScriptableObject` without `CreateAssetMenuAttribute`. ![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/CreateAsset2.png) |
| Tools/OlegHcp/Assets/Find Asset By Guid | Asset searching by guid. |
| Tools/OlegHcp/Assets/Meshes/ | Creates simple mesh assets using mesh generating. |
| Tools/OlegHcp/CaptureScreen/ | Creates screenshots and saves them to a file. |
| Tools/OlegHcp/Code/Generate Layer Set Class | Allows to generate static class with layer and mask constants. ![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/LayerSet1.png) |
| Tools/OlegHcp/Files/Convert Code Files to UTF8 | Converts project code files to UTF8 including shaders. |
| Tools/OlegHcp/Files/Convert Text Files to UTF8 | Converts all text files to UTF8 including code files. |
| Tools/OlegHcp/Files/Find Huge Files | Searches files by given size. |
| Tools/OlegHcp/Folders/Open Project Folder | Opens folder where the project is located. |
| Tools/OlegHcp/Folders/Open Persistent Data Folder | Opens folder which is located at `Application.persistentDataPath`. |
| Tools/OlegHcp/Folders/Remove Empty Folders | Removes empty folders within Assets folder. |

### Custom Transform Editor

Has quick reset buttons and can be switched between local and world view.  
Also has indicators of tool handles if they are switched to center and global value.

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Transform1.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Transform2.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/Transform3.png)

### Asset Context Menu

- **Order Children**. Sorts siblings by name.

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/OrderChildren1.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/OrderChildren2.png)

- **Create Script From Template**. Template can be modified: \your_project\UserSettings\Templates\C#ScriptTemplate.cs.txt.

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/CreateScriptFromTemplate.png)

- **Show Asset Guid**

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/ShowAssetGuid.png)

- **Find References In Project**. Text searching finds addressable references but can be used only if asses tex serialization is used.

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/FindReferencesInProject.png)

- **Create Asset Based On ScriptableObject**

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/CreateAsset1.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/CreateAsset2.png)

- **Destroy Subasset**

![](https://raw.githubusercontent.com/oleghcp/UnityTools/master/_images/DestroySubasset.png)
