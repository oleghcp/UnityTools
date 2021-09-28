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
            const string tab = "    ";
            StringBuilder builder = new StringBuilder();

            GeneratingTools.GenerateBanner(builder);

            builder.AppendLine()
                   .Append("namespace ")
                   .AppendLine(nameSpace)
                   .AppendLine("{")
                   .Append(tab)
                   .Append("public enum ")
                   .Append(enumName);

            if (enumType == EnumType.Int)
                builder.AppendLine();
            else
                builder.Append(" : ").AppendLine(enumType.GetName().ToLower());

            builder.Append(tab)
                   .Append('{')
                   .AppendLine();

            foreach (var (name, intValue) in enumValues)
            {
                builder.Append(tab)
                       .Append(tab)
                       .Append(name)
                       .Append(" = ")
                       .Append(intValue)
                       .Append(',')
                       .AppendLine();
            }

            builder.Append(tab)
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
