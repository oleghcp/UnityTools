using System;
using System.Collections.Generic;
using System.Text;

namespace UnityUtilityEditor.CodeGenerating
{
    public static class EnumGenerator
    {
        public static string Generate(string nameSpace, string enumName,
                                      IEnumerable<(string name, int intValue)> enumValues,
                                      EnumType enumType = EnumType.Int)
        {
            StringBuilder builder = new StringBuilder();

            GeneratingTools.GenerateBanner(builder);

            builder.AppendLine()
                   .Append("namespace ")
                   .AppendLine(nameSpace)
                   .AppendLine("{")
                   .Append(GeneratingTools.TAB)
                   .Append("public enum ")
                   .Append(enumName);

            if (enumType == EnumType.Int)
                builder.AppendLine();
            else
                builder.Append(" : ").AppendLine(enumType.GetName().ToLower());

            builder.Append(GeneratingTools.TAB)
                   .Append('{')
                   .AppendLine();

            foreach (var (name, intValue) in enumValues)
            {
                builder.Append(GeneratingTools.TAB)
                       .Append(GeneratingTools.TAB)
                       .Append(name)
                       .Append(" = ")
                       .Append(intValue)
                       .Append(',')
                       .AppendLine();
            }

            builder.Append(GeneratingTools.TAB)
                   .Append('}')
                   .AppendLine()
                   .Append('}')
                   .AppendLine();

            return builder.ToString();
        }
    }

    public enum EnumType
    {
        SByte,
        Byte,
        Short,
        UShort,
        Int,
        UInt,
        Loung,
        ULong,
    }
}
