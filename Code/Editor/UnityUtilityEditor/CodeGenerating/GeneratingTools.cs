using System.IO;
using System.Text;
using UnityEditor;

namespace UnityUtilityEditor.CodeGenerating
{
    public static class GeneratingTools
    {
        public const string TAB = "    ";

        public static void CreateCsFile(string text, string rootFolder, string className, string nameSpace, bool refreshAssets = true)
        {
            string dirPath = Path.Combine(rootFolder, $"{nameSpace.Replace('.', '/')}");
            Directory.CreateDirectory(dirPath);
            File.WriteAllText(dirPath + $"/{className}.cs", text);
            if (refreshAssets)
                AssetDatabase.Refresh();
        }

        public static void GenerateBanner(StringBuilder builder)
        {
            builder.AppendLine("///////////////////////////////////")
                   .AppendLine("// Auto-generated (do not edit). //")
                   .AppendLine("///////////////////////////////////");
        }
    }
}
