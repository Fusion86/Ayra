using Ayra.Core.Classes;
using Ayra.Core.Helpers;
using Ayra.Core.Models.WUP;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System.IO;
using System.Net;

namespace Ayra.Benchmark.Benchmark
{
    [ClrJob, CoreJob, MonoJob]
    public class Stuff : IBenchmark
    {
        private const string filePath = "tmd";
        private const string ticketPath = "cetk";
        private const string gamePath = "game";

        [GlobalSetup]
        public void Setup()
        {
            NUSClientWiiU nus = new NUSClientWiiU();
            WebClient webClient = new WebClient();

            TMD tmd = nus.DownloadTMD(Config.TitleId, true, filePath).Result;
            nus.DownloadTitle(tmd, gamePath).Wait();
            webClient.DownloadFile(Config.TicketUrl, ticketPath);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            File.Delete(filePath);
            File.Delete(ticketPath);
            Directory.Delete(gamePath, true);
        }

        [Benchmark]
        public void LoadTMD()
        {
            byte[] data = File.ReadAllBytes(filePath);
            TMD tmd = TMD.Load(ref data);
        }

        [Benchmark]
        public void LoadTicket()
        {
            byte[] data = File.ReadAllBytes(ticketPath);
            Ticket ticket = Ticket.Load(ref data);
        }

        [Benchmark]
        public void DecryptGame()
        {
            byte[] data = File.ReadAllBytes(filePath);
            TMD tmd = TMD.Load(ref data);

            data = File.ReadAllBytes(ticketPath);
            Ticket ticket = Ticket.Load(ref data);

            CDecrypt.DecryptContents(tmd, ticket, gamePath);
        }
    }
}
