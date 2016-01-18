using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using WebSocket4UWP.Internal;

namespace WebSocket4UWP.ToolBox
{
    public static class ByteExtensions
    {
        public static byte[] CopyOfRange(this byte[] original, int from, int to)
        {
            int newLength = to - from;
            if (newLength < 0)
                throw new Exception(from + " > " + to);
            byte[] copy = new byte[newLength];
            Array.Copy(original, from, copy, 0, Math.Min(original.Length - from, newLength));
            return copy;
        }

        public static IBuffer ToBuffer(this IRandomAccessStream randomStream)
        {
            Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(randomStream.GetInputStreamAt(0));
            var memoryStream = new MemoryStream();
            if (stream != null)
            {
                byte[] bytes = stream.ToByteArray();
                if (bytes != null)
                {
                    var binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write(bytes);
                }
            }
            return WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(memoryStream, 0, (int)memoryStream.Length);
        }

        public static IBuffer ToBuffer(this string str)
        {
            using (var dataWriter = new DataWriter())
            {
                dataWriter.WriteString(str);
                return dataWriter.DetachBuffer();
            }
        }

        public static IBuffer ToBuffer(this byte[] bytes)
        {
            using (var dataWriter = new DataWriter())
            {
                dataWriter.WriteBytes(bytes);
                return dataWriter.DetachBuffer();
            }
        }

        public static int ToBit(this bool value)
        {
            return value ? 1 : 0;
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public static byte[] ToByteArray(this IBuffer buf)
        {
            using (var dataReader = DataReader.FromBuffer(buf))
            {
                var bytes = new byte[buf.Capacity];
                dataReader.ReadBytes(bytes);
                return bytes;
            }
        }

        public static byte[] ToByteArray(this bool value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this char value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this double value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this float value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this int value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this long value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this short value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this uint value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this ulong value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static byte[] ToByteArray(this ushort value, ByteOrder byteOrder)
        {
            var bytes = BitConverter.GetBytes(value);
            return AdjustByteOrder(bytes, byteOrder);
        }

        public static ushort ToUInt16(this byte[] src, ByteOrder srcOrder)
        {
            return BitConverter.ToUInt16(src.ToHostOrder(srcOrder), 0);
        }

        public static ulong ToUInt64(this byte[] src, ByteOrder srcOrder)
        {
            return BitConverter.ToUInt64(src.ToHostOrder(srcOrder), 0);
        }

        public static byte[] ToHostOrder(this byte[] src, ByteOrder srcOrder)
        {
            if (src == null)
                return null;

            return src.Length > 1 && !srcOrder.IsHostOrder()
                   ? src.CopyReverse()
                   : src;
        }

        public static byte[] CopyReverse(this byte[] src)
        {
            if (src == null)
                return null;

            var len = src.Length;

            var reverse = new byte[len];
            for (var i = 0; i <= len / 2; ++i)
            {
                var t = src[i];
                var o = len - i - 1;
                reverse[i] = src[o];
                reverse[o] = t;
            }

            return reverse;
        }

        private static byte[] AdjustByteOrder(byte[] bytes, ByteOrder byteOrder)
        {
            if (!byteOrder.IsHostOrder())
                Array.Reverse(bytes);
            return bytes;
        }
    }
}
