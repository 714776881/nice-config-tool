using XViewer.Model.Data;

namespace Tool
{
    class EncodeHelper
    {
        public static MemoryStream CreateResponseHeader(Cmds command, int status)
        {
            MemoryStream data = new MemoryStream();
            EncodeTag(data, Params.PARAMETER_COMMANDID);
            data.WriteByte(0x01);
            data.WriteByte((byte)command);
            EncodeIntTag(data, Params.PARAMETER_RESPONSE_STATUS, status);

            return data;
        }

        public static void CreateHeader(MemoryStream packet, int packetlen)
        {
            packet.Write(PacketDelimiter.PacketBegin, 0, PacketDelimiter.PacketBegin.Length);
            EncodeIntTag(packet, Params.PARAMETER_PACKAGE_LENGTH, packetlen);
        }

        public static void EncodeTag(MemoryStream packet, Params tag)
        {
            ushort temp = (ushort)tag;
            byte[] r = BitConverter.GetBytes(temp);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(r);
            }
            packet.Write(r, 0, r.Length);
        }

        public static void EncodeStringTag(MemoryStream packet, Params tag, string strvalue)
        {
            if (null != strvalue)
            {
                EncodeTag(packet, tag);
                byte[] bvalue = System.Text.Encoding.UTF8.GetBytes(strvalue);
                packet.WriteByte((byte)bvalue.Length);
                packet.Write(bvalue, 0, bvalue.Length);
            }
        }

        public static void EncodeIntTag(MemoryStream packet, Params tag, int nvalue)
        {
            EncodeTag(packet, tag);
            byte[] bvalue = BitConverter.GetBytes(nvalue);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bvalue);
            }
            packet.WriteByte((byte)0x04);
            packet.Write(bvalue, 0, bvalue.Length);
        }

        public static void EncodeByteTag(MemoryStream packet, Params tag, byte value)
        {
            EncodeTag(packet, tag);
            packet.WriteByte((byte)0x01);
            packet.WriteByte(value);
        }

        public static MemoryStream CreateInvalidResponse(Cmds command, string error)
        {
            MemoryStream data = new MemoryStream();
            EncodeTag(data, Params.PARAMETER_COMMANDID);
            data.WriteByte(0x01);
            data.WriteByte((byte)command);
            EncodeTag(data, Params.PARAMETER_RESPONSE_STATUS);
            data.WriteByte((0x04));
            int status = 0;
            byte[] dstatus = BitConverter.GetBytes(status);
            data.Write(dstatus, 0, dstatus.Length);

            byte[] tipdata = System.Text.Encoding.UTF8.GetBytes(error);
            EncodeTag(data, Params.PARAMETER_RESPONSE_ERRORINFO);
            data.WriteByte((byte)tipdata.Length);
            data.Write(tipdata, 0, tipdata.Length);

            return FormatPacket(data);
        }

        public static MemoryStream FormatPacket(MemoryStream data)
        {
            EncodePacketEnd(data);
            int packetlen = (int)data.Length;

            MemoryStream packet = new MemoryStream();
            CreateHeader(packet, packetlen);
            byte[] body = data.ToArray();
            packet.Write(body, 0, body.Length);

            return packet;
        }

        public static void EncodePacketHeader(MemoryStream packet)
        {
            packet.Write(PacketDelimiter.PacketBegin, 0, PacketDelimiter.PacketBegin.Length);
        }

        public static void EncodePacketEnd(MemoryStream packet)
        {
            packet.Write(PacketDelimiter.PacketEnd, 0, PacketDelimiter.PacketEnd.Length);
        }
    }
}
