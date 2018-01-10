using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using Ayra.Core.Classes;
using Ayra.Core.Enums;
using Ayra.Core.Models;
using System.IO;
using System.Net;
using Ayra.Core.Helpers;

namespace Ayra.Benchmark.Benchmark
{
    [ClrJob, CoreJob, MonoJob]
    public class Stuff : IBenchmark
    {
        const string filePath = "tmd";
        const string ticketPath = "cetk";
        const string gamePath = "game";

        [GlobalSetup]
        public void Setup()
        {
            NUSClient nus = new NUSClient(NDevice.WII_U);
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
