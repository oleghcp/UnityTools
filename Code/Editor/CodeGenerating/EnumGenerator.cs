using System.Collections.Generic;
using System.Linq;
using System.Text;
using OlegHcp.CSharp;

namespace OlegHcpEditor.CodeGenerating
{
    public static class EnumGenerator
    {
        public static string Generate(string nameSpace, string enumName,
                                      IEnumerable<string> enumValues,
                                      EnumType enumType = EnumType.Int)
        {
            int i = 0;
            return Generate(nameSpace, enumName, enumValues.Select(item => (item, i++)), enumType);
        }

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
                   .Append(GeneratingTools.Tab)
                   .Append("public enum ")
                   .Append(enumName);

            if (enumType == EnumType.Int)
                builder.AppendLine();
            else
                builder.Append(" : ").AppendLine(enumType.GetName().ToLower());

            builder.Append(GeneratingTools.Tab)
                   .Append('{')
                   .AppendLine();

            foreach (var (name, intValue) in enumValues)
            {
                builder.Append(GeneratingTools.Tab)
                       .Append(GeneratingTools.Tab)
                       .Append(name)
                       .Append(" = ")
                       .Append(intValue)
                       .Append(',')
                       .AppendLine();
            }

            builder.Append(GeneratingTools.Tab)
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
