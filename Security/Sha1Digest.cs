using System;
using System.Collections.Generic;

namespace WebSocket4UWP.Security
{
    internal class Sha1Digest
    {
        private readonly uint[] _x;
        private readonly byte[] _buf;

        private uint _h1, _h2, _h3, _h4, _h5;
        private int _xOffset;
        private int _bufOffset;
        private long _byteCount;

        /// <summary>
        /// 性能测试结果比默认系统函数更高
        ///var algorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
        ///var buffer = b.BytesToBuffer();
        ///var hashedData = algorithm.HashData(buffer);
        ///return Convert.ToBase64String(hashedData.BufferToBytes());
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(byte[] input)
        {
            var sha1 = new Sha1Digest();
            sha1.Update(input, 0, input.Length);
            return sha1.GetResult();
        }

        public Sha1Digest()
        {
            _x = new uint[80];
            _buf = new byte[4];

            this.Reset();
        }

        public void Update(byte input)
        {
            _buf[_bufOffset++] = input;
            if (_bufOffset == _buf.Length)
            {
                this.ProcessWord(_buf, 0);
                _bufOffset = 0;
            }
            _byteCount++;
        }

        public void Update(byte[] input, int offset, int length)
        {
            //
            // fill the current word
            //
            while (_bufOffset != 0 && length > 0)
            {
                this.Update(input[offset]);
                offset++;
                length--;
            }

            //
            // process whole words.
            //
            while (length > _buf.Length)
            {
                this.ProcessWord(input, offset);

                offset += _buf.Length;
                length -= _buf.Length;
                _byteCount += _buf.Length;
            }

            //
            // load in the remainder.
            //
            while (length > 0)
            {
                this.Update(input[offset]);

                offset++;
                length--;
            }
        }

        public byte[] GetResult()
        {
            var output = new byte[20];

            this.Finish();

            Unpack(_h1, output, 0);
            Unpack(_h2, output, 4);
            Unpack(_h3, output, 8);
            Unpack(_h4, output, 12);
            Unpack(_h5, output, 16);

            this.Reset();

            return output;
        }

        public void Reset()
        {
            _h1 = 0x67452301;
            _h2 = 0xefcdab89;
            _h3 = 0x98badcfe;
            _h4 = 0x10325476;
            _h5 = 0xc3d2e1f0;

            _byteCount = 0;
            _xOffset = 0;
            _bufOffset = 0;
            Array.Clear(_x, 0, _x.Length);
            Array.Clear(_buf, 0, _buf.Length);
        }

        private void ProcessWord(byte[] input, int offset)
        {
            _x[_xOffset++] = Pack(input, offset);
            if (_xOffset == 16)
            {
                ProcessBlock();
            }
        }

