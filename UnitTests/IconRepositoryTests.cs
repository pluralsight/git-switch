using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class IconRepositoryTests
    {
        private IconRepository classUnderTest;
        private Mock<IIconDownloader> mockIconDownloader;
        private Mock<IFileHandler> mockFileHandler;

        private GitUser gitUser = new GitUser
        {
            Email = "test@example.com"
        };
        private string expectedFilePath = "./55502f40dc8b7c769880b10874abc9d0.jpg";
        
        [SetUp]
        public void Setup()
        {
            mockIconDownloader = new Mock<IIconDownloader>();
            mockFileHandler = new Mock<IFileHandler>();

            classUnderTest = new IconRepository(mockIconDownloader.Object, mockFileHandler.Object);
        }

        [Test]
        public void GetIconForUser_ReturnsACachedIcon()
        {
            mockFileHandler.Setup(x => x.DoesFileExist(expectedFilePath)).Returns(true);

            var result = classUnderTest.GetIconFilePathForUser(gitUser);

            Assert.That(result, Is.EqualTo(expectedFilePath));
            mockIconDownloader.Verify(x => x.DownloadIcon(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetIconForUser_ReturnsADownloadedIcon()
        {
            var expectedUrl = new GravatarUrlBuilder().GetUrlForEmail(gitUser.Email);
            mockFileHandler.Setup(x => x.DoesFileExist(expectedFilePath)).Returns(false);

            var result = classUnderTest.GetIconFilePathForUser(gitUser);

            Assert.That(result, Is.EqualTo(expectedFilePath));
            mockIconDownloader.Verify(x => x.DownloadIcon(expectedUrl, expectedFilePath));
        }
    }
}
