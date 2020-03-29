#if VS_FILE_HOOK
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
        XDocument document = XDocument.Parse(content);

        bool checkComdition(XElement item)
        {
            return item.Name.LocalName == "Reference" &&
                   item.Attribute("Include")?.Value == "Boo.Lang";
        }

        foreach (XElement element in document.Root.Elements())
        {
            XElement booReferenceElement = element.Elements().FirstOrDefault(checkComdition);

            if (booReferenceElement != null)
            {
                booReferenceElement.Remove();
                break;
            }
        }

        const string warns = "CS0649, IDE0044";
        XName xName = document.Root.GetDefaultNamespace() + "NoWarn";

        foreach (XElement element in document.Root.Elements())
        {
            if (element.Name.LocalName == "PropertyGroup")
            {
                XAttribute attribute = element.Attribute("Condition");

                if (attribute != null)
                {
                    string value = attribute.Value;

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

        using (Utf8StringWriter writer = new Utf8StringWriter())
        {
            document.Save(writer);
            content = writer.ToString();
        }

        return content;
    }
}
#endif
