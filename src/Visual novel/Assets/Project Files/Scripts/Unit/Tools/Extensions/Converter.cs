using System.IO;
using System.Runtime.Serialization;

namespace Tools.Extensions
{
    public static class Converter
    {
        public static byte[] SerializeToByteArray<T>(this T obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }

            using var ms = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(ms, obj);
            return ms.ToArray();
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return default;
            }

            using var memStream = new MemoryStream(byteArray);
            var serializer = new DataContractSerializer(typeof(T));
            var obj = (T)serializer.ReadObject(memStream);
            return obj;
        }
    }
}