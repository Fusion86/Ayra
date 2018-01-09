using Ayra.Core.Helpers;
using System;
using System.Runtime.InteropServices;

namespace Ayra.Core.Structs
{
    // See http://wiibrew.org/wiki/Ticket

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _TicketEntry_Header
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
    public struct _TicketEntry_TimeLimit
    {
        [Endian(Endianness.BigEndian)]
        public UInt32 Enabled;

        [Endian(Endianness.BigEndian)]
        public UInt32 Seconds; // In seconds
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _TicketEntry
    {
        public _TicketEntry_Header Header;
        public _TicketEntry_TimeLimit[] TimeLimit;
    }

}
