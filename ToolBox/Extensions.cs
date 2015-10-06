using System;
using System.IO;
using System.Text;
using Windows.Storage.Streams;

namespace WebSocket4UWP.ToolBox
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class Extensions
    {

        public static byte[] CopyOfRange(this byte[] original, int from, int to)
        {
            int newLength = to - from;
            if (newLength < 0)
                throw new Exception(from + " > " + to);
            byte[] copy = new byte[newLength];
            Array.Copy(original, from, copy, 0,
                             Math.Min(original.Length - from, newLength));
            return copy;
        }

        public static byte[] StringToBytes(this string str)
        {
            try
            {
                return Encoding.UTF8.GetBytes(str);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static string BytesToString(this byte[] buffer)
        {
            try
            {
                return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// 将 Stream 转成 byte[]

        public static byte[] StreamToBytes(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// 将 byte[] 转成 Stream

        public static Stream BytesToStream(this byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        public static int CodePointAt(this string value, int index)
        {
            return CodePointAt(value.ToCharArray(), index);
        }

        public static int CodePointAt(this char[] value, int index)
        {
            if ((index < 0) || (index >= value.Length))
            {
                throw new IndexOutOfRangeException(index + "");
            }
            return Character.CodePointAtImpl(value, index, value.Length);
        }

        public static byte[] BufferToBytes(this IBuffer buf)
        {
            using (var dataReader = DataReader.FromBuffer(buf))
            {
                var bytes = new byte[buf.Capacity];
                dataReader.ReadBytes(bytes);
                return bytes;
            }
        }



        public static IBuffer BytesToBuffer(this byte[] bytes)
        {
            using (var dataWriter = new DataWriter())
            {

                dataWriter.WriteBytes(bytes);

                return dataWriter.DetachBuffer();

            }

        }
    }
}
