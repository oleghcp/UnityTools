using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtilityEditor.Configs;

namespace UnityUtilityEditor.CodeGenerating
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
                               .AppendLine("using UnityUtility;");
                        break;

                    default:
                        throw new UnsupportedValueException(config.MaskFieldType);
                }
            }

            builder.AppendLine()
                   .Append("namespace ").AppendLine(config.Namespace)
                   .AppendLine("{")
                   .Append(GeneratingTools.TAB).Append("public static class ").AppendLine(config.ClassName)
                   .Append(GeneratingTools.TAB).Append('{').AppendLine();

            if (config.TagFields)
            {
                SerializedProperty tags = tagManager.FindProperty("tags");

                foreach (var item in tags.EnumerateArrayElements())
                {
                    builder.Append(GeneratingTools.TAB)
                           .Append(GeneratingTools.TAB)
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
                    builder.Append(GeneratingTools.TAB)
                           .Append(GeneratingTools.TAB)
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

                    builder.Append(GeneratingTools.TAB)
                           .Append(GeneratingTools.TAB)
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
                    builder.Append(GeneratingTools.TAB)
                           .Append(GeneratingTools.TAB)
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

            builder.Append(GeneratingTools.TAB)
                   .Append('}')
                   .AppendLine()
                   .Append('}')
                   .AppendLine();

            return builder.ToString();
        }
    }
}
