using System.IO;
using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    class OptionsControllerTests
    {
        private OptionsController controller;
        private Mock<IFileHandler> mockFileHandler;

        [SetUp]
        public void SetUp()
        {
            mockFileHandler = new Mock<IFileHandler>();
            mockFileHandler.Setup(mock => mock.DeserializeFromFile<GitSwitchOptions>(It.IsAny<string>()))
                .Throws(new FileNotFoundException());

            controller = new OptionsController(mockFileHandler.Object);
        }

        [Test]
        public void ItSetsDefaultValuesWhenNoFileWasLoaded()
        {
            Assert.False(controller.KillSshAgent);
        }

        [Test]
        public void ItTriesToLoadAFileWhenConstructed()
        {
            var optionsFromFile = new GitSwitchOptions
            {
                KillSshAgent = true
            };
            mockFileHandler.Setup(mock => mock.DeserializeFromFile<GitSwitchOptions>(It.IsAny<string>())).Returns(optionsFromFile);
            
            controller = new OptionsController(mockFileHandler.Object);

            Assert.True(controller.KillSshAgent);
            mockFileHandler.Verify(mock => mock.DeserializeFromFile<GitSwitchOptions>(OptionsController.OptionsFile));
        }

        [Test]
        public void ItCanToggleAndSaveTheKillSshAgentOption()
        {
            controller.ToggleKillSshAgent();

            Assert.True(controller.KillSshAgent);

            controller.ToggleKillSshAgent();

            Assert.False(controller.KillSshAgent);
            mockFileHandler.Verify(mock => mock.SerializeToFile(OptionsController.OptionsFile, It.IsAny<GitSwitchOptions>()), Times.Exactly(2));
        }
    }
}
