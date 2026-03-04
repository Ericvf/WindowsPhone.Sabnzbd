using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;

namespace BitSq.Wp7.SabNZBd.App
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.queuePage.DataContext = App.QueueuViewModel;
            this.historyPage.DataContext = App.HistoryViewModel;
            this.infoPage.DataContext = App.QueueuViewModel;
            this.queuePage.DataContext = App.QueueuViewModel;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            UpdateAuthenticationType();

            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.QueueuViewModel.LoadData();
            App.HistoryViewModel.LoadData(); 
            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            App.QueueuViewModel.UpdateCalculations();
        }

        private void authenticationTypeChanged(object sender, RoutedEventArgs e)
        {
            UpdateAuthenticationType();
        }

        private void UpdateAuthenticationType()
        {
            if (this.spUserNameAndPassword == null)
                return;

            bool isUserName = this.rbUserNameAndPassword.IsChecked.Value;

            this.spUserNameAndPassword.Visibility = isUserName
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;

            this.spApiKey.Visibility = !isUserName
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
        }
    }
}