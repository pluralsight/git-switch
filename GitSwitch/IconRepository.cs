using System.Drawing;

namespace GitSwitch
{
    public class IconRepository
    {
        private readonly IFileHandler fileHandler;
        private readonly IIconDownloader iconDownloader;

        public IconRepository(IIconDownloader iconDownloader, IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
            this.iconDownloader = iconDownloader;
        }

        public string GetIconFilePathForUser(GitUser gitUser)
        {
            var gravitarUrlBuilder = new GravatarUrlBuilder();
            var fileName = string.Format("./{0}.jpg", gravitarUrlBuilder.HashEmail(gravitarUrlBuilder.NormalizeEmail(gitUser.Email)));

            if (!fileHandler.DoesFileExist(fileName))
                iconDownloader.DownloadIcon(gravitarUrlBuilder.GetUrlForEmail(gitUser.Email), fileName);

            return fileName;
        }

        public Icon GetIconForUser(GitUser gitUser)
        {
            return Icon.FromHandle(new Bitmap(GetIconFilePathForUser(gitUser)).GetHicon());
        }
    }
}
