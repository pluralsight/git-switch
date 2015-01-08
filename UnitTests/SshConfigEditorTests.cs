using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    class SshConfigEditorTests
    {
        private SshConfigEditor editor;
        private Mock<IFileHandler> mockFileHandler;
        private string testKeyWindowsPath = @"C:\Users\fakeuser\.ssh\githubkey";
        private string testKeyUnixPath = @"/C/Users/fakeuser/.ssh/githubkey";

        [SetUp]
        public void SetUp()
        {
            mockFileHandler = new Mock<IFileHandler>(MockBehavior.Strict);

            editor = new SshConfigEditor(mockFileHandler.Object);
        }

        [Test]
        public void ItWritesANewFileIfOneWasNotFound()
        {
            string expectedData = "Host github.com\n\tIdentityFile " + testKeyUnixPath + "\n";
            
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Throws(new FileNotFoundException());
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(testKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        [Test]
        public void ItAddsTheGitHubEntryIfItWasNotFound()
        {
            List<string> priorConfigLines = GetExampleConfigLines();
            string expectedData = string.Join("\n", priorConfigLines) + "\nHost github.com\n\tIdentityFile " + testKeyUnixPath + "\n";
            
            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(testKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        [Test]
        public void ItChangesTheGitHubIdentityFile()
        {
            List<string> priorConfigLines = GetExampleConfigLines("~/.ssh/oldGitHubKey");
            string expectedData = string.Join("\n", GetExampleConfigLines(testKeyUnixPath));

            mockFileHandler.Setup(mock => mock.ReadLines(It.IsAny<string>())).Returns(priorConfigLines);
            mockFileHandler.Setup(mock => mock.WriteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, data) =>
                {
                    Assert.AreEqual(editor.SshConfigFilePath, path);
                    Assert.AreEqual(expectedData, data);
                });

            editor.SetGitHubKeyFile(testKeyWindowsPath);

            mockFileHandler.Verify(mock => mock.ReadLines(editor.SshConfigFilePath));
            mockFileHandler.Verify(mock => mock.WriteFile(editor.SshConfigFilePath, expectedData));
        }

        private List<string> GetExampleConfigLines(string gitHubKey = null)
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
            lines.Add("");

            return lines;
        }
    }
}
