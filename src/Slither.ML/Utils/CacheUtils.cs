using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Slither.ML.Utils
{
    public static class CacheUtils
    {
        public static void InitCache(params (string name, object val)[] vals)
        {
            foreach (var v in vals)
            {
                if (!File.Exists(getObjFilePath(v.name)))
                {
                    SaveObj(v.val, v.name);
                }
            }
        }
        public static void SaveObj(object obj, string name)
        {
            var formatter = new BinaryFormatter();
            using (var fs = new FileStream(getObjFilePath(name), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fs, obj);
            }
        }
        public static T LoadObj<T>(string name)
        {
            T val;
            var filename = getObjFilePath(name);
            var formatter = new BinaryFormatter();
            if (File.Exists(filename))
            {
                using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    val = (T)formatter.Deserialize(fs);
                }
            }
            else
            {
                val = default(T);
                // SaveObj(val, name);
            }
            return val;
        }

        private static string getObjFilePath(string name)
        {
            return $"objects/{name}.bin";
        }
    }
}