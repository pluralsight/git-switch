using System.Net;

namespace GitSwitch
{
    public interface IIconDownloader
    {
        void DownloadIcon(string url, string filePath);
    }

    public class IconDownloader : IIconDownloader
    {
        public void DownloadIcon(string url, string filePath)
        {
            new WebClient().DownloadFile(url, filePath);
        }
    }
}
