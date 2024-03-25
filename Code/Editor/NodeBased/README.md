## NodeDrawer

```csharp
using OlegHcp;
using OlegHcpEditor.NodeBased;
using UnityEditor;
using UnityEngine;

[CustomNodeDrawer(typeof(ExampleNode))]
public class ExampleNodeDrawer : NodeDrawer
{
    protected override float GetHeight(SerializedProperty property)
    {
        return base.GetHeight(property) * 2f;
    }

    protected override Color GetHeaderColor(bool rootNode)
    {
        return rootNode ? Colours.Sky : Colours.Lime;
    }

    protected override void OnGui(SerializedProperty property, float width)
    {
        EditorGUIUtility.labelWidth = width * 0.5f;
        EditorGUILayout.LabelField("Overridden", "Qwerty");
        GUILayout.Button("Button");
    }
}
```
