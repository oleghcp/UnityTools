using System.Runtime.CompilerServices;
using System.Text;
using OlegHcp;
using OlegHcp.CSharp;
using OlegHcp.Strings;
using OlegHcpEditor.Configs;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.CodeGenerating
{
    internal static class LayerSetClassGenerator
    {
        public static string Generate(LayerSetConfig config, SerializedObject tagManager)
        {
            bool needEmptyLine = false;

            StringBuilder builder = new StringBuilder();

            GeneratingTools.GenerateBanner(builder);

            if (config.LayerMasks.Length > 0)
            {
                switch (config.MaskFieldType)
                {
                    case LayerSetConfig.LayerMaskFieldType.LayerMask:
                        builder.AppendLine()
                               .AppendLine("using UnityEngine;");
                        break;

                    case LayerSetConfig.LayerMaskFieldType.Int:
                        break;

                    case LayerSetConfig.LayerMaskFieldType.IntMask:
                        builder.AppendLine()
                               .Append("using ")
                               .Append(nameof(OlegHcp))
                               .Append(';')
                               .AppendLine();
                        break;

                    default:
                        throw new SwitchExpressionException(config.MaskFieldType);
                }
            }

            builder.AppendLine()
                   .Append("namespace ").AppendLine(config.Namespace)
                   .AppendLine("{")
                   .AppendLine("#pragma warning disable IDE1006")
                   .Append(StringUtility.Tab).Append("public static class ").AppendLine(config.ClassName)
                   .Append(StringUtility.Tab).Append('{').AppendLine();

            if (config.TagFields)
            {
                foreach (SerializedProperty tagProperty in tagManager.FindProperty("tags").EnumerateArrayElements())
                {
                    builder.Append(StringUtility.Tab)
                           .Append(StringUtility.Tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("string ")
                           .Append(tagProperty.stringValue.Nicify())
                           .Append("Tag")
                           .Append(" = ")
                           .Append('"')
                           .Append(tagProperty.stringValue)
                           .Append('"')
                           .Append(';')
                           .AppendLine();
                }

                needEmptyLine = true;
            }

            if (config.SortingLayerFields)
            {
                if (needEmptyLine)
                    builder.AppendLine();

                foreach (SortingLayer layer in SortingLayer.layers)
                {
                    builder.Append(StringUtility.Tab)
                           .Append(StringUtility.Tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("int ")
                           .Append(layer.name.Nicify())
                           .Append("Id")
                           .Append(" = ")
                           .Append(layer.id)
                           .Append(';')
                           .AppendLine();
                }

                needEmptyLine = true;
            }

            if (config.LayerFields)
            {
                if (needEmptyLine)
                    builder.AppendLine();

                for (int i = 0; i < BitMask.SIZE; i++)
                {
                    string layerName = LayerMask.LayerToName(i);

                    if (layerName.IsNullOrWhiteSpace())
                        continue;

                    builder.Append(StringUtility.Tab)
                           .Append(StringUtility.Tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("int ")
                           .Append(layerName.Nicify())
                           .Append("Layer")
                           .Append(" = ")
                           .Append(i)
                           .Append(';')
                           .AppendLine();
                }

                if (config.LayerMasks.Length > 0)
                    builder.AppendLine();

                foreach (var maskInfo in config.LayerMasks)
                {
                    builder.Append(StringUtility.Tab)
                           .Append(StringUtility.Tab)
                           .Append("public ");

                    switch (config.MaskFieldType)
                    {
                        case LayerSetConfig.LayerMaskFieldType.LayerMask:
                            builder.Append("static ")
                                   .Append("readonly ")
                                   .Append("LayerMask ");
                            break;

                        case LayerSetConfig.LayerMaskFieldType.Int:
                            builder.Append("const ")
                                   .Append("int ");
                            break;

                        case LayerSetConfig.LayerMaskFieldType.IntMask:
                            builder.Append("static ")
                                   .Append("readonly ")
                                   .Append("IntMask ");
                            break;

                        default:
                            throw new SwitchExpressionException(config.MaskFieldType);
                    }

                    builder.Append(maskInfo.Name.RemoveWhiteSpaces())
                           .Append("Mask")
                           .Append(" = ")
                           .Append(maskInfo.Mask)
                           .Append(';')
                           .Append(" // ");

                    foreach (int layerIndex in BitMask.EnumerateIndices(maskInfo.Mask))
                    {
                        builder.Append(LayerMask.LayerToName(layerIndex).RemoveWhiteSpaces())
                               .Append(" | ");
                    }

                    builder.Remove(builder.Length - 3, 2)
                           .AppendLine();
                }
            }

            builder.Append(StringUtility.Tab)
                   .Append('}')
                   .AppendLine()
                   .Append('}')
                   .AppendLine();

            return builder.ToString();
        }

        private static string Nicify(this string self)
        {
            return self.RemoveWhiteSpaces().Replace('/', '_');
        }
    }
}
