using System.Drawing;
using System.IO;
using System.Net;

namespace GitSwitch
{
    public class IconDownloader : IIconDownloader
    {
        public void DownloadIcon(string url, string filePath)
        {
            var bytes = new WebClient().DownloadData(url);
            var bitmap = new Bitmap(new MemoryStream(bytes));
            var icon = Icon.FromHandle(bitmap.GetHicon());
            icon.Save(new FileStream(filePath, FileMode.Create));
        }
    }
}
