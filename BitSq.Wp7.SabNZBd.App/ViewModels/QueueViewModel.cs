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
using System.Collections.ObjectModel;
using System.Xml.Linq;
using BitSq.Wp7.SabNZBd.App.Helpers;

namespace BitSq.Wp7.SabNZBd.App.ViewModels
{
    public class QueueViewModel : INotifyPropertyChanged
    {
        #region Classes

        /// <summary>
        /// 
        /// </summary>
        public class QueueItem : INotifyPropertyChanged
        {
            public string Title { get; set; }
            public double Mb { get; set; }
            public double MbLeft { get; set; }
            public string TimeLeft { get; set; }
            public string SabNzbdId { get; set; }
            public double Speed { get; set; }
            public string Status { get; set; }

            private DateTime startTime;
            public QueueItem()
            {
                this.startTime = DateTime.Now;
            }

            public string StatusFriendly
            {
                get
                {
                    return string.Format("{0}: {1} of {2}", this.Status, ByteSize.GetBytesFromMB(estimatedMbLeft), ByteSize.GetBytesFromMB(Mb)) +
                        Environment.NewLine + string.Format("Time left: {0:0}:{1:00}:{2:00}", 
                            EstimatedTimeLeft.TotalHours, 
                            EstimatedTimeLeft.Minutes, 
                            EstimatedTimeLeft.Seconds);
                }
            }

            public string Percentage
            {
                get
                {
                    return string.Format("{0}%", estimatePercentage);
                }
            }

            
            public double ProgressWidth
            {
                get
                {
                    return estimatePercentage * 4.2;
                }
            }
            
            private double estimatedMbLeft;
            private double estimatePercentage;
            public void UpdateCalculations()
            {
                if (!this.Status.Equals("Downloading")) return;

                var d = TimeSpan.FromTicks(DateTime.Now.Ticks - this.startTime.Ticks);
                estimatedMbLeft = MbLeft - ((this.Speed / 1024) * d.TotalSeconds);
                estimatePercentage = Math.Round(((Mb - estimatedMbLeft) / Mb) * 100);

                if (estimatedMbLeft < 0)
                {
                    this.Status = "Completed";
                    estimatePercentage = 100;
                }

                NotifyPropertyChanged("ProgressWidth");
                NotifyPropertyChanged("Percentage");
                NotifyPropertyChanged("StatusFriendly");
            }


            public TimeSpan EstimatedTimeLeft
            {
                get
                {
                    var d = DateTime.Now - this.startTime;
                    return TimeSpan.Parse(this.TimeLeft) - TimeSpan.FromTicks(d.Ticks);
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
        
        #endregion

        #region Members

        
        public int QueueLength { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public string DiskSpace { get; set; }

        public string FreeSpaceFriendly
        {
            get
            {
                if (string.IsNullOrEmpty(this.DiskSpace))
                    return string.Empty;

                var freeSpace = ByteSize.GetBytesFromMB(double.Parse(this.DiskSpace) * 1024);
                return string.Format("Free space: {0}", freeSpace);
            }
        }


        public string VersionFriendly
        {
            get
            {
                return string.Format("SabNZBd version: {0}", this.Version);
            }
        }


        public string SizeTotalFriendly
        {
            get
            {
                return string.Format("Size total: {0}", this.SizeTotal);
            }
        }


        public string SizeRemainingFriendly
        {
            get
            {
                return string.Format("Size remaining: {0}", ByteSize.GetBytesFromMB(this.SizeRemaining));
            }
        }



        public string QueueLengthFriendly
        {
            get
            {
                return string.Format("Queue length: {0}", this.QueueLength);
            }
        }


        public string Percentage
        {
            get
            {
                return string.Format("{0:0}%", estimatePercentage * 100);
            }
        }

        public double ProgressWidth
        {
            get
            {
                return estimatePercentage * 420;
            }
        }

        /// <summary>
        /// Gets the number of warnings
        /// </summary>
        public int Warnings { get; private set; }

        /// <summary>
        /// Gets the remaining time of the download queue
        /// </summary>
        public TimeSpan TimeLeft { get; private set; }

        /// <summary>
        /// Gets the total size of the download queue
        /// </summary>
        public double SizeTotal { get; private set; }

        /// <summary>
        /// Gets the global pause state
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Gets the remaining size of the download queue
        /// </summary>
        public double SizeRemaining { get; set; }

        /// <summary>
        /// Gets the download speed (kbps)
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Returns a collection of QueueItems
        /// </summary>
        public ObservableCollection<QueueItem> QueueItems { get; private set; }
        #endregion

        /// <summary>
        /// Initializes the class
        /// </summary>
        public QueueViewModel()
        {
            this.QueueItems = new ObservableCollection<QueueItem>();
        }

        /// <summary>
        /// Loads data from the SabNzbd server
        /// </summary>
        public void LoadData()
        {
            var client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri("http://sabnzbd:8080/api?mode=queue&output=xml&apikey="));
        }

        /// <summary>
        /// Updates the calculations
        /// </summary>
        /// 
        private double estimatePercentage;
        public void UpdateCalculations()
        {
            this.estimatePercentage = ((this.SizeTotal - this.SizeRemaining) / this.SizeTotal);

            this.NotifyPropertyChanged("VersionFriendly");
            this.NotifyPropertyChanged("FreeSpaceFriendly");
            this.NotifyPropertyChanged("QueueLengthFriendly");
            this.NotifyPropertyChanged("ProgressWidth");
            this.NotifyPropertyChanged("Percentage");
            this.NotifyPropertyChanged("SizeRemainingFriendly");
            
            foreach (var item in this.QueueItems)
                item.UpdateCalculations();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Check for errors with the request
            if (e.Error != null)
            {
                // TODO:
                return;
            }

            // Parse the result into a linq to xml container
            var result = XElement.Parse(e.Result);

            this.Version = result.Element("version").Value;
            this.Status = result.Element("status").Value;
            this.DiskSpace = result.Element("diskspace1").Value;

            //
            this.Warnings = int.Parse(result.Element("have_warnings").Value);
            this.TimeLeft = TimeSpan.Parse(result.Element("timeleft").Value);
            this.SizeTotal = double.Parse(result.Element("mb").Value);
            this.SizeRemaining = double.Parse(result.Element("mbleft").Value);
            this.IsPaused = bool.Parse(result.Element("paused").Value);
            this.Speed = double.Parse(result.Element("kbpersec").Value);

            var jobs = result.Element("slots").Descendants("slot");
            this.QueueItems.Clear();
            foreach (var job in jobs)
            {
                var jobItem = new QueueItem()
                {
                    Title = job.Element("filename").Value,
                    Mb = double.Parse(job.Element("mb").Value),
                    MbLeft = double.Parse(job.Element("mbleft").Value),
                    TimeLeft = job.Element("timeleft").Value,
                    SabNzbdId = job.Element("nzo_id").Value,
                    Status = job.Element("status").Value,
                    Speed = this.Speed
                };

                this.QueueItems.Add(jobItem);
            }

            this.QueueLength = this.QueueItems.Count;

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
