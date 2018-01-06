using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Ayra.TitleKeyDatabase.Wii_U
{
    /// <summary>
    /// Only useful when using a website based on TomEke/WiiU-Title-Key
    /// </summary>
    public class TitleKeyDatabase
    {
        private List<TitleKeyDatabaseEntry> entries = new List<TitleKeyDatabaseEntry>();
        public IReadOnlyList<TitleKeyDatabaseEntry> Entries => entries.AsReadOnly();
        
        private List<TitleKeyDatabaseEntry> ParseJson(string json) => JsonConvert.DeserializeObject<List<TitleKeyDatabaseEntry>>(json);
        
        /// <summary>
        /// Update database from website
        /// </summary>
        /// <param name="url"></param>
        /// <param name="storeLocalCopy"></param>
        /// <returns></returns>
        public bool UpdateDatabase(string url, bool storeLocalCopy = false)
        {
            try
            {
                string json = new WebClient().DownloadString(url + "/json");

                if (storeLocalCopy) File.WriteAllText("TitleKeyDatabase.json", json);
                entries = ParseJson(json);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Load database from local json file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadDatabase(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                entries = ParseJson(json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
