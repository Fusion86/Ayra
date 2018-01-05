using Ayra.Core;
using Ayra.Core.Enums;
using Ayra.Core.Models;
using Ayra.TitleKeyDatabase.Wii_U;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

        static async Task Main(string[] args)
        {
            NUSClient client = new NUSClient(NDevice.WII_U);
            TMD tmd = await client.DownloadTMD("0005000c101c9500");

            client.DownloadTitle(tmd, "download");
        }

        #region CLI Methods
        static TitleKeyDatabaseEntry CLI_SelectGame(IEnumerable<TitleKeyDatabaseEntry> keys)
        {
            while (true)
            {
                Console.Write("Search for game: ");
                string query = Console.ReadLine().ToLower();

                List<TitleKeyDatabaseEntry> searchResults = keys.Where(x => x.Name != null && x.Name.ToLower().Contains(query)).ToList();

                if (searchResults.Count > 0)
                {
                    for (int i = 0; i < searchResults.Count; i++)
                    {
                        TitleKeyDatabaseEntry x = searchResults[i];

                        string str = x.HasTicket ? (i + 1).ToString().PadRight(4) : "".PadRight(4);
                        str += $"{x.Name.Replace("\n", " ")} [{NSoftwareTypes.GetById(x.Id)}] [{x.Region}]";
                        str += $" [{x.Id}]";
                        if (!x.HasTicket) str += " [NO TICKET]";

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