        private void ProcessBlock()
        {
            //
            // expand 16 word block into 80 word block.
            //
            for (var i = 16; i < 80; i++)
            {
                var t = _x[i - 3] ^ _x[i - 8] ^ _x[i - 14] ^ _x[i - 16];
                _x[i] = t << 1 | t >> 31;
            }

            //
            // set up working variables.
            //
            var a = _h1;
            var b = _h2;
            var c = _h3;
            var d = _h4;
            var e = _h5;

            //
            // round 1
            //
            var idx = 0;

            for (var j = 0; j < 4; j++)
            {
                // E = rotateLeft(A, 5) + F(B, C, D) + E + _x[idx++] + Y1
                // B = rotateLeft(B, 30)
                e += (a << 5 | (a >> 27)) + F(b, c, d) + _x[idx++] + Y1;
                b = b << 30 | (b >> 2);

                d += (e << 5 | (e >> 27)) + F(a, b, c) + _x[idx++] + Y1;
                a = a << 30 | (a >> 2);

                c += (d << 5 | (d >> 27)) + F(e, a, b) + _x[idx++] + Y1;
                e = e << 30 | (e >> 2);

                b += (c << 5 | (c >> 27)) + F(d, e, a) + _x[idx++] + Y1;
                d = d << 30 | (d >> 2);

                a += (b << 5 | (b >> 27)) + F(c, d, e) + _x[idx++] + Y1;
                c = c << 30 | (c >> 2);
            }

            //
            // round 2
            //
            for (var j = 0; j < 4; j++)
            {
                // E = rotateLeft(A, 5) + H(B, C, D) + E + _x[idx++] + Y2
                // B = rotateLeft(B, 30)
                e += (a << 5 | (a >> 27)) + H(b, c, d) + _x[idx++] + Y2;
                b = b << 30 | (b >> 2);

                d += (e << 5 | (e >> 27)) + H(a, b, c) + _x[idx++] + Y2;
                a = a << 30 | (a >> 2);

                c += (d << 5 | (d >> 27)) + H(e, a, b) + _x[idx++] + Y2;
                e = e << 30 | (e >> 2);

                b += (c << 5 | (c >> 27)) + H(d, e, a) + _x[idx++] + Y2;
                d = d << 30 | (d >> 2);

                a += (b << 5 | (b >> 27)) + H(c, d, e) + _x[idx++] + Y2;
                c = c << 30 | (c >> 2);
            }

            //
            // round 3
            //
            for (var j = 0; j < 4; j++)
            {
                // E = rotateLeft(A, 5) + G(B, C, D) + E + _x[idx++] + Y3
                // B = rotateLeft(B, 30)
                e += (a << 5 | (a >> 27)) + G(b, c, d) + _x[idx++] + Y3;
                b = b << 30 | (b >> 2);

                d += (e << 5 | (e >> 27)) + G(a, b, c) + _x[idx++] + Y3;
                a = a << 30 | (a >> 2);

                c += (d << 5 | (d >> 27)) + G(e, a, b) + _x[idx++] + Y3;
                e = e << 30 | (e >> 2);

                b += (c << 5 | (c >> 27)) + G(d, e, a) + _x[idx++] + Y3;
                d = d << 30 | (d >> 2);

                a += (b << 5 | (b >> 27)) + G(c, d, e) + _x[idx++] + Y3;
                c = c << 30 | (c >> 2);
            }

            //
            // round 4
            //
            for (var j = 0; j < 4; j++)
            {
                // E = rotateLeft(A, 5) + H(B, C, D) + E + _x[idx++] + Y4
                // B = rotateLeft(B, 30)
                e += (a << 5 | (a >> 27)) + H(b, c, d) + _x[idx++] + Y4;
                b = b << 30 | (b >> 2);

                d += (e << 5 | (e >> 27)) + H(a, b, c) + _x[idx++] + Y4;
                a = a << 30 | (a >> 2);

                c += (d << 5 | (d >> 27)) + H(e, a, b) + _x[idx++] + Y4;
                e = e << 30 | (e >> 2);

                b += (c << 5 | (c >> 27)) + H(d, e, a) + _x[idx++] + Y4;
                d = d << 30 | (d >> 2);

                a += (b << 5 | (b >> 27)) + H(c, d, e) + _x[idx++] + Y4;
                c = c << 30 | (c >> 2);
            }

            _h1 += a;
            _h2 += b;
            _h3 += c;
            _h4 += d;
            _h5 += e;

            //
            // reset start of the buffer.
            //
            _xOffset = 0;
            Array.Clear(_x, 0, 16);
        }

        private void ProcessLength(long bitLength)
        {
            if (_xOffset > 14)
                this.ProcessBlock();

            _x[14] = (uint)((ulong)bitLength >> 32);
            _x[15] = (uint)((ulong)bitLength);
        }

        private void Finish()
        {
            var bitLength = (_byteCount << 3);

            //
            // add the pad bytes.
            //
            Update(0x80);

            while (_bufOffset != 0)
                Update(0x0);

            this.ProcessLength(bitLength);
            this.ProcessBlock();
        }

        private const uint Y1 = 0x5a827999;
        private const uint Y2 = 0x6ed9eba1;
        private const uint Y3 = 0x8f1bbcdc;
        private const uint Y4 = 0xca62c1d6;

        private static uint F(uint u, uint v, uint w)
        {
            return (u & v) | (~u & w);
        }

        private static uint H(uint u, uint v, uint w)
        {
            return u ^ v ^ w;
        }

        private static uint G(uint u, uint v, uint w)
        {
            return (u & v) | (u & w) | (v & w);
        }

        private static void Unpack(uint n, IList<byte> b, int off)
        {
            b[off] = (byte)(n >> 24);
            b[++off] = (byte)(n >> 16);
            b[++off] = (byte)(n >> 8);
            b[++off] = (byte)(n);
        }

        internal static uint Pack(IList<byte> b, int off)
        {
            var n = (uint)b[off] << 24;
            n |= (uint)b[++off] << 16;
            n |= (uint)b[++off] << 8;
            n |= b[++off];
            return n;
        }
    }
}
