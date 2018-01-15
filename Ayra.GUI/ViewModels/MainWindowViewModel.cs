using System.Collections.ObjectModel;

namespace Ayra.GUI.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public TitleKeyDatabase.Wii_U.TitleKeyDatabase TitleKeyDatabase = new TitleKeyDatabase.Wii_U.TitleKeyDatabase();
        public ObservableCollection<TitleKeyDatabase.Wii_U.TitleKeyDatabaseEntry> TitleKeyDatabaseEntries => new ObservableCollection<TitleKeyDatabase.Wii_U.TitleKeyDatabaseEntry>(TitleKeyDatabase.Entries);
    }
}
