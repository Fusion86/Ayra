using System.Collections.ObjectModel;

namespace Ayra.GUI.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public TitleKeyDatabase.WUP.TitleKeyDatabase TitleKeyDatabase = new TitleKeyDatabase.WUP.TitleKeyDatabase();
        public ObservableCollection<TitleKeyDatabase.WUP.TitleKeyDatabaseEntry> TitleKeyDatabaseEntries => new ObservableCollection<TitleKeyDatabase.WUP.TitleKeyDatabaseEntry>(TitleKeyDatabase.Entries);
    }
}
