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
        private const string TestKeyWindowsPath = @"C:\Users\fakeuser\.ssh\githubkey";
        private const string TestKeyUnixPath = @"/C/Users/fakeuser/.ssh/githubkey";
        
        private SshConfigEditor editor;
        private Mock<IFileHandler> mockFileHandler;

        [SetUp]
        public void SetUp()
        {
            mockFileHandler = new Mock<IFileHandler>(MockBehavior.Strict);

            editor = new SshConfigEditor(mockFileHandler.Object);
        }

        [Test]
        public void ItWritesANewFileIfOneWasNotFound()
        {
            string expectedData = "Host github.com\n\tIdentityFile " + TestKeyUnixPath + "\n";
            
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Throws(new FileNotFoundException());
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        [Test]
        public void ItWritesANewFileIfTheConfigDirectoryWasNotFound()
        {
            string expectedData = "Host github.com\n\tIdentityFile " + TestKeyUnixPath + "\n";

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Throws(new DirectoryNotFoundException());
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        [Test]
        public void ItAddsTheGitHubEntryIfItWasNotFound()
        {
            List<string> priorConfigLines = GetExampleConfigLines();
            string expectedData = string.Join("\n", priorConfigLines) + "\nHost github.com\n\tIdentityFile " + TestKeyUnixPath + "\n";
            
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        [Test]
        public void ItChangesTheGitHubIdentityFile()
        {
            List<string> priorConfigLines = GetExampleConfigLines("~/.ssh/oldGitHubKey");
            string expectedData = string.Join("\n", GetExampleConfigLines(TestKeyUnixPath));

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        [Test]
        public void ItAddsANewLineToTheEndOfTheFileIfNecessary()
        {
            List<string> priorConfigLines = GetExampleConfigLines("~/.ssh/oldGitHubKey", endWithNewLine: false);
            string expectedData = string.Join("\n", GetExampleConfigLines(TestKeyUnixPath));

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(TestKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ItAllowsANullOrEmptySshKeyString(string sshKey)
        {
            List<string> priorConfigLines = GetExampleConfigLines("~/.ssh/oldGitHubKey");
            string expectedData = string.Join("\n", GetExampleConfigLines(""));

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(sshKey);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        private List<string> GetExampleConfigLines(string gitHubKey = null, bool endWithNewLine = true)
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
            {
                lines.Add("");
            }

            return lines;
        }
    }
}
