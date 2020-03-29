#if VS_FILE_HOOK
using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;
using SyntaxTree.VisualStudio.Unity.Bridge;

[UnityEditor.InitializeOnLoad]
public class ProjectFileHook
{
    class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    static ProjectFileHook()
    {
        ProjectFilesGenerator.ProjectFileGeneration += HandleNewFile;
    }

    private static string HandleNewFile(string name, string content)
    {
        var document = XDocument.Parse(content);

        var checkComdition = new Func<XElement, bool>(item =>
        {
            return item.Name.LocalName == "Reference" &&
                   item.Attribute("Include")?.Value == "Boo.Lang";
        });

        foreach (var element in document.Root.Elements())
        {
            var booReferenceElement = element.Elements().FirstOrDefault(checkComdition);

            if (booReferenceElement != null)
            {
                booReferenceElement.Remove();
                break;
            }
        }

        const string warns = "CS0649, IDE0044";
        var xName = document.Root.GetDefaultNamespace() + "NoWarn";

        foreach (var element in document.Root.Elements())
        {
            if (element.Name.LocalName == "PropertyGroup")
            {
                var attribute = element.Attribute("Condition");

                if (attribute != null)
                {
                    var value = attribute.Value;

                    if (value.Contains("Debug") || value.Contains("Release"))
                    {
                        XElement subElement = element.Element(xName);

                        if (subElement == null)
                            element.Add(new XElement(xName, warns));
                        else
                            subElement.Value += ", " + warns;
                    }
                }
            }
        }

        using (var writer = new Utf8StringWriter())
        {
            document.Save(writer);
            content = writer.ToString();
        }

        return content;
    }
}
#endif
