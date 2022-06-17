using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models;

namespace System
{
    public static class ConvertTypes
    {
        public static byte[] ToBytes(object obj)
        {
            try
            {
                if (obj == default)
                    return default;
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error type: '{obj}'\n{ex}");
            }
        }

        public static object ToObject(byte[] data)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
        public static T ToObject<T>(byte[] data) where T : class
        {
            return ToObject(data) as T;
        }
    }
}
