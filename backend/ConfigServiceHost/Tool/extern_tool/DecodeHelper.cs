
using XViewer.Model.Data;

namespace Tool
{
    class DecodeHelper
    {
        public static Params DecodeTag(byte[] buffer)
        {
            //元素都用两个字节来标识
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, 0, 2);
            }
            Params tag = (Params)BitConverter.ToUInt16(buffer, 0);

            return tag;
        }

        public static string DecodeStringTag(MemoryStream packet, Params tag)
        {
            string strtag = null;

            int bufferlen = 3;
            byte[] buffer = new byte[bufferlen];
            packet.Read(buffer, 0, bufferlen);
            Params ctag = DecodeTag(buffer);
            if (tag == ctag)
            {
                int taglen = buffer[2];
                if (0 < taglen)
                {
                    byte[] tagvalue = new byte[taglen];
                    packet.Read(tagvalue, 0, taglen);
                    strtag = System.Text.Encoding.UTF8.GetString(tagvalue, 0, taglen);
                }
            }
            else
            {
                packet.Seek(-bufferlen, SeekOrigin.Current);
            }

            return strtag;
        }

        public static bool DecodeBoolTag(MemoryStream packet, Params tag)
        {
            byte temp = DecodeByteTag(packet, tag);

            bool ret = temp > 0 ? true : false;

            return ret;
        }

        public static byte DecodeByteTag(MemoryStream packet, Params tag)
        {
            byte ret = 0;

            int bufferlen = 3;
            byte[] buffer = new byte[bufferlen];
            packet.Read(buffer, 0, bufferlen);
            Params ctag = DecodeTag(buffer);
            if (tag == ctag)
            {
                int taglen = buffer[2];
                if (0 < taglen)
                {
                    byte[] tagvalue = new byte[taglen];
                    packet.Read(tagvalue, 0, taglen);
                    ret = tagvalue[0];
                }
            }
            else
            {
                packet.Seek(-bufferlen, SeekOrigin.Current);
            }

            return ret;
        }

        public static int DecodeIntTag(MemoryStream packet, Params tag)
        {
            int ret = 0;

            int bufferlen = 3;
            byte[] buffer = new byte[bufferlen];
            packet.Read(buffer, 0, bufferlen);
            Params ctag = DecodeTag(buffer);
            if (tag == ctag)
            {
                int taglen = buffer[2];
                if (0 < taglen)
                {
                    byte[] tagvalue = new byte[taglen];
                    packet.Read(tagvalue, 0, taglen);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(tagvalue, 0, taglen);
                    }
                    ret = BitConverter.ToInt32(tagvalue, 0);
                }
            }
            else
            {
                packet.Seek(-bufferlen, SeekOrigin.Current);
            }

            return ret;
        }
    }
}
