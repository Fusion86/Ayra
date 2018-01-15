using Ayra.Core.Classes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ayra.Core.Models.WiiU
{
    public class Game
    {
        public TMD Tmd;
        public Ticket Ticket;
        public FST Fst;

        public string LocalPath;

        public async static Task<Game> GetFromNus(string titleId)
        {
            NUSClientWiiU client = new NUSClientWiiU();
            TMD tmd = await client.DownloadTMD(titleId);

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
