# OlegHCP Unity Tools

Helpful code stuff for Unity.  
For Unity 2019.4 or higher.  
© Oleg Pulkin

## Usage

For using the library as a unity package add next line to \Packages\manifest.json:

```json
"dependencies": {
    "com.oleghcp.unitytools": "https://github.com/oleghcp/UnityTools.git",
```

Also it can be downloaded manually from github and placed into assets folder.  
Mind that assembly definitions are used.

## Editor Stuff Overview

### Custom Transform Editor

Has quick reset buttons and can be switched between local and world view.  
Also has indicators of tool handles if they are switched to center and global value.

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Transform1.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Transform2.png)
![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/Transform3.png)

### Asset Context Menu

#### Create Script From Template

Template can be modified

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/CreateScriptFromTemplate.png)

#### Show Asset Guid

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/ShowAssetGuid.png)

#### Find References In Project

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/FindReferencesInProject.png)

#### Create Asset Based On ScriptableObject

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/CreateAsset1.png)

#### Destroy Subasset

![](https://raw.githubusercontent.com/oleghcp/UnityTools/workflow/corrections/_images/DestroySubasset.png)