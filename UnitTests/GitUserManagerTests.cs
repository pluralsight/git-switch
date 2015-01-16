using System.Collections.Generic;
using System.IO;
using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class GitUserManagerTests
    {
        private GitUserManager manager;
        private Mock<IFileHandler> mockSerializer;
        private Mock<IFileHasher> mockFileHasher;
        private Mock<IGitConfigEditor> mockGitConfigEditor;
        private Mock<ISshConfigEditor> mockSshConfigEditor;
        private GitUser testUser;

        [SetUp]
        public void SetUp()
        {
            mockSerializer = new Mock<IFileHandler>();
            mockFileHasher = new Mock<IFileHasher>();
            mockGitConfigEditor = new Mock<IGitConfigEditor>();
            mockSshConfigEditor = new Mock<ISshConfigEditor>();
            testUser = new GitUser()
            {
                Username = "Test User",
                Email = "test@example.com",
                SshKeyPath = @"c:\fake-key"
            };

            mockSerializer.Setup(mock => mock.DeserializeFromFile<List<GitUser>>(GitUserManager.GitUserFile))
                .Throws(new FileNotFoundException());

            manager = new GitUserManager(mockSerializer.Object, mockFileHasher.Object, mockGitConfigEditor.Object, mockSshConfigEditor.Object);
        }

        [Test]
        public void InitialState()
        {
            Assert.AreEqual(0, manager.GetUsers().Count);
            Assert.Null(manager.GetUserByUsername("Any User"));
            Assert.IsInstanceOf<NullGitUser>(manager.GetCurrentUser());
        }

        [Test]
        public void AUserCanBeAdded()
        {
            const string testHash = "fake-hash";
            mockFileHasher.Setup(mock => mock.HashFile(It.IsAny<string>())).Returns(testHash);

            manager.AddUser(testUser);

            Assert.AreEqual(1, manager.GetUsers().Count);
            Assert.AreSame(testUser, manager.GetUsers()[0]);
            Assert.AreEqual(testUser.SshKeyHash, testHash);
            mockFileHasher.Verify(mock => mock.HashFile(testUser.SshKeyPath));
            mockSerializer.Verify(mock => mock.SerializeToFile(GitUserManager.GitUserFile, It.IsAny<List<GitUser>>()));
        }

        [Test]
        public void AddingAnExisingUserUpdatesTheHash()
        {
            manager.AddUser(testUser);
            manager.AddUser(testUser);
            
            Assert.AreEqual(1, manager.GetUsers().Count);
            Assert.AreSame(testUser, manager.GetUsers()[0]);
            mockFileHasher.Verify(mock => mock.HashFile(testUser.SshKeyPath), Times.Exactly(2));
            mockSerializer.Verify(mock => mock.SerializeToFile(GitUserManager.GitUserFile, It.IsAny<List<GitUser>>()));
        }

        [Test]
        public void GetUserByUserName()
        {
            Assert.Null(manager.GetUserByUsername(testUser.Username));
            manager.AddUser(testUser);
            Assert.AreSame(testUser, manager.GetUserByUsername(testUser.Username));
        }

        [Test]
        public void AUserCanBeRemoved()
        {
            manager.AddUser(testUser);
            manager.RemoveUser(testUser);
            
            Assert.AreEqual(0, manager.GetUsers().Count);
            mockSerializer.Verify(mock => mock.SerializeToFile(GitUserManager.GitUserFile, It.IsAny<List<GitUser>>()), Times.Exactly(2));
        }

        [Test]
        public void ConfigureForUserSetsTheConfigFiles()
        {
            manager.AddUser(testUser);
            mockFileHasher.Setup(mock => mock.IsHashCorrectForFile(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            Assert.IsInstanceOf<NullGitUser>(manager.GetCurrentUser());

            manager.ConfigureForUser(testUser.Username);

            Assert.AreEqual(testUser, manager.GetCurrentUser());
            mockGitConfigEditor.Verify(mock => mock.SetGitUsernameAndEmail(testUser.Username, testUser.Email));
            mockSshConfigEditor.Verify(mock => mock.SetGitHubKeyFile(testUser.SshKeyPath));
            mockFileHasher.Verify(mock => mock.IsHashCorrectForFile(testUser.SshKeyHash, testUser.SshKeyPath));
        }

        [Test]
        public void ConfigureForUserUsesTheNullGitUserIfItCannotFindTheUser()
        {
            manager.ConfigureForUser("some-fake-user");

            Assert.IsInstanceOf<NullGitUser>(manager.GetCurrentUser());
        }

        [Test]
        public void ConfigureForUserThrowsAnExceptionIfTheSshKeyHasADifferentHash()
        {
            manager.AddUser(testUser);
            mockFileHasher.Setup(mock => mock.IsHashCorrectForFile(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            
            Assert.Throws<SshKeyHashException>(() => manager.ConfigureForUser(testUser.Username));

            mockFileHasher.Verify(mock => mock.IsHashCorrectForFile(testUser.SshKeyHash, testUser.SshKeyPath));
        }

        [Test]
        public void AddUserThrowsFileNotFoundExceptionIfSshFileDoesNotExist()
        {
            mockFileHasher.Setup(x => x.HashFile(It.IsAny<string>())).Throws<FileNotFoundException>();
            Assert.Throws<FileNotFoundException>(() => manager.AddUser(new GitUser{Email = "email", Username = "name"}));
        }
    }
}
