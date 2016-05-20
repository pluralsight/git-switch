using System.Collections.Generic;
using System.IO;
using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    class SshConfigEditorTests
    {
        const string TestKeyWindowsPath = @"C:\Users\fakeuser\.ssh\githubkey";
        const string TestKeyUnixPath = @"/C/Users/fakeuser/.ssh/githubkey";

        SshConfigEditor classUnderTest;
        Mock<IFileHandler> mockFileHandler;

        [SetUp]
        public void SetUp()
        {
            mockFileHandler = new Mock<IFileHandler>(MockBehavior.Strict);

            classUnderTest = new SshConfigEditor(mockFileHandler.Object);
        }

        [Test]
        public void SetGitHubKeyFile_WithMissingConfig_WritesANewFile()
        {
            string expectedData = "Host github.com\n\tIdentityFile " + TestKeyUnixPath + "\n";

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Throws(new FileNotFoundException());
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.SshConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.SshConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitHubKeyFile_WithMissingSshDirectory_WritesANewFile()
        {
            string expectedData = "Host github.com\n\tIdentityFile " + TestKeyUnixPath + "\n";

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Throws(new DirectoryNotFoundException());
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.SshConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.SshConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitHubKeyFile_WithMissingGitHubEntry_AddsConfig()
        {
            List<string> priorConfigLines = GetExampleConfigLines();
            string expectedData = string.Join("\n", priorConfigLines) + "\nHost github.com\n\tIdentityFile " + TestKeyUnixPath + "\n";

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.SshConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.SshConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitHubKeyFile_WithOldData_UpdatesGitHubIdentityFile()
        {
            List<string> priorConfigLines = GetExampleConfigLines("~/.ssh/oldGitHubKey");
            string expectedData = string.Join("\n", GetExampleConfigLines(TestKeyUnixPath));

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.SshConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.SshConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitHubKeyFile_WithMissingNewlineEof_AddsANewLine()
        {
            List<string> priorConfigLines = GetExampleConfigLines("~/.ssh/oldGitHubKey", endWithNewLine: false);
            string expectedData = string.Join("\n", GetExampleConfigLines(TestKeyUnixPath));

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.SshConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.SshConfigFilePath, expectedData));
        }

        [TestCase("")]
        [TestCase(null)]
        public void SetGitHubKeyFile_AllowsANullOrEmptySshKeyString(string sshKey)
        {
            List<string> priorConfigLines = GetExampleConfigLines("~/.ssh/oldGitHubKey");
            string expectedData = string.Join("\n", GetExampleConfigLines(""));

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.SshConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitHubKeyFile(sshKey);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.SshConfigFilePath, expectedData));
        }

        [Test]
        public void SetGitHubKeyFile_HandlesEmptyConfigFile()
        {
            List<string> priorConfigLines = new List<string>();
            string expectedData = "Host github.com\n\tIdentityFile " + TestKeyUnixPath + "\n";

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.That(classUnderTest.SshConfigFilePath, Is.EqualTo(path));
                    Assert.That(data, Is.EqualTo(expectedData));
                });

            classUnderTest.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(classUnderTest.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(classUnderTest.SshConfigFilePath, expectedData));
        }

        List<string> GetExampleConfigLines(string gitHubKey = null, bool endWithNewLine = true)
        {
            var lines = new List<string>();

            lines.Add("Host bitbucket.com");
            lines.Add("\tIdentityFile ~/.ssh/bitbucket");
            lines.Add("");

            if (gitHubKey != null)
            {
                lines.Add("Host github.com");
                lines.Add("\tIdentityFile " + gitHubKey);
                lines.Add("");
            }

            lines.Add("Host example.com");
            lines.Add("\tStrictHostKeyChecking no");

            if (endWithNewLine)
                lines.Add("");

            return lines;
        }
    }
}
