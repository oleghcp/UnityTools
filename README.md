# OlegHCP Unity Tools

Helpful code stuff for Unity.  
For Unity 2019.4 or higher.  
© Oleg Pulkin

## Usage

For using the library as a unity package add next line to \Packages\manifest.json:

```
"dependencies": {
    "com.oleghcp.unitytools": "https://github.com/oleghcp/UnityTools.git",
```

Also it can be downloaded manually from github and placed into assets folder.  
Mind that assembly definitions are used.

## Code Overview

### Runtime Code

### Editor Code

## Editor Stuff Overview

### Preferences

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Preferences.png)

### Base Menu

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/BaseMenu.png)

| Menu Item | Description |
| - | - |
| Tools/OlegHcp/Assets/Create Scriptable Object Asset | Allow to create assets based on `ScriptableObject` without `CreateAssetMenuAttribute`. |
| Tools/OlegHcp/Assets/Find Asset By Guid | Searching assets by guid. |
| Tools/OlegHcp/Assets/Meshes/ | Create simple mesh assets using mesh generating. |
| Tools/OlegHcp/CaptureScreen/ | Creates screenshots and saves them to a file. |
| Tools/OlegHcp/Code/Generate Layer Set Class | Allow to generate static class with set of layers and masks. |
| Tools/OlegHcp/Files/Convert Code Files to UTF8 | Converts project code files to UTF8 including shaders. |
| Tools/OlegHcp/Files/Convert Text Files to UTF8 | Converts all text files to UTF8 including code files. |
| Tools/OlegHcp/Files/Find Huge Files | Searches files by given size. |
| Tools/OlegHcp/Folders/Open Project Folder | Opens folder where the project is located. |
| Tools/OlegHcp/Folders/Open Persistent Data Folder | Opens folder which is located at `Application.persistentDataPath`. |
| Tools/OlegHcp/Folders/Remove Empty Folders | Removes empty folders within Assets folder. |

### Custom Transform Editor

Has quick reset buttons and can be switched between local and world view.  
Also has indicators of tool handles if they are switched to center and global value.

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Transform1.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Transform2.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Transform3.png)

### Asset Context Menu

- **Create Script From Template**. Template can be modified.

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/CreateScriptFromTemplate.png)

- **Show Asset Guid**

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/ShowAssetGuid.png)

- **Find References In Project**. Text searching finds addressable references but can be used only if asses tex serialization is used.

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/FindReferencesInProject.png)

- **Create Asset Based On ScriptableObject**

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/CreateAsset1.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/CreateAsset2.png)

- **Destroy Subasset**

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/DestroySubasset.png)
