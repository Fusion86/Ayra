using Ayra.Core;
using Ayra.Core.Enums;
using Ayra.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ayra.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Ayra.CLI v{Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"Ayra.Core v{Assembly.GetAssembly(typeof(TitleKeyDatabaseEntry)).GetName().Version}\n");

            Console.WriteLine("Downloading Title Keys...");
            List<TitleKeyDatabaseEntry> keys = TitleKeyDatabase.GetTitleKeyDatabaseEntries("http://wiiu.titlekeys.gq/");
            Console.WriteLine($"Downloaded {keys.Count} Title Keys\n");

            TitleKeyDatabaseEntry selectedGame = CLI_SelectGame(keys);

            NUSClient client = new NUSClient(NDevice.WII_U);

            Console.WriteLine("Downloading metadata...");
            TitleMetaData tmd = await client.DownloadTitleMetadata(selectedGame);

            //if (!selectedGame.HasTicket)
            //{
            //    Console.WriteLine("There is not ticket available for this title!");
            //    goto Exit;
            //}

            //byte[] ticket = await selectedGame.DownloadTicket("http://wiiu.titlekeys.gq/");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        #region CLI Methods
        static TitleKeyDatabaseEntry CLI_SelectGame(List<TitleKeyDatabaseEntry> keys)
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
                        str += $"{x.Name.Replace("\n", " ")} [{x.Type.Name}] [{x.Region}]";
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
