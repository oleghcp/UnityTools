using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace UnityUtility.IO
{
    public class BinaryFileUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Load<T>(string path)
        {
            return (T)Load(path);
        }

        public static object Load(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return formatter.Deserialize(stream);
            }
        }

        public static void Save(string path, object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, obj);
            }
        }
    }
}
