using System.IO;
using System.Text;
using UnityEditor;

namespace UnityUtilityEditor.CodeGenerating
{
    public static class GeneratingTools
    {
        public static void CreateCsFile(string text, string rootFolder, string enumName, string nameSpace)
        {
            string dirPath = Path.Combine(rootFolder, $"{nameSpace.Replace('.', '/')}");
            Directory.CreateDirectory(dirPath);
            File.WriteAllText(dirPath + $"/{enumName}.cs", text);
            AssetDatabase.Refresh();
        }

        internal static void GenerateBanner(StringBuilder builder)
        {
            builder.AppendLine("///////////////////////////////////")
                   .AppendLine("// Auto-generated (do not edit). //")
                   .AppendLine("///////////////////////////////////");
        }
    }
}
