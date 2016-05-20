using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class IconRepositoryTests
    {
        IconRepository classUnderTest;
        Mock<IIconDownloader> mockIconDownloader;
        Mock<IFileHandler> mockFileHandler;
        Mock<IGravatarUrlBuilder> mockGravatarUrlBuilder;

        const string ExpectedFileName = "55502f40dc8b7c769880b10874abc9d0";
        readonly string expectedFilePath = string.Format("./{0}.jpg", ExpectedFileName);
        readonly GitUser gitUser = new GitUser("user-name", "test@example.com", "key-path");

        [SetUp]
        public void Setup()
        {
            mockIconDownloader = new Mock<IIconDownloader>();
            mockFileHandler = new Mock<IFileHandler>();
            mockGravatarUrlBuilder = new Mock<IGravatarUrlBuilder>();

            classUnderTest = new IconRepository(mockIconDownloader.Object, mockFileHandler.Object, mockGravatarUrlBuilder.Object);
        }

        [Test]
        public void GetIconFilePathForUser_ReturnsACachedIcon()
        {
            mockFileHandler.Setup(x => x.DoesFileExist(expectedFilePath)).Returns(true);
            mockGravatarUrlBuilder.Setup(x => x.HashEmail(gitUser.Email)).Returns(ExpectedFileName);

            var result = classUnderTest.GetIconFilePathForUser(gitUser);

            Assert.That(result, Is.EqualTo(expectedFilePath));
            mockIconDownloader.Verify(x => x.DownloadIcon(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetIconFilePathForUser_ReturnsADownloadedIcon()
        {
            mockFileHandler.Setup(x => x.DoesFileExist(expectedFilePath)).Returns(false);
            mockGravatarUrlBuilder.Setup(x => x.HashEmail(gitUser.Email)).Returns(ExpectedFileName);

            var expectedUrl = string.Format(AppConstants.GravatarUrlFormat, ExpectedFileName);
            mockGravatarUrlBuilder.Setup(x => x.GetUrlForEmail(gitUser.Email)).Returns(expectedUrl);

            var result = classUnderTest.GetIconFilePathForUser(gitUser);

            Assert.That(result, Is.EqualTo(expectedFilePath));
            mockIconDownloader.Verify(x => x.DownloadIcon(expectedUrl, expectedFilePath));
            mockGravatarUrlBuilder.Verify(x => x.GetUrlForEmail(gitUser.Email));
        }
    }
}
