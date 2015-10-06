using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace WebSocket4UWP
{
    /*
  0                   1                   2                   3
  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
  +-+-+-+-+-------+-+-------------+-------------------------------+
  |F|R|R|R| opcode|M| Payload len |    Extended payload length    |
  |I|S|S|S|  (4)  |A|     (7)     |             (16/64)           |
  |N|V|V|V|       |S|             |   (if payload len==126/127)   |
  | |1|2|3|       |K|             |                               |
  +-+-+-+-+-------+-+-------------+ - - - - - - - - - - - - - - - +
  |     Extended payload length continued, if payload len == 127  |
  + - - - - - - - - - - - - - - - +-------------------------------+
  |                               |Masking-key, if MASK set to 1  |
  +-------------------------------+-------------------------------+
  | Masking-key (continued)       |          Payload Data         |
  +-------------------------------- - - - - - - - - - - - - - - - +
  :                     Payload Data continued ...                :
  + - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - +
  |                     Payload Data continued ...                |
  +---------------------------------------------------------------+
  FIN: Indicates that this is the final fragment in a message.
  RSV1 RSV2 RSV3 reserved, must be 0
  Opcode:  4 bits Defines the interpretation of the "Payload data.

  */
    internal sealed class FrameParser
    {
        public static Random random = new Random();

        // first byte bits:
        private static int FIN = (1 << 7);
        private static int RSV = 0x70;
        private static int OPCODE = 0x0F;
        // the second byte bits: 
        private static int MASK = (1 << 7);
        private static int LENGTH = 0x7F;

        // operation codes
        public const int OP_CONTINUATION = 0;
        public const int OP_TEXT = 1;
        public const int OP_BINARY = 2;
        public const int OP_CLOSE = 8;
        public const int OP_PING = 9;
        public const int OP_PONG = 10;

        // to check
        private static int[] OPCODES = { OP_CONTINUATION, OP_TEXT, OP_BINARY, OP_CLOSE, OP_PING, OP_PONG };
        private static int[] CTRL_OPCODES = { OP_CLOSE, OP_PING, OP_PONG };


        public class Frame
        {
            public byte[] payload;
            public int opCode;
            public bool isMasked;
            public bool isFinal;
        }

        private static void UseMask(byte[] payload, byte[] mask, int offset)
        {
            for (int i = 0; i < payload.Length - offset; i++)
            {
                payload[offset + i] = (byte)(payload[offset + i] ^ mask[i % 4]);
            }
        }


        /**
         *  Read and parse frame
         */
        public async static Task<Frame> ReadFrame(DataReader dataReader)
        {
            Frame frame = new Frame();

            // read the first byte (op code)

            //var dataReader = new DataReader(stream);

            var DataReaderLoadOperation = await dataReader.LoadAsync(sizeof(byte));

            //if (DataReaderLoadOperation != sizeof(byte))
            //{
            //    throw new Exception("DataReaderLoadOperation is zero");//抛出异常
            //}

            int data = dataReader.ReadByte();


            frame.isFinal = (data & FIN) != 0;
            bool isReserved = (data & RSV) != 0;
            if (isReserved)
            {
                throw new Exception("RSV not zero");
            }

            frame.opCode = (data & OPCODE);



            if (Array.BinarySearch<int>(OPCODES, frame.opCode) < 0)
            {
                throw new Exception("Bad opcode");
            }

            if (Array.BinarySearch<int>(CTRL_OPCODES, frame.opCode) >= 0 && !frame.isFinal)
            {
                throw new Exception("In control opcode, must set FIN");
            }

            // read the second byte (mask and payload length)

            await dataReader.LoadAsync(sizeof(byte));

            data = dataReader.ReadByte();

            frame.isMasked = (data & MASK) != 0;
            int length = (data & LENGTH);

            // read extended length if need
            if (length < 126)
            {
                // short length is already read.
            }
            else if (length == 126)
            {
                await dataReader.LoadAsync(sizeof(UInt16));
                var b1 = dataReader.ReadByte();
                var b2 = dataReader.ReadByte();

                //length = b1 * 256 + b2;

                length = (((b1 & 0xff) << 8) | (b2 & 0xff));
                //  length = dataReader.ReadUInt16(); // read 2 bytes length
            }
            else if (length == 127)
            {
                await dataReader.LoadAsync(sizeof(Int64));
                byte[] longByte = new byte[8];
                //     long length8 = dataReader.ReadInt64();  // read 8 bytes length

                dataReader.ReadBytes(longByte);

                long length8 = byte2Long(longByte);
                if (length8 > int.MaxValue)
                {
                    throw new IOException("too big frame length");
                }
                length = (int)length8;
            }

            byte[] mask = null;
            if (frame.isMasked)
            {
                mask = new byte[4];
                await dataReader.LoadAsync(4);
                dataReader.ReadBytes(mask);
                //stream.readFully(mask);
            }

            frame.payload = new byte[length]; // can be optimized.
            //stream.readFully(frame.payload);

            if (length > 0)
            {
                await dataReader.LoadAsync((uint)length);
                dataReader.ReadBytes(frame.payload);
            }

            if (frame.isMasked)
            {
                UseMask(frame.payload, mask, 0);
            }


            return frame;
        }

        public static long byte2Long(byte[] b)
        {
            return
            ((b[0] & 0xff) << 56) |
            ((b[1] & 0xff) << 48) |
            ((b[2] & 0xff) << 40) |
            ((b[3] & 0xff) << 32) |

            ((b[4] & 0xff) << 24) |
            ((b[5] & 0xff) << 16) |
            ((b[6] & 0xff) << 8) |
            (b[7] & 0xff);
        }



        public static byte[] BuildFrame(byte[] buffer, int opcode, int errorCode, bool isMasked, bool isFinal)
        {
            if (buffer == null)
                buffer = new byte[0];
            int insert = (errorCode > 0) ? 2 : 0;
            int length = buffer.Length + insert;
            int header = (length <= 125) ? 2 : (length <= 0xFFFF ? 4 : 10);
            int offset = header + (isMasked ? 4 : 0);
            byte[] frame = new byte[length + offset];
            int masked = isMasked ? MASK : 0;
            int finbit = isFinal ? FIN : 0;

            frame[0] = (byte)((byte)(finbit) | (byte)opcode); // always create only one frame

            if (length <= 125)
            {
                frame[1] = (byte)(masked | length);
            }
            else if (length <= 65535)
            {
                frame[1] = (byte)(masked | 126);
                frame[2] = (byte)(length >> 8);
                frame[3] = (byte)(length & 0xFF);
            }
            else
            {
                frame[1] = (byte)(masked | 127);
                //frame[2] = (byte) (0);
                //frame[3] = (byte) (0);
                //frame[4] = (byte) (0);
                //frame[5] = (byte) (0);
                frame[6] = (byte)((length >> 24) & 0xFF);
                frame[7] = (byte)((length >> 16) & 0xFF);
                frame[8] = (byte)((length >> 8) & 0xFF);
                frame[9] = (byte)(length & 0xFF);
            }

            if (errorCode > 0)
            {
                frame[offset] = (byte)((errorCode >> 8) & 0xFF);
                frame[offset + 1] = (byte)(errorCode & 0xFF);
            }

            Array.Copy(buffer, 0, frame, offset + insert, buffer.Length);

            if (isMasked)
            {
                byte[] mask = new byte[4];

                random.NextBytes(mask);
                Array.Copy(mask, 0, frame, header, mask.Length);
                UseMask(frame, mask, offset);
            }

            return frame;
        }
    }
}
