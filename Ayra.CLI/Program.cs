using Ayra.Core.Classes;
using Ayra.Core.Enums;
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
            Console.WriteLine($"Ayra.CLI v{Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"Ayra.Core v{Assembly.GetAssembly(typeof(TitleKeyDatabaseEntryBase)).GetName().Version}\n");

            List<(string Name, Func<Task> Action)> menuOptions = new List<(string, Func<Task>)>
            {
                ( "Download N3DS Games", CLI_N3DS_Download ),
                ( "Download Wii U Games", CLI_WiiU_Download ),
            };

            Console.WriteLine("Menu:");
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
            await menuOptions[selectedNumber - 1].Action();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        #region Nintendo 3DS

        private static async Task CLI_N3DS_Download()
        {
            TitleKeyDatabase.CTR.TitleKeyDatabase titleKeyDatabase = new TitleKeyDatabase.CTR.TitleKeyDatabase();

            Console.WriteLine("Downloading Title Keys...");
            titleKeyDatabase.UpdateDatabase();
            Console.WriteLine($"Downloaded {titleKeyDatabase.Entries.Count} Title Keys\n");

            var selectedGame = (TitleKeyDatabase.CTR.TitleKeyDatabaseEntry)CLI_SelectGame(titleKeyDatabase.Entries);

            // TODO: ticket stuff

            Console.Write("Download game? [y/n]: ");
            if (!CLI_GetConfirmation(true)) return;

            var game = await Core.Models.CTR.Game.GetFromNus(selectedGame.TitleId);
            // byte[] ticketData = await selectedGame.DownloadTicket();
            // game.Ticket = Core.Models.WUP.Ticket.Load(ref ticketData);

            // NUSClientN3DS client = new NUSClientN3DS();
            // await client.DownloadTitle();
        }

        #endregion Nintendo 3DS

        #region Nintendo Wii U

        private static async Task CLI_WiiU_Download()
        {
            // Load game database and select game
            TitleKeyDatabase.WUP.TitleKeyDatabase titleKeyDatabase = new TitleKeyDatabase.WUP.TitleKeyDatabase();

            Console.WriteLine("Downloading Title Keys...");
            titleKeyDatabase.UpdateDatabase();
            Console.WriteLine($"Downloaded {titleKeyDatabase.Entries.Count} Title Keys");

            var selectedGame = (TitleKeyDatabase.WUP.TitleKeyDatabaseEntry)CLI_SelectGame(titleKeyDatabase.Entries);

            if (!selectedGame.HasTicket)
            {
                Console.WriteLine("There is no ticket available for this title!");
                return;
            }

            Console.Write("Download game? [y/n]: ");
            if (!CLI_GetConfirmation(true)) return;

            var game = await Core.Models.WUP.Game.GetFromNus(selectedGame.TitleId);
            byte[] ticketData = await selectedGame.DownloadTicket();
            game.Ticket = Core.Models.WUP.Ticket.Load(ref ticketData);

            // Download game
            NUSClientWiiU client = new NUSClientWiiU();
            await client.DownloadTitle(game.Tmd, "download");
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

                if (searchResults.Count > 1)
                {
                    Console.WriteLine("Games found:");
                    for (int i = 0; i < searchResults.Count; i++)
                    {
                        TitleKeyDatabaseEntryBase x = searchResults[i];

                        string str = "  " + (i + 1).ToString().PadRight(4);
                        str += $"{x.Name.Replace("\n", " ")} [{NSoftwareType.GetByTitleId(x.TitleId)?.Name}] [{x.Region}]";
                        str += $" [{x.TitleId.ToUpper()}]";

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
                    TitleKeyDatabaseEntryBase x = searchResults[0];
                    Console.WriteLine($"{x.Name.Replace("\n", " ")} [{NSoftwareType.GetByTitleId(x.TitleId)?.Name}] [{x.Region}]");
                    return searchResults[0];
                }
                else
                {
                    Console.WriteLine("Nothing found, try again.");
                }
            }
        }

        private static bool CLI_GetConfirmation(bool requireValidAnswer = false)
        {
            while(true)
            {
                string answer = Console.ReadLine().ToLower();

                if (answer == "y" || answer == "yes") return true;
                else if (answer == "n" || answer == "no") return false;
                else if (!requireValidAnswer) return false;
            }
        }

        #endregion Shared
    }
}
