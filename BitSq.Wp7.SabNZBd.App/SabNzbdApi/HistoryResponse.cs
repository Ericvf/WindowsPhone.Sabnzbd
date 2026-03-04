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
using System.Collections.Generic;

namespace BitSq.Wp7.SabNZBd.App.SabNzbdApi
{
    public class HistoryResponse
    {
        public class HistoryItem
        {
            public string Name { get; set; }
            public string Size { get; set; }
            public string Status { get; set; }
            public TimeSpan DownloadTime { get; set; }
        }

        public List<HistoryItem> Items { get; set; }

        public HistoryResponse()
        {
            this.Items = new List<HistoryItem>();
        }
    }
}
