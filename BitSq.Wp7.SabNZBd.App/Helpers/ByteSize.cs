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

namespace BitSq.Wp7.SabNZBd.App.Helpers
{
    public class ByteSize
    {
        public static string GetBytesFromMB(double bytes)
        {
            if (bytes == 0) return bytes.ToString() + " Bytes";
            bytes = bytes * 1024 * 1024;

            if (bytes >= 1073741824)
            {
                bytes = (bytes / 1024 / 1024 / 1024);
                return bytes.ToString("0.00") + " GB";
            }

            if (bytes >= 1048576)
            {
                bytes = (bytes / 1024 / 1024);
                return bytes.ToString("0.0") + " MB";
            }

            if (bytes >= 1024)
            {
                bytes = (bytes / 1024);
                return bytes.ToString("0.0") + " KB";
            }

            return bytes.ToString() + " Bytes";
        }

    }
}
