using Ayra.Core.Extensions;
using Ayra.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ayra.Core.Models
{
    // See http://wiibrew.org/wiki/Ticket

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TicketEntry_Header
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 SignatureType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.U1)]
        public byte[] Signature;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60, ArraySubType = UnmanagedType.U1)]
        public byte[] Padding1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Issuer; // Root-CA%08x-CP%08x

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60, ArraySubType = UnmanagedType.U1)]
        public byte[] ECDH;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Padding2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
        public byte[] EncryptedTitleKey;

        byte Unk1;

        [Endian(Endianness.BigEndian)]
        public UInt64 TicketId;

        [Endian(Endianness.BigEndian)]
        public UInt32 ConsoleId;

        [Endian(Endianness.BigEndian)]
        public UInt64 TitleId;

        public UInt16 Unk2;

        [Endian(Endianness.BigEndian)]
        public UInt16 TicketVersion;

        [Endian(Endianness.BigEndian)]
        public UInt32 PermittedTitlesMask;

        [Endian(Endianness.BigEndian)]
        public UInt32 PermitMask;

        bool TitleExportAllowed;
        byte CommonKeyIndex;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk3; // If last byte == 0x01 then this is a VC title

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] ContentAccessPermissions;

        ushort Padding3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TicketEntry_TimeLimit
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 Enabled;

        [Endian(Endianness.BigEndian)]
        public UInt32 Seconds; // In seconds
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TicketEntry
    {
        public TicketEntry_Header Header;
        public TicketEntry_TimeLimit[] TimeLimit;
    }

    public class Ticket
    {
        public TicketEntry[] Tickets;

        public Ticket(List<TicketEntry> entries)
        {
            Tickets = entries.ToArray();
        }

        public static Ticket Load(byte[] data)
        {
            // Size:
            // ===
            // TicketEntry = 0x264
            // TimeLimit * 8 = (8 * 8 = 64) 0x40
            // Padding/stuff = 0xAC
            // ===
            // Total = 0x350

            //int idx = 0;
            List<TicketEntry> entries = new List<TicketEntry>();

            //// Parse all ticket entries
            //while (true)
            //{
            //    int baseOffset = idx++ * 0x350;

            //    if (baseOffset >= data.Length) break; // Break if we parsed all the data

            //    TicketEntry entry = new TicketEntry();

            //    byte[] headerData = new byte[0x264];
            //    Buffer.BlockCopy(data, baseOffset, headerData, 0, 0x264);
            //    entry.Header = headerData.ToStruct<TicketEntry_Header>();
            //    entry.TimeLimit = new TicketEntry_TimeLimit[8]; // Always 8

            //    for (int i = 0; i < 8; i++)
            //    {
            //        byte[] timeLimit = new byte[0x40];
            //        Buffer.BlockCopy(data, baseOffset + 0x264 + i * 8, timeLimit, 0, 8);
            //        entry.TimeLimit[i] = timeLimit.ToStruct<TicketEntry_TimeLimit>();
            //    }

            //    entries.Add(entry);
            //}


            // Only parse the first entry for now, since we don't need more atm and above code is incorrect
            // Not all items in a ticket file are the same (as far as I know atm)

            TicketEntry entry = new TicketEntry();
            byte[] headerData = new byte[0x264];
            Buffer.BlockCopy(data, 0, headerData, 0, 0x264);
            entry.Header = headerData.ToStruct<TicketEntry_Header>();
            entry.TimeLimit = new TicketEntry_TimeLimit[8]; // Always 8

            for (int i = 0; i < 8; i++)
            {
                byte[] timeLimit = new byte[0x40];
                Buffer.BlockCopy(data, 0x264 + i * 8, timeLimit, 0, 8);
                entry.TimeLimit[i] = timeLimit.ToStruct<TicketEntry_TimeLimit>();
            }
            entries.Add(entry);

            return new Ticket(entries);
        }
    }
}
