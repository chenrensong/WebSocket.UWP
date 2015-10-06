namespace WebSocket4UWP.ToolBox
{
    public class Character
    {

        public const int MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;
        public const char MAX_LOW_SURROGATE = '\uDFFF';
        public const char MIN_HIGH_SURROGATE = '\uD800';
        public const char MIN_LOW_SURROGATE = '\uDC00';
        public const char MAX_HIGH_SURROGATE = '\uDBFF';
        public static bool IsHighSurrogate(char ch)
        {
            // Help VM constant-fold; MAX_HIGH_SURROGATE + 1 == MIN_LOW_SURROGATE
            return ch >= MIN_HIGH_SURROGATE && ch < (MAX_HIGH_SURROGATE + 1);
        }

        public static bool IsLowSurrogate(char ch)
        {
            return ch >= MIN_LOW_SURROGATE && ch < (MAX_LOW_SURROGATE + 1);
        }

        public static int ToCodePoint(char high, char low)
        {
            // Optimized form of:
            // return ((high - MIN_HIGH_SURROGATE) << 10)
            //         + (low - MIN_LOW_SURROGATE)
            //         + MIN_SUPPLEMENTARY_CODE_POINT;
            return ((high << 10) + low) + (MIN_SUPPLEMENTARY_CODE_POINT
                                           - (MIN_HIGH_SURROGATE << 10)
                                           - MIN_LOW_SURROGATE);
        }

        public static int CodePointAtImpl(char[] a, int index, int limit)
        {
            char c1 = a[index];
            if (IsHighSurrogate(c1) && ++index < limit)
            {
                char c2 = a[index];
                if (IsLowSurrogate(c2))
                {
                    return ToCodePoint(c1, c2);
                }
            }
            return c1;
        }

        public static int CharCount(int codePoint)
        {
            return codePoint >= MIN_SUPPLEMENTARY_CODE_POINT ? 2 : 1;
        }

    }
}
