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
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace BitSq.Wp7.SabNZBd.App.SabNzbdApi
{
    public class QueueFactory
    {
        public static ObservableCollection<JobItem> Jobs { get; set; }

        public class JobItem
        {
            public string Title { get; set; }
            public double Mb { get; set; }
            public double MbLeft { get; set; }
            public string TimeLeft { get; set; }
        }

        static WebClient client = new WebClient();

        static QueueFactory()
        {
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
        }
        
        public static void Update()
        {
            client.DownloadStringAsync(new Uri("http://sabnzbd:8080/api?mode=qstatus&output=xml&apikey="));
        }

        static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Check for errors with the request
            if (e.Error != null)
            {
                // TODO:
                return;
            }

            // Parse the result into a linq to xml container
            var result = XElement.Parse(e.Result);
            var jobs = result.Element("jobs").Descendants("job");

            Jobs.Clear();

            foreach (var job in jobs)
            {
                var jobItem = new JobItem()
                {
                    Title = job.Element("filename").Value,
                    Mb = double.Parse(job.Element("mb").Value),
                    MbLeft = double.Parse(job.Element("mbleft").Value),
                    TimeLeft = job.Element("timeleft").Value
                };

                Jobs.Add(jobItem);
            }
        }
    }
}
