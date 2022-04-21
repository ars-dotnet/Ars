using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ars.Commom.Tool.Serializer
{
    public class ArsSerializer : IArsSerializer
    {
        public T DeSerialize<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes);
        }

        public byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize<T>(obj);
        }

        public string ConvertToJson(byte[] bytes)
        {
            return MessagePackSerializer.ConvertToJson(bytes);
        }

        public string SerializeToJson<T>(T obj)
        {
            return MessagePackSerializer.SerializeToJson(obj);
        }

        public byte[] ConvertFromJson(string json)
        {
            return MessagePackSerializer.ConvertFromJson(json);
        }
    }
}
