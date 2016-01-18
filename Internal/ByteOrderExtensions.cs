using System;
using Windows.Storage.Streams;

namespace WebSocket4UWP.Internal
{
    internal static class ByteOrderExtensions
    {
        public static bool IsHostOrder(this ByteOrder order)
        {
            // true : !(true ^ true)  or !(false ^ false)
            // false: !(true ^ false) or !(false ^ true)
            return !(BitConverter.IsLittleEndian ^ (order == ByteOrder.LittleEndian));
        }
    }
}