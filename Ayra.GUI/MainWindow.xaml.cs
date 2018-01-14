﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ayra.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isRefreshingTitleKeyDatabase = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TitleKeyDatabase_Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !isRefreshingTitleKeyDatabase;
        private async void TitleKeyDatabase_Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!isRefreshingTitleKeyDatabase)
            {
                await Task.Run(() =>
                {
                    bool success = vm.TitleKeyDatabase.UpdateDatabase("http://wiiu.titlekeys.gq/");
                    vm.OnPropertyChanged(nameof(vm.TitleKeyDatabaseEntries));

                    isRefreshingTitleKeyDatabase = false;
                });
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TitleKeyDatabase_Refresh_Executed(null, null);
        }
    }
}
