using Ayra.Core.Classes;
using System.IO;
using System.Threading.Tasks;

namespace Ayra.Core.Models.CTR
{
    public class Game
    {
        public TMD Tmd;
        public Ticket Ticket;

        public string LocalPath;

        /// <summary>
        /// Download TMD from NUS and create a new game model
        /// </summary>
        /// <returns>New game model</returns>
        /// <param name="titleId"></param>
        public async static Task<Game> GetFromNus(string titleId)
        {
            NUSClientCTR client = new NUSClientCTR();
            TMD tmd = await client.DownloadTMD(titleId);

            return new Game
            {
                Tmd = tmd,
                LocalPath = Path.Combine("download", tmd.Header.TitleId.ToString("X8"))
            };
        }
    }
}
