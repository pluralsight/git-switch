using System.Drawing;

namespace GitSwitch
{
    public interface IIconRepository
    {
        Icon GetIconForUser(GitUser gitUser);
    }

    public class IconRepository : IIconRepository
    {
        readonly IIconDownloader iconDownloader;
        readonly IFileHandler fileHandler;
        readonly IGravatarUrlBuilder gravatarUrlBuilder;

        public IconRepository(IIconDownloader iconDownloader, IFileHandler fileHandler, IGravatarUrlBuilder gravatarUrlBuilder)
        {
            this.iconDownloader = iconDownloader;
            this.fileHandler = fileHandler;
            this.gravatarUrlBuilder = gravatarUrlBuilder;
        }

        public Icon GetIconForUser(GitUser gitUser)
        {
            return Icon.FromHandle(new Bitmap(GetIconFilePathForUser(gitUser)).GetHicon());
        }

        internal string GetIconFilePathForUser(GitUser gitUser)
        {
            var fileName = string.Format("./{0}.jpg", gravatarUrlBuilder.HashEmail(gitUser.Email));

            if (!fileHandler.DoesFileExist(fileName))
                iconDownloader.DownloadIcon(gravatarUrlBuilder.GetUrlForEmail(gitUser.Email), fileName);

            return fileName;
        }
    }
}
