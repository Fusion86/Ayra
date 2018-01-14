using Ayra.Core.Classes;
using Ayra.Core.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ayra.Core.Models
{
    public class Game
    {
        public TMD Tmd;
        public Ticket Ticket;
        public FST Fst;

        public string LocalPath;

        public async static Task<Game> GetFromNus(string titleId)
        {
            NUSClient client = new NUSClient(NDevice.WII_U);
            TMD tmd = await client.DownloadTMD("0005000e101a9f00");

            return new Game
            {
                Tmd = tmd,
                LocalPath = Path.Combine("download", tmd.Header.TitleId.ToString("X8"))
            };
        }

        public async void DecryptContent()
        {
            if (Ticket == null) throw new Exception("No ticket!");
        }
    }
}
