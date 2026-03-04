using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace BitSq.Wp7.SabNZBd.App.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        #region Classes

        /// <summary>
        /// 
        /// </summary>
        public class HistoryItem : INotifyPropertyChanged
        {
            public string Name { get; set; }
            public string Size { get; set; }
            public string Status { get; set; }
            public TimeSpan DownloadTime { get; set; }

            #region INotifyPropertyChanged
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (null != handler)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion
        }

        #endregion

        /// <summary>
        /// Returns a collection of QueueItems
        /// </summary>
        public ObservableCollection<HistoryItem> HistoryItems { get; private set; }

        public HistoryViewModel()
        {
            this.HistoryItems = new ObservableCollection<HistoryItem>();
        }

        public void LoadData()
        {
            var client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri("http://sabnzbd:8080/api?mode=history&output=xml&apikey="));
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Check for errors with the request
            if (e.Error != null)
            {
                // TODO:
                return;
            }

            // Parse the result into a linq to xml container
            var result = XElement.Parse(e.Result);
            var slots = result.Descendants("slot");

            // Check for downloaded
            this.HistoryItems.Clear();
            foreach (var slot in slots)
            {
                if (slot.Parent != null && slot.Parent.Name.ToString() != "slots")
                    continue;

                var h = new HistoryItem()
                {
                    Name = slot.Element("name").Value,
                    Size = slot.Element("size").Value,
                    Status = slot.Element("status").Value,
                    DownloadTime = TimeSpan.FromSeconds(int.Parse(slot.Element("download_time").Value))
                };

                this.HistoryItems.Add(h);
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
