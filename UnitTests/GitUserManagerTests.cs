using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitSwitch;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class GitUserManagerTests
    {
        GitUserManager classUnderTest;
        Mock<IFileHandler> mockFileHandler;
        Mock<IFileHasher> mockFileHasher;
        Mock<IGitConfigEditor> mockGitConfigEditor;
        Mock<ISshConfigEditor> mockSshConfigEditor;

        GitUser testUser;

        [SetUp]
        public void SetUp()
        {
            mockFileHandler = new Mock<IFileHandler>();
            mockFileHasher = new Mock<IFileHasher>();
            mockGitConfigEditor = new Mock<IGitConfigEditor>();
            mockSshConfigEditor = new Mock<ISshConfigEditor>();
            testUser = new GitUser("Test User", "test@example.com", @"c:\fake-key");

            mockFileHandler.Setup(mock => mock.DeserializeFromFile<List<GitUser>>(AppConstants.UsersFile)).Throws(new FileNotFoundException());

            classUnderTest = new GitUserManager(mockFileHandler.Object, mockFileHasher.Object, mockGitConfigEditor.Object, mockSshConfigEditor.Object);
        }

        [Test]
        public void InitialState()
        {
            Assert.That(classUnderTest.Users.Count, Is.EqualTo(0));
            Assert.That(classUnderTest.GetUserByName(testUser.Name), Is.InstanceOf<NullGitUser>());
            Assert.That(classUnderTest.ActiveUsers, Is.Empty);
        }

        [Test]
        public void Users_AreLazyLoadedFromFile()
        {
            Assert.That(classUnderTest.Users.Count, Is.EqualTo(0));

            mockFileHandler.Setup(mock => mock.DeserializeFromFile<List<GitUser>>(AppConstants.UsersFile)).Returns(new List<GitUser> { testUser });

            Assert.That(classUnderTest.Users.Count, Is.EqualTo(1));
            Assert.That(classUnderTest.GetUserByName(testUser.Name), Is.SameAs(testUser));
            Assert.That(classUnderTest.ActiveUsers, Is.Empty);
        }

        [Test]
        public void AddUser()
        {
            const string testHash = "fake-hash";
            mockFileHasher.Setup(mock => mock.HashFile(It.IsAny<string>())).Returns(testHash);

            classUnderTest.AddUser(testUser);

            var availableUsers = classUnderTest.Users;

            Assert.That(availableUsers.Count, Is.EqualTo(1));
            Assert.That(availableUsers.First(), Is.SameAs(testUser));
            Assert.That(availableUsers.First().SshKeyHash, Is.EqualTo(testHash));
            mockFileHasher.Verify(mock => mock.HashFile(testUser.SshKeyPath));
            mockFileHandler.Verify(mock => mock.SerializeToFile(AppConstants.UsersFile, It.IsAny<List<GitUser>>()));
        }

        [Test]
        public void AddUser_WithExistingUser_UpdatesHash()
        {
            classUnderTest.AddUser(testUser);
            classUnderTest.AddUser(testUser);

            var availableUsers = classUnderTest.Users;

            Assert.That(availableUsers.Count, Is.EqualTo(1));
            Assert.That(availableUsers.First(), Is.SameAs(testUser));
            mockFileHasher.Verify(mock => mock.HashFile(testUser.SshKeyPath), Times.Exactly(2));
            mockFileHandler.Verify(mock => mock.SerializeToFile(AppConstants.UsersFile, It.IsAny<List<GitUser>>()));
        }

        [Test]
        public void AddUser_WithMissingSshFile_ThrowsFileNotFoundException()
        {
            mockFileHasher.Setup(x => x.HashFile(It.IsAny<string>())).Throws<FileNotFoundException>();

            Assert.Throws<FileNotFoundException>(() => classUnderTest.AddUser(new GitUser("name", "email", null)));
        }

        [Test]
        public void GetUserByName()
        {
            Assert.That(classUnderTest.GetUserByName(testUser.Name), Is.InstanceOf<NullGitUser>());

            classUnderTest.AddUser(testUser);

            Assert.That(classUnderTest.GetUserByName(testUser.Name), Is.SameAs(testUser));
        }

        [Test]
        public void GetUserByName_WithUnknownUser_ReturnsNullGitUser()
        {
            Assert.That(classUnderTest.GetUserByName(""), Is.InstanceOf<NullGitUser>());
            Assert.That(classUnderTest.GetUserByName(null), Is.InstanceOf<NullGitUser>());
            Assert.That(classUnderTest.GetUserByName("Some User"), Is.InstanceOf<NullGitUser>());
        }

        [Test]
        public void RemoveUser()
        {
            classUnderTest.AddUser(testUser);
            classUnderTest.RemoveUser(testUser);

            Assert.That(classUnderTest.Users.Count, Is.EqualTo(0));
            mockFileHandler.Verify(mock => mock.SerializeToFile(AppConstants.UsersFile, It.IsAny<List<GitUser>>()), Times.Exactly(2));
        }

        [Test]
        public void MakeUserActive_AddsUserToActiveList()
        {
            classUnderTest.AddUser(testUser);

            Assert.That(classUnderTest.ActiveUsers, Is.Empty);

            classUnderTest.MakeUserActive(testUser, isPairOrMob: false);

            Assert.That(classUnderTest.ActiveUsers.Count, Is.EqualTo(1));
            Assert.That(classUnderTest.ActiveUsers.First(), Is.EqualTo(testUser));
        }

        [Test]
        public void MakeUserActive_IsNotPairOrMob_DoesNotAllowMultipleUsers()
        {
            classUnderTest.AddUser(testUser);
            var otherUser = new GitUser("Some User", "user@email.com", "path/to/key");
            classUnderTest.AddUser(otherUser);

            Assert.That(classUnderTest.ActiveUsers, Is.Empty);

            classUnderTest.MakeUserActive(testUser, isPairOrMob: false);

            Assert.That(classUnderTest.ActiveUsers.Count, Is.EqualTo(1));
            Assert.That(classUnderTest.ActiveUsers.First(), Is.EqualTo(testUser));

            classUnderTest.MakeUserActive(otherUser, isPairOrMob: false);

            Assert.That(classUnderTest.ActiveUsers.Count, Is.EqualTo(1));
            Assert.That(classUnderTest.ActiveUsers.First(), Is.EqualTo(otherUser));
        }

        [Test]
        public void MarkUserActive_WithUnknownUser_DoesNothing()
        {
            classUnderTest.AddUser(testUser);

            var unkownUser = new GitUser("Some User", "user@email.com", "path-to-key");
            classUnderTest.MakeUserActive(unkownUser, isPairOrMob: false);

            Assert.That(classUnderTest.ActiveUsers.Count, Is.EqualTo(0));
        }

        [Test]
        public void MakeUserActive_IsPairOrMob_AllowsMultipleUsers()
        {
            classUnderTest.AddUser(testUser);
            var otherUser = new GitUser("Some User", "user@email.com", "path/to/key");
            classUnderTest.AddUser(otherUser);

            Assert.That(classUnderTest.ActiveUsers, Is.Empty);

            classUnderTest.MakeUserActive(testUser, isPairOrMob: true);

            Assert.That(classUnderTest.ActiveUsers.Count, Is.EqualTo(1));
            Assert.That(classUnderTest.ActiveUsers.First(), Is.EqualTo(testUser));

            classUnderTest.MakeUserActive(otherUser, isPairOrMob: true);

            Assert.That(classUnderTest.ActiveUsers.Count, Is.EqualTo(2));
            Assert.That(classUnderTest.ActiveUsers.Last(), Is.EqualTo(otherUser));
        }

        [Test]
        public void ConfigureForUser_SetsTheConfigFiles()
        {
            classUnderTest.AddUser(testUser);
            classUnderTest.MakeUserActive(testUser, false);
            mockFileHasher.Setup(mock => mock.IsHashCorrectForFile(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            classUnderTest.ConfigureForActiveUsers();

            mockGitConfigEditor.Verify(mock => mock.SetGitNameAndEmail(testUser.Name, testUser.Email));
            mockSshConfigEditor.Verify(mock => mock.SetGitHubKeyFile(testUser.SshKeyPath));
            mockFileHasher.Verify(mock => mock.IsHashCorrectForFile(testUser.SshKeyHash, testUser.SshKeyPath));
        }

        [Test]
        public void ConfigureForActiveUsers_WithNoActiveUsers_EmptiesConfig()
        {
            classUnderTest.AddUser(testUser);

            var nullUser = new NullGitUser();
            classUnderTest.ConfigureForActiveUsers();

            mockGitConfigEditor.Verify(mock => mock.SetGitNameAndEmail(nullUser.Name, nullUser.Email));
            mockSshConfigEditor.Verify(mock => mock.SetGitHubKeyFile(nullUser.SshKeyPath));
            mockFileHasher.Verify(mock => mock.IsHashCorrectForFile(nullUser.SshKeyHash, nullUser.SshKeyPath), Times.Never());
        }

        [Test]
        public void ConfigureForActiveUsers_WithSshKeyWithDifferentHash_ThrowsAnException()
        {
            classUnderTest.AddUser(testUser);
            classUnderTest.MakeUserActive(testUser, false);
            mockFileHasher.Setup(mock => mock.IsHashCorrectForFile(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            Assert.Throws<SshKeyHashException>(() => classUnderTest.ConfigureForActiveUsers());

            mockFileHasher.Verify(mock => mock.IsHashCorrectForFile(testUser.SshKeyHash, testUser.SshKeyPath));
        }

        [Test]
        public void ConfigureForSingleUser_MakesActiveAndConfigures()
        {
            classUnderTest.AddUser(testUser);
            mockFileHasher.Setup(mock => mock.IsHashCorrectForFile(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            classUnderTest.ConfigureForSingleUser(testUser);

            Assert.That(classUnderTest.ActiveUsers.Count, Is.EqualTo(1));
            Assert.That(classUnderTest.ActiveUsers.First(), Is.EqualTo(testUser));
            mockGitConfigEditor.Verify(mock => mock.SetGitNameAndEmail(testUser.Name, testUser.Email));
            mockSshConfigEditor.Verify(mock => mock.SetGitHubKeyFile(testUser.SshKeyPath));
            mockFileHasher.Verify(mock => mock.IsHashCorrectForFile(testUser.SshKeyHash, testUser.SshKeyPath));
        }
    }
}
