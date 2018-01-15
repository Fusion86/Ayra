using Ayra.Core.Extensions;
using Ayra.Core.Structs.WiiU;
using System;
using System.Collections.Generic;

namespace Ayra.Core.Models.WiiU
{
    public class Ticket
    {
        public _TicketEntry[] Tickets;

        public Ticket(List<_TicketEntry> entries)
        {
            Tickets = entries.ToArray();
        }

        public static Ticket Load(ref byte[] data)
        {
            // Size:
            // ===
            // TicketEntry = 0x264
            // TimeLimit * 8 = (8 * 8 = 64) 0x40
            // Padding/stuff = 0xAC
            // ===
            // Total = 0x350

            //int idx = 0;
            List<_TicketEntry> entries = new List<_TicketEntry>();

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

            _TicketEntry entry = new _TicketEntry();
            byte[] headerData = new byte[0x264];
            Buffer.BlockCopy(data, 0, headerData, 0, 0x264);
            entry.Header = headerData.ToStruct<_TicketEntry_Header>();
            entry.TimeLimit = new _TicketEntry_TimeLimit[8]; // Always 8

            for (int i = 0; i < 8; i++)
            {
                byte[] timeLimit = new byte[0x8];
                Buffer.BlockCopy(data, 0x264 + i * 8, timeLimit, 0, 8);
                entry.TimeLimit[i] = timeLimit.ToStruct<_TicketEntry_TimeLimit>();
            }
            entries.Add(entry);

            return new Ticket(entries);
        }
    }
}
