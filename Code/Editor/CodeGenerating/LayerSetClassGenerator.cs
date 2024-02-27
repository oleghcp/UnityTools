using System.Text;
using OlegHcp;
using OlegHcp.CSharp;
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
                        throw new UnsupportedValueException(config.MaskFieldType);
                }
            }

            builder.AppendLine()
                   .Append("namespace ").AppendLine(config.Namespace)
                   .AppendLine("{")
                   .AppendLine("#pragma warning disable IDE1006")
                   .Append(GeneratingTools.Tab).Append("public static class ").AppendLine(config.ClassName)
                   .Append(GeneratingTools.Tab).Append('{').AppendLine();

            if (config.TagFields)
            {
                SerializedProperty tags = tagManager.FindProperty("tags");

                foreach (var item in tags.EnumerateArrayElements())
                {
                    builder.Append(GeneratingTools.Tab)
                           .Append(GeneratingTools.Tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("string ")
                           .Append(item.stringValue.RemoveWhiteSpaces())
                           .Append("Tag")
                           .Append(" = ")
                           .Append('"')
                           .Append(item.stringValue)
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

                foreach (var layer in SortingLayer.layers)
                {
                    builder.Append(GeneratingTools.Tab)
                           .Append(GeneratingTools.Tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("int ")
                           .Append(layer.name.RemoveWhiteSpaces())
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

                SerializedProperty layers = tagManager.FindProperty("layers");

                foreach (var item in layers.EnumerateArrayElements())
                {
                    if (item.stringValue.IsNullOrWhiteSpace())
                        continue;

                    builder.Append(GeneratingTools.Tab)
                           .Append(GeneratingTools.Tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("int ")
                           .Append(item.stringValue.RemoveWhiteSpaces())
                           .Append("Layer")
                           .Append(" = ")
                           .Append(LayerMask.NameToLayer(item.stringValue))
                           .Append(';')
                           .AppendLine();
                }

                if (config.LayerMasks.Length > 0)
                    builder.AppendLine();

                foreach (var maskInfo in config.LayerMasks)
                {
                    builder.Append(GeneratingTools.Tab)
                           .Append(GeneratingTools.Tab)
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
                            throw new UnsupportedValueException(config.MaskFieldType);
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

            builder.Append(GeneratingTools.Tab)
                   .Append('}')
                   .AppendLine()
                   .Append('}')
                   .AppendLine();

            return builder.ToString();
        }
    }
}
