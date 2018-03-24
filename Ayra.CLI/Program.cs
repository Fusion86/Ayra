using Ayra.Core.Classes;
using Ayra.Core.Enums;
using Ayra.Core.Extensions;
using Ayra.Core.Helpers.CTR;
using Ayra.Core.Helpers.WUP;
using Ayra.TitleKeyDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Ayra.Core.Structs;
using Ayra.Core.Helpers;

namespace Ayra.CLI
{
    internal static class Program
    {
        const string titleKeyFile3ds = "TitleKeys3DS.json";
        const string titleKeyFileWiiU = "TitleKeyWiiU.json";

        static TitleKeyDatabase.CTR.TitleKeyDatabase titleKeyDatabase3ds = new TitleKeyDatabase.CTR.TitleKeyDatabase();
        static TitleKeyDatabase.WUP.TitleKeyDatabase titleKeyDatabaseWiiU = new TitleKeyDatabase.WUP.TitleKeyDatabase();

        private static async Task Main(string[] args)
        {
            // Setup Ayra.Core logger
            Ayra.Core.Logging.LogProvider.SetCurrentLogProvider(new ColoredConsoleLogProvider());

            Console.WriteLine($"Ayra.CLI v{Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"Ayra.Core v{Assembly.GetAssembly(typeof(TitleKeyDatabaseEntryBase)).GetName().Version}");

            // Load TitleKeys (if possible)
            if (File.Exists(titleKeyFile3ds))
                titleKeyDatabase3ds.LoadDatabase(titleKeyFile3ds);

            if (File.Exists(titleKeyFileWiiU))
                titleKeyDatabaseWiiU.LoadDatabase(titleKeyFileWiiU);

            // Menu options
            List<(string Name, Func<Task> Action)> menuOptions = new List<(string, Func<Task>)>
            {
                ( "Download N3DS game", CLI_N3DS_Download ),
                ( "Download Wii U game", CLI_WiiU_Download ),
                ( "Download N3DS/Wii U game by Title ID", CLI_3DS_WiU_Download_TitleId ),
                ( "Decrypt Wii U game", CLI_WiiU_Decrypt ),
                ( "Download TitleKeys from that one site", CLI_TKDB_Download ),
            };

            menu:
            Console.WriteLine("\nNintendo 3DS TitleKey count: " + titleKeyDatabase3ds.Entries.Count);
            Console.WriteLine("Nintendo Wii U TitleKey count: " + titleKeyDatabaseWiiU.Entries.Count);

            Console.WriteLine("\nMenu:");
            for (int i = 0; i < menuOptions.Count; i++)
            {
                string str = "  " + (i + 1).ToString().PadRight(4);
                str += menuOptions[i].Name;
                Console.WriteLine(str);
            }
            Console.WriteLine("  q   Quit\n");

            int selectedNumber = 0;
            do
            {
                Console.Write("Enter option: ");
                string str = Console.ReadLine();
                if (str.ToCharArray()[0] == 'q') return; // Exit

                int.TryParse(str, out selectedNumber);
            } while (selectedNumber == 0 || selectedNumber > menuOptions.Count);

            // Run selected cli method
            await menuOptions[selectedNumber - 1].Action();

            goto menu;
        }

        #region Nintendo 3DS

        private static async Task CLI_N3DS_Download()
        {
            if (titleKeyDatabase3ds.Entries.Count == 0)
            {
                Console.WriteLine("No TitleKeys available! Download or import them from the main menu.");
                return;
            }

            var selectedGame = (TitleKeyDatabase.CTR.TitleKeyDatabaseEntry)CLI_SelectGame(titleKeyDatabase3ds.Entries);

            // TODO: ticket stuff

            Console.Write("Download game? [y/n]: ");
            if (!CLI_GetConfirmation(true)) return;

            Console.Write("Make CIA? [y/n]: ");
            bool makeCia = CLI_GetConfirmation(true);

            Console.WriteLine("Downloading game, this might take a while...");
            var game = await Core.Models.CTR.Game.GetFromNus(selectedGame.TitleId);
            // byte[] ticketData = await selectedGame.DownloadTicket();
            // game.Ticket = Core.Models.WUP.Ticket.Load(ticketData);

            NUSClientN3DS client = new NUSClientN3DS();
            await client.DownloadTitle(game.Tmd, game.LocalPath);
            Console.WriteLine("Downloading completed!");

            if (makeCia)
                MakeCdnCia.MakeCia(game.Tmd, game.Ticket, game.LocalPath, "game.cia");
        }

        #endregion Nintendo 3DS

        #region Nintendo Wii U

        private static async Task CLI_WiiU_Download()
        {
            if (titleKeyDatabaseWiiU.Entries.Count == 0)
            {
                Console.WriteLine("No TitleKeys available! Download or import them from the main menu.");
                return;
            }

            var selectedGame = (TitleKeyDatabase.WUP.TitleKeyDatabaseEntry)CLI_SelectGame(titleKeyDatabaseWiiU.Entries);

            if (!selectedGame.HasTicket)
            {
                Console.WriteLine("There is no ticket available for this title!");
                Console.WriteLine("This means that you won't be able to decrypt the game, unless you supply the ticket yourself.");
                Console.Write("Continue? [y/n]: ");
                if (CLI_GetConfirmation(true) == false) return;
            }

            Console.Write("Download game? [y/n]: ");
            if (CLI_GetConfirmation(true) == false) return;

            Console.WriteLine("Downloading game, this might take a while...");
            var game = await Core.Models.WUP.Game.GetFromNus(selectedGame.TitleId);

            // Download game
            NUSClientWiiU client = new NUSClientWiiU();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            long[] lastReportMs = new long[game.Tmd.Header.ContentCount];
            Progress<DownloadContentProgress> progress = new Progress<DownloadContentProgress>();
            progress.ProgressChanged += (s, e) =>
            {
                // Throttle reporting to once per second (per content).
                if (lastReportMs[e.ContentIndex] + 1000 < sw.ElapsedMilliseconds)
                {
                    lastReportMs[e.ContentIndex] = sw.ElapsedMilliseconds;

                    string str = (e.ContentIndex + 1) + "/" + game.Tmd.Header.ContentCount; // +1 because zero indexed
                    str += " received: " + Utility.GetSizeString(e.BytesReceived) + " of " + Utility.GetSizeString(e.TotalBytesToReceive);
                    str += "    " + (float)e.BytesReceived / e.TotalBytesToReceive * 100 + "%"; // Cast to float to make sure we don't ignore decimals

                    Console.WriteLine(str);
                }
            };

            await client.DownloadTitleParallel(game.Tmd, game.LocalPath, progress);

            sw.Stop();
            Console.WriteLine("Download completed in " + sw.Elapsed.ToReadableString());

            if (selectedGame.HasTicket)
            {
                Console.Write("Decrypt contents? [y/n]: ");
                if (CLI_GetConfirmation(true))
                {
                    Console.WriteLine("Downloading ticket...");
                    byte[] ticketData = await selectedGame.DownloadTicket();
                    game.Ticket = Core.Models.WUP.Ticket.Load(ticketData);


                    Console.WriteLine("Decrypting game...");
                    CDecrypt.DecryptContents(game.Tmd, game.Ticket, game.LocalPath);
                }
            }
        }

        private static async Task CLI_WiiU_Decrypt()
        {
            string path = "";
            while (true)
            {
                Console.Write("Enter path to CDN contents: ");
                path = Console.ReadLine();

                if (Directory.Exists(path)) break;
                else Console.WriteLine("Invalid path!");
            }

            bool isMissingFiles = false;
            string[] requiredFiles = new[] { "tmd", "cetk" };

            do
            {
                foreach (string file in requiredFiles)
                {
                    if (!File.Exists(Path.Combine(path, file)))
                    {
                        isMissingFiles = true;
                        Console.WriteLine($"Missing {file}!");
                    }
                }

                if (isMissingFiles)
                {
                    Console.Write("Try to download the missing files? [y/n]: ");
                    if (CLI_GetConfirmation())
                    {
                        CDecrypt.DownloadMissingFiles(path);
                    }
                    else
                    {
                        Console.WriteLine("Can't continue without the required files!");
                        return;
                    }
                }
            } while (isMissingFiles);

            byte[] data = await File.ReadAllBytesAsync(Path.Combine(path, "tmd"));
            var tmd = Core.Models.WUP.TMD.Load(data);

            data = await File.ReadAllBytesAsync(Path.Combine(path, "cetk"));
            var ticket = Core.Models.WUP.Ticket.Load(data);

            CDecrypt.DecryptContents(tmd, ticket, path);
        }

        #endregion Nintendo Wii U

        private static async Task CLI_3DS_WiU_Download_TitleId()
        {
            string titleId = null;
            while (true)
            {
                Console.Write("Enter 16 character Title ID: ");
                titleId = Console.ReadLine().TrimEnd('\n');
                Console.WriteLine();

                if (titleId.Length != 16)
                {
                    Console.WriteLine("Invalid length!");
                }

                throw new NotImplementedException();
            }
        }

        private static async Task CLI_TKDB_Download()
        {
            Console.WriteLine("You can save the downloaded TitleKeys so that this program can autoamtically import them the next time you run it.\nIf you don't save them now then you'll need to download them again after you close this program.");
            Console.Write("Save TitleKeys? [y/n]: ");
            bool storeLocalCopy = CLI_GetConfirmation();

            Console.WriteLine("Downloading TitleKeys for Nintendo 3DS...");
            titleKeyDatabase3ds.UpdateDatabase(storeLocalCopy, titleKeyFile3ds);
            Console.WriteLine($"Downloaded {titleKeyDatabase3ds.Entries.Count} Title Keys");

            Console.WriteLine("Downloading TitleKeys for Nintendo Wii U...");
            titleKeyDatabaseWiiU.UpdateDatabase(storeLocalCopy, titleKeyFileWiiU);
            Console.WriteLine($"Downloaded {titleKeyDatabaseWiiU.Entries.Count} Title Keys");
        }

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="requireValidAnswer">Require either y, yes, n or no as answer. If false then return false on invalid answer.</param>
        /// <returns>True if user entered y or yes</returns>
        private static bool CLI_GetConfirmation(bool requireValidAnswer = false)
        {
            while (true)
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
