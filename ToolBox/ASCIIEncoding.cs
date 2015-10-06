using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebSocket.ToolBox
{
    public class ASCIIEncoding : Encoding
    {
        private static ASCIIEncoding m_Instance;
        private char? fallbackCharacter;
        private static char[] byteToChar;
        private static Dictionary<char, byte> charToByte;
        public static ASCIIEncoding Instance
        {
            get
            {
                return ASCIIEncoding.m_Instance;
            }
        }
        public override string WebName
        {
            get
            {
                return "us-ascii";
            }
        }
        public char? FallbackCharacter
        {
            get
            {
                return this.fallbackCharacter;
            }
            set
            {
                this.fallbackCharacter = value;
                if (value.HasValue && !ASCIIEncoding.charToByte.ContainsKey(value.Value))
                {
                    string msg = "Cannot use the character [{0}] (int value {1}) as fallback value - the fallback character itself is not supported by the encoding.";
                    msg = string.Format(msg, value.Value, (int)value.Value);
                    throw new EncoderFallbackException(msg);
                }
                this.FallbackByte = (value.HasValue ? new byte?(ASCIIEncoding.charToByte[value.Value]) : default(byte?));
            }
        }
        public byte? FallbackByte
        {
            get;
            private set;
        }
        public static int CharacterCount
        {
            get
            {
                return ASCIIEncoding.byteToChar.Length;
            }
        }
        static ASCIIEncoding()
        {
            ASCIIEncoding.m_Instance = null;
            ASCIIEncoding.byteToChar = new char[]
			{
				'\0',
				'\u0001',
				'\u0002',
				'\u0003',
				'\u0004',
				'\u0005',
				'\u0006',
				'\a',
				'\b',
				'\t',
				'\n',
				'\v',
				'\f',
				'\r',
				'\u000e',
				'\u000f',
				'\u0010',
				'\u0011',
				'\u0012',
				'\u0013',
				'\u0014',
				'\u0015',
				'\u0016',
				'\u0017',
				'\u0018',
				'\u0019',
				'\u001a',
				'\u001b',
				'\u001c',
				'\u001d',
				'\u001e',
				'\u001f',
				' ',
				'!',
				'"',
				'#',
				'$',
				'%',
				'&',
				'\'',
				'(',
				')',
				'*',
				'+',
				',',
				'-',
				'.',
				'/',
				'0',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9',
				':',
				';',
				'<',
				'=',
				'>',
				'?',
				'@',
				'A',
				'B',
				'C',
				'D',
				'E',
				'F',
				'G',
				'H',
				'I',
				'J',
				'K',
				'L',
				'M',
				'N',
				'O',
				'P',
				'Q',
				'R',
				'S',
				'T',
				'U',
				'V',
				'W',
				'X',
				'Y',
				'Z',
				'[',
				'\\',
				']',
				'^',
				'_',
				'`',
				'a',
				'b',
				'c',
				'd',
				'e',
				'f',
				'g',
				'h',
				'i',
				'j',
				'k',
				'l',
				'm',
				'n',
				'o',
				'p',
				'q',
				'r',
				's',
				't',
				'u',
				'v',
				'w',
				'x',
				'y',
				'z',
				'{',
				'|',
				'}',
				'~',
				'\u007f'
			};
            Dictionary<char, byte> dictionary = new Dictionary<char, byte>();
            dictionary.Add('\0', 0);
            dictionary.Add('\u0001', 1);
            dictionary.Add('\u0002', 2);
            dictionary.Add('\u0003', 3);
            dictionary.Add('\u0004', 4);
            dictionary.Add('\u0005', 5);
            dictionary.Add('\u0006', 6);
            dictionary.Add('\a', 7);
            dictionary.Add('\b', 8);
            dictionary.Add('\t', 9);
            dictionary.Add('\n', 10);
            dictionary.Add('\v', 11);
            dictionary.Add('\f', 12);
            dictionary.Add('\r', 13);
            dictionary.Add('\u000e', 14);
            dictionary.Add('\u000f', 15);
            dictionary.Add('\u0010', 16);
            dictionary.Add('\u0011', 17);
            dictionary.Add('\u0012', 18);
            dictionary.Add('\u0013', 19);
            dictionary.Add('\u0014', 20);
            dictionary.Add('\u0015', 21);
            dictionary.Add('\u0016', 22);
            dictionary.Add('\u0017', 23);
            dictionary.Add('\u0018', 24);
            dictionary.Add('\u0019', 25);
            dictionary.Add('\u001a', 26);
            dictionary.Add('\u001b', 27);
            dictionary.Add('\u001c', 28);
            dictionary.Add('\u001d', 29);
            dictionary.Add('\u001e', 30);
            dictionary.Add('\u001f', 31);
            dictionary.Add(' ', 32);
            dictionary.Add('!', 33);
            dictionary.Add('"', 34);
            dictionary.Add('#', 35);
            dictionary.Add('$', 36);
            dictionary.Add('%', 37);
            dictionary.Add('&', 38);
            dictionary.Add('\'', 39);
            dictionary.Add('(', 40);
            dictionary.Add(')', 41);
            dictionary.Add('*', 42);
            dictionary.Add('+', 43);
            dictionary.Add(',', 44);
            dictionary.Add('-', 45);
            dictionary.Add('.', 46);
            dictionary.Add('/', 47);
            dictionary.Add('0', 48);
            dictionary.Add('1', 49);
            dictionary.Add('2', 50);
            dictionary.Add('3', 51);
            dictionary.Add('4', 52);
            dictionary.Add('5', 53);
            dictionary.Add('6', 54);
            dictionary.Add('7', 55);
            dictionary.Add('8', 56);
            dictionary.Add('9', 57);
            dictionary.Add(':', 58);
            dictionary.Add(';', 59);
            dictionary.Add('<', 60);
            dictionary.Add('=', 61);
            dictionary.Add('>', 62);
            dictionary.Add('?', 63);
            dictionary.Add('@', 64);
            dictionary.Add('A', 65);
            dictionary.Add('B', 66);
            dictionary.Add('C', 67);
            dictionary.Add('D', 68);
            dictionary.Add('E', 69);
            dictionary.Add('F', 70);
            dictionary.Add('G', 71);
            dictionary.Add('H', 72);
            dictionary.Add('I', 73);
            dictionary.Add('J', 74);
            dictionary.Add('K', 75);
            dictionary.Add('L', 76);
            dictionary.Add('M', 77);
            dictionary.Add('N', 78);
            dictionary.Add('O', 79);
            dictionary.Add('P', 80);
            dictionary.Add('Q', 81);
            dictionary.Add('R', 82);
            dictionary.Add('S', 83);
            dictionary.Add('T', 84);
            dictionary.Add('U', 85);
            dictionary.Add('V', 86);
            dictionary.Add('W', 87);
            dictionary.Add('X', 88);
            dictionary.Add('Y', 89);
            dictionary.Add('Z', 90);
            dictionary.Add('[', 91);
            dictionary.Add('\\', 92);
            dictionary.Add(']', 93);
            dictionary.Add('^', 94);
            dictionary.Add('_', 95);
            dictionary.Add('`', 96);
            dictionary.Add('a', 97);
            dictionary.Add('b', 98);
            dictionary.Add('c', 99);
            dictionary.Add('d', 100);
            dictionary.Add('e', 101);
            dictionary.Add('f', 102);
            dictionary.Add('g', 103);
            dictionary.Add('h', 104);
            dictionary.Add('i', 105);
            dictionary.Add('j', 106);
            dictionary.Add('k', 107);
            dictionary.Add('l', 108);
            dictionary.Add('m', 109);
            dictionary.Add('n', 110);
            dictionary.Add('o', 111);
            dictionary.Add('p', 112);
            dictionary.Add('q', 113);
            dictionary.Add('r', 114);
            dictionary.Add('s', 115);
            dictionary.Add('t', 116);
            dictionary.Add('u', 117);
            dictionary.Add('v', 118);
            dictionary.Add('w', 119);
            dictionary.Add('x', 120);
            dictionary.Add('y', 121);
            dictionary.Add('z', 122);
            dictionary.Add('{', 123);
            dictionary.Add('|', 124);
            dictionary.Add('}', 125);
            dictionary.Add('~', 126);
            dictionary.Add('\u007f', 127);
            ASCIIEncoding.charToByte = dictionary;
            ASCIIEncoding.m_Instance = new ASCIIEncoding();
        }
        public override int GetHashCode()
        {
            return this.WebName.GetHashCode();
        }
        public ASCIIEncoding()
        {
            this.FallbackCharacter = new char?('?');
        }
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return this.FallbackByte.HasValue ? this.GetBytesWithFallBack(chars, charIndex, charCount, bytes, byteIndex) : this.GetBytesWithoutFallback(chars, charIndex, charCount, bytes, byteIndex);
        }
        private int GetBytesWithFallBack(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                char character = chars[i + charIndex];
                byte byteValue;
                bool status = ASCIIEncoding.charToByte.TryGetValue(character, out byteValue);
                bytes[byteIndex + i] = (status ? byteValue : this.FallbackByte.Value);
            }
            return charCount;
        }
        private int GetBytesWithoutFallback(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                char character = chars[i + charIndex];
                byte byteValue;
                if (!ASCIIEncoding.charToByte.TryGetValue(character, out byteValue))
                {
                    string msg = "The encoding [{0}] cannot encode the character [{1}] (int value {2}). Set the FallbackCharacter property in order to suppress this exception and encode a default character instead.";
                    msg = string.Format(msg, this.WebName, character, (int)character);
                    throw new EncoderFallbackException(msg);
                }
                bytes[byteIndex + i] = byteValue;
            }
            return charCount;
        }
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return this.FallbackCharacter.HasValue ? this.GetCharsWithFallback(bytes, byteIndex, byteCount, chars, charIndex) : this.GetCharsWithoutFallback(bytes, byteIndex, byteCount, chars, charIndex);
        }
        private int GetCharsWithFallback(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                byte lookupIndex = bytes[i + byteIndex];
                char result = ((int)lookupIndex >= ASCIIEncoding.byteToChar.Length) ? this.FallbackCharacter.Value : ASCIIEncoding.byteToChar[(int)lookupIndex];
                chars[charIndex + i] = result;
            }
            return byteCount;
        }
        private int GetCharsWithoutFallback(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                byte lookupIndex = bytes[i + byteIndex];
                if ((int)lookupIndex >= ASCIIEncoding.byteToChar.Length)
                {
                    string msg = "The encoding [{0}] cannot decode byte value [{1}]. Set the FallbackCharacter property in order to suppress this exception and decode the value as a default character instead.";
                    msg = string.Format(msg, this.WebName, lookupIndex);
                    throw new EncoderFallbackException(msg);
                }
                chars[charIndex + i] = ASCIIEncoding.byteToChar[(int)lookupIndex];
            }
            return byteCount;
        }
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }
    }
}
