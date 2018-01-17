using Ayra.Core.Classes;
using System.IO;
using System.Threading.Tasks;

namespace Ayra.Core.Models.N3DS
{
    public class Game
    {
        public TMD Tmd;
        //public Ticket Ticket;
        //public FST Fst;

        public string LocalPath;

        public async static Task<Game> GetFromNus(string titleId)
        {
            NUSClientN3DS client = new NUSClientN3DS();
            TMD tmd = await client.DownloadTMD(titleId);

            return new Game
            {
                Tmd = tmd,
                LocalPath = Path.Combine("download", tmd.Header.TitleId.ToString("X8"))
            };
        }
    }
}
