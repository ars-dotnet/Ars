using System;
using System.Collections.Generic;
using System.Text;

namespace Ars.Commom.Tool.Serializer
{
    public interface IArsSerializer
    {
        /// <summary>
        /// 序列化对象为byte数组
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        byte[] Serialize<T>(T obj);

        /// <summary>
        /// 反序列化byte数组为对象
        /// </summary>
        /// <typeparam name="T">要反序列化的对象类型</typeparam>
        /// <param name="bytes">源byte[]数组</param>
        T DeSerialize<T>(byte[] bytes);

        /// <summary>
        /// 反序列化byte数组为json字符串
        /// </summary>
        /// <param name="bytes">要反序列化的byte[]数组</param>
        string ConvertToJson(byte[] bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        string SerializeToJson<T>(T obj);
    }
}
