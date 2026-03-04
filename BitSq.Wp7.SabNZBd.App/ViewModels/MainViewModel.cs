using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml.Linq;

using System.Linq;
using BitSq.Wp7.SabNZBd.App.SabNzbdApi;


namespace BitSq.Wp7.SabNZBd.App
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
        }



        public bool IsDataLoaded
        {
            get;
            private set;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void LoadData()
        {
            this.IsDataLoaded = true;
        }
    }
}