using Ayra.Core;
using Ayra.Core.Classes;
using Ayra.Core.Enums;
using Ayra.Core.Helpers;
using Ayra.Core.Models;
using Ayra.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ayra.TitleKeyDatabase;

namespace Ayra.CLI
{
    class Program
    {
        //static async Task Main(string[] args)
        //{
        //    Console.WriteLine($"Ayra.CLI v{Assembly.GetExecutingAssembly().GetName().Version}");
        //    Console.WriteLine($"Ayra.Core v{Assembly.GetAssembly(typeof(TitleKeyDatabaseEntry)).GetName().Version}\n");

        //    TitleKeyDatabase.Wii_U.TitleKeyDatabase titleKeyDatabase = TitleKeyDatabase.Wii_U.TitleKeyDatabase.Instance;

        //    Console.WriteLine("Downloading Title Keys...");
        //    titleKeyDatabase.UpdateDatabase("http://wiiu.titlekeys.gq/");
        //    Console.WriteLine($"Downloaded {titleKeyDatabase.Entries.Count} Title Keys\n");

        //    TitleKeyDatabaseEntry selectedGame = CLI_SelectGame(titleKeyDatabase.Entries);

        //    NUSClient client = new NUSClient(NDevice.WII_U);

        //    Console.WriteLine("Downloading metadata...");
        //    TMD tmd = await client.DownloadTMD(selectedGame.Id);

        //    //if (!selectedGame.HasTicket)
        //    //{
        //    //    Console.WriteLine("There is not ticket available for this title!");
        //    //    goto Exit;
        //    //}

        //    //byte[] ticket = await selectedGame.DownloadTicket("http://wiiu.titlekeys.gq/");

        //    Console.WriteLine("Press enter to exit...");
        //    Console.ReadLine();
        //}

        //static async Task Main(string[] args)
        //{
        //    NUSClient client = new NUSClient(NDevice.WII_U);
        //    TMD tmd = await client.DownloadTMD("0005000e101a9f00");
        //    string gameLocation = tmd.Header.TitleId.ToString("X8");

        //    //await client.DownloadTitle(tmd, gameLocation);
        //    byte[] ticketData = File.ReadAllBytes(Path.Combine(gameLocation, "cetk"));
        //    Ticket ticket = Ticket.Load(ref ticketData);

        //    CDecrypt.DecryptContents(tmd, ticket, gameLocation);
        //}

        static async Task Main(string[] args)
        {
            NUSClient client = new NUSClient(NDevice.WII_U);
            TMD tmd = await client.DownloadTMD("0004000000068f00");

            Console.WriteLine($"TitleID: {tmd.Header.TitleId.ToString("X8")}");

            TitleKeyDatabase.N3DS.TitleKeyDatabase titleKeyDatabase = new TitleKeyDatabase.N3DS.TitleKeyDatabase();
            titleKeyDatabase.UpdateDatabase();
            var list = titleKeyDatabase.Entries;

            var item = list.FirstOrDefault(x => x.Id.ToLower() == "0004000000068f00".ToLower());

            if (item != null)
                Console.WriteLine($"TitleKeyDatabaseEntry: {item.Name} [{item.Region}]");
        }

        #region CLI Methods
        static TitleKeyDatabaseEntryBase CLI_SelectGame(IEnumerable<TitleKeyDatabaseEntryBase> keys)
        {
            while (true)
            {
                Console.Write("Search for game: ");
                string query = Console.ReadLine().ToLower();

                List<TitleKeyDatabaseEntryBase> searchResults = keys.Where(x => x.Name != null && x.Name.ToLower().Contains(query)).ToList();

                if (searchResults.Count > 0)
                {
                    for (int i = 0; i < searchResults.Count; i++)
                    {
                        TitleKeyDatabaseEntryBase x = searchResults[i];

                        //string str = x.HasTicket ? (i + 1).ToString().PadRight(4) : "".PadRight(4);
                        string str = "";
                        str += $"{x.Name.Replace("\n", " ")} [{NSoftwareTypes.GetById(x.Id)}] [{x.Region}]";
                        str += $" [{x.Id}]";
                        //if (!x.HasTicket) str += " [NO TICKET]";

                        Console.WriteLine(str);
                    }

                    int selectedNumber = 0;
                    do
                    {
                        Console.Write("Enter number: ");
                        string str = Console.ReadLine();
                        int.TryParse(str, out selectedNumber);
                    } while (selectedNumber == 0);

                    return searchResults[selectedNumber - 1];
                }
                else if (searchResults.Count == 1)
                {
                    return searchResults[0];
                }
                else
                {
                    Console.WriteLine("Nothing found, try again.");
                }
            }
        }
        #endregion
    }
}
