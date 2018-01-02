using Ayra.Core;
using Ayra.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ayra.GUI.Managers
{
    public class TitleDatabaseManager
    {
        private static readonly Lazy<TitleDatabaseManager> lazy = new Lazy<TitleDatabaseManager>(() => new TitleDatabaseManager());
        public static TitleDatabaseManager Instance => lazy.Value;

        private List<TitleKeyDatabaseEntry> titleKeys;
        public List<TitleKeyDatabaseEntry> TitleKeys
        {
            get
            {
                if (titleKeys == null) RefreshTitleKeys();
                return titleKeys;
            }
        }

        private TitleDatabaseManager()
        {

        }

        /// <summary>
        /// Refresh title keys, also used for the initial load
        /// </summary>
        public void RefreshTitleKeys()
        {
            titleKeys = TitleKeyDatabase.GetTitleKeyDatabaseEntries(Properties.Settings.Default.TitleDatabaseUrl);
        }
    }
}
