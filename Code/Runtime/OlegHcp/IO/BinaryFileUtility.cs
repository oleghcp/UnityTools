using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OlegHcp.IO
{
#if UNITY_6000_4_OR_NEWER
    [System.Obsolete("BinaryFileUtility is deprecated due to the deprecation of BinaryFormatter.")]
#endif
    public class BinaryFileUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Load<T>(string path)
        {
            return (T)Load(path);
        }

        public static object Load(string path)
        {
#pragma warning disable UAC0023
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return formatter.Deserialize(stream);
            }
        }

        public static void Save(string path, object obj)
        {
#pragma warning disable UAC0023
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, obj);
            }
        }
    }
}
