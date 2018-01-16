using Ayra.Core.Classes;
using Ayra.Core.Models;
using Ayra.TitleKeyDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ayra.CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine($"  Ayra.CLI v{Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"  Ayra.Core v{Assembly.GetAssembly(typeof(TitleKeyDatabaseEntryBase)).GetName().Version}\n");

            List<(string Name, Action Action)> menuOptions = new List<(string, Action)>
            {
                ( "Download N3DS Games", CLI_N3DS_Download ),
                ( "Download Wii U Games", CLI_WiiU_Download ),
            };

            Console.WriteLine("Menu:\n");
            for (int i = 0; i < menuOptions.Count; i++)
            {
                string str = "  " + (i + 1).ToString().PadRight(4);
                str += menuOptions[i].Name;
                Console.WriteLine(str);

                if (i == menuOptions.Count - 1) Console.WriteLine();
            }

            int selectedNumber = 0;
            do
            {
                Console.Write("Enter number: ");
                string str = Console.ReadLine();
                int.TryParse(str, out selectedNumber);
            } while (selectedNumber == 0 || selectedNumber > menuOptions.Count);

            // Run selected cli method
            menuOptions[selectedNumber - 1].Action();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        #region Nintendo 3DS

        private static async void CLI_N3DS_Download()
        {
        }

        #endregion Nintendo 3DS

        #region Nintendo Wii U

        private static async void CLI_WiiU_Download()
        {
            TitleKeyDatabase.Wii_U.TitleKeyDatabase titleKeyDatabase = new TitleKeyDatabase.Wii_U.TitleKeyDatabase();

            Console.WriteLine("Downloading Title Keys...");
            titleKeyDatabase.UpdateDatabase();
            Console.WriteLine($"Downloaded {titleKeyDatabase.Entries.Count} Title Keys\n");

            var selectedGame = (TitleKeyDatabase.Wii_U.TitleKeyDatabaseEntry)CLI_SelectGame(titleKeyDatabase.Entries);

            NUSClientWiiU client = new NUSClientWiiU();

            Console.WriteLine("Downloading metadata...");
            var tmd = await client.DownloadTMD(selectedGame.Id);

            if (!selectedGame.HasTicket)
            {
                Console.WriteLine("There is not ticket available for this title!");
                return;
            }

            byte[] ticket = await selectedGame.DownloadTicket();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        #endregion Nintendo Wii U

        #region Shared

        private static TitleKeyDatabaseEntryBase CLI_SelectGame(IEnumerable<TitleKeyDatabaseEntryBase> keys)
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

                        string str = "  " + (i + 1).ToString().PadRight(4);
                        str += $"{x.Name.Replace("\n", " ")} [{NSoftwareTypes.GetById(x.Id).Name}] [{x.Region}]";
                        str += $" [{x.Id}]";

                        Console.WriteLine(str);

                        if (i == searchResults.Count - 1) Console.WriteLine();
                    }

                    int selectedNumber = 0;
                    do
                    {
                        Console.Write("Enter number: ");
                        string str = Console.ReadLine();
                        int.TryParse(str, out selectedNumber);
                    } while (selectedNumber == 0 || selectedNumber > searchResults.Count);

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

        #endregion Shared
    }
}
