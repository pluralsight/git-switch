using System.Collections.Generic;
using System.IO;
using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    class GitConfigEditorTests
    {
        const string TestUsername = "John Doe";
        const string TestEmail = "john-doe@example.com";

        GitConfigEditor classUnderTest;
        Mock<IFileHandler> mockFileHandler;

        [SetUp]
        public void SetUp()
        {
            mockFileHandler = new Mock<IFileHandler>(MockBehavior.Strict);

            classUnderTest = new GitConfigEditor(mockFileHandler.Object);
        }

        [Test]
        public void SetGitUserNameAndEmail_WithNoConfigFile_WritesNewConfigFile()
        {
            string expectedData = string.Format("[user]\n\tname = {0}\n\temail = {1}\n", TestUsername, TestEmail);
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Throws(new FileNotFoundException());
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.ConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitNameAndEmail(TestUsername, TestEmail);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.ConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.ConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitUserNameAndEmail_WithEmptyCofig_WritesNewConfigData()
        {
            string expectedData = string.Format("[user]\n\tname = {0}\n\temail = {1}\n", TestUsername, TestEmail);
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(new List<string>());
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.ConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitNameAndEmail(TestUsername, TestEmail);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.ConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.ConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitUserNameAndEmail_WithMissingUserSectionInConfig_AddsTheUserSection()
        {
            string expectedData = string.Join("\n", GetExampleGitConfig()) +
                                  string.Format("[user]\n\tname = {0}\n\temail = {1}\n", TestUsername, TestEmail);
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(GetExampleGitConfig());
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.ConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitNameAndEmail(TestUsername, TestEmail);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.ConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.ConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitUserNameAndEmail_WithOldData_UpdatesTheUsernameAndEmail()
        {
            string expectedData = string.Join("\n", GetExampleGitConfig(TestUsername, TestEmail));
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(GetExampleGitConfig("oldUser", "oldEmail"));
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.ConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitNameAndEmail(TestUsername, TestEmail);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.ConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.ConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitUserNameAndEmail_WithMissingNewlineEof_AddsANewLine()
        {
            string expectedData = string.Join("\n", GetExampleGitConfig(TestUsername, TestEmail));
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(GetExampleGitConfig("oldUser", "oldEmail", withEndingNewLine: false));
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.ConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitNameAndEmail(TestUsername, TestEmail);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.ConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.ConfigFilePath, expectedData));
        }

        IEnumerable<string> GetExampleGitConfig(string username = null, string email = null, bool withEndingNewLine = true)
        {
            var configLines = new List<string>();

            configLines.Add("[core]");
            configLines.Add("\tautocrlf = false");

            if (username != null || email != null)
            {
                configLines.Add("[user]");
                if (username != null)
                    configLines.Add("\tname = " + username);

                if (email != null)
                    configLines.Add("\temail = " + email);
            }
            configLines.Add("[push]");
            configLines.Add("\tdefault = simple");

            if (withEndingNewLine)
                configLines.Add("");

            return configLines;
        }
    }
}
