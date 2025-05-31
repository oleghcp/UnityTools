using System.Collections.Generic;
using System.Linq;
using System.Text;
using OlegHcp.CSharp;
using OlegHcp.Strings;

namespace OlegHcpEditor.CodeGenerating
{
    public static class EnumGenerator
    {
        public static string Generate(string nameSpace, string enumName,
                                      IEnumerable<string> enumValues,
                                      EnumType enumType = EnumType.Int,
                                      bool isPublic = true)
        {
            int i = 0;
            return Generate(nameSpace, enumName, enumValues.Select(item => (item, i++)), enumType, isPublic);
        }

        public static string Generate(string nameSpace, string enumName,
                                      IEnumerable<(string name, int intValue)> enumValues,
                                      EnumType enumType = EnumType.Int,
                                      bool isPublic = true)
        {
            StringBuilder builder = new StringBuilder();

            GeneratingTools.GenerateBanner(builder);

            builder.AppendLine()
                   .Append("namespace ")
                   .AppendLine(nameSpace)
                   .AppendLine("{")
                   .Append(StringUtility.Tab)
                   .Append(isPublic ? "public " : "internal ")
                   .Append("enum ")
                   .Append(enumName);

            if (enumType == EnumType.Int)
                builder.AppendLine();
            else
                builder.Append(" : ").AppendLine(enumType.GetName().ToLower());

            builder.Append(StringUtility.Tab)
                   .Append('{')
                   .AppendLine();

            foreach (var (name, intValue) in enumValues)
            {
                builder.Append(StringUtility.Tab)
                       .Append(StringUtility.Tab)
                       .Append(name)
                       .Append(" = ")
                       .Append(intValue)
                       .Append(',')
                       .AppendLine();
            }

            builder.Append(StringUtility.Tab)
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
        Long,
        ULong,
    }
}
