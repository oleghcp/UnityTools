﻿using System.Xml.Linq;
using UnityEditor;

namespace UnityUtilityEditor
{
    public class CsprojFilePostprocessor : AssetPostprocessor
    {
        public static string OnGeneratedCSProject(string path, string content)
        {
            XDocument document = XDocument.Parse(content);

            string warns = EditorPrefs.GetString(PrefsKeys.SUPPRESSED_WARNINGS_IN_IDE, string.Empty);
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
                                subElement.Value = warns;
                        }
                    }
                }
            }

            return document.ToString();
        }
    }
}
