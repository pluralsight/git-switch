using System.IO;
using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    class OptionsManagerTests
    {
        OptionsManager classUnderTest;
        Mock<IFileHandler> mockFileHandler;

        GitSwitchOptions expectedOptions;

        [SetUp]
        public void SetUp()
        {
            mockFileHandler = new Mock<IFileHandler>();
            expectedOptions = new GitSwitchOptions { KillSshAgent = true, UsersFile = "path/to/file.xml" };

            classUnderTest = new OptionsManager(mockFileHandler.Object);
        }

        [Test]
        public void DefaultState()
        {
            mockFileHandler.Setup(mock => mock.DeserializeFromFile<GitSwitchOptions>(AppConstants.OptionsFile)).Throws(new FileNotFoundException());

            Assert.That(classUnderTest.KillSshAgent, Is.False);
            Assert.That(classUnderTest.UsersFile, Is.EqualTo(AppConstants.UsersFile));
        }

        [Test]
        public void Options_AreLazyLoadedFromFile()
        {
            mockFileHandler.Setup(mock => mock.DeserializeFromFile<GitSwitchOptions>(AppConstants.OptionsFile)).Returns(expectedOptions);

            Assert.That(classUnderTest.KillSshAgent, Is.EqualTo(expectedOptions.KillSshAgent));
            Assert.That(classUnderTest.UsersFile, Is.EqualTo(expectedOptions.UsersFile));
            mockFileHandler.Verify(mock => mock.DeserializeFromFile<GitSwitchOptions>(AppConstants.OptionsFile));
        }

        [Test]
        public void ToggleKillSshAgent_Toggles()
        {
            mockFileHandler.Setup(mock => mock.DeserializeFromFile<GitSwitchOptions>(AppConstants.OptionsFile)).Returns(expectedOptions);

            classUnderTest.ToggleKillSshAgent();

            Assert.That(classUnderTest.KillSshAgent, Is.False);

            classUnderTest.ToggleKillSshAgent();

            Assert.That(classUnderTest.KillSshAgent, Is.True);
            mockFileHandler.Verify(mock => mock.SerializeToFile(AppConstants.OptionsFile, It.IsAny<GitSwitchOptions>()), Times.Exactly(2));
        }
    }
}
