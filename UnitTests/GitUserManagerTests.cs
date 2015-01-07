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
    public class GitUserManagerTests
    {
        private GitUserManager manager;
        private Mock<IFileHandler> mockSerializer;
        private Mock<IFileHasher> mockFileHasher;
        private GitUser testUser;

        [SetUp]
        public void SetUp()
        {
            mockSerializer = new Mock<IFileHandler>();
            mockFileHasher = new Mock<IFileHasher>();
            testUser = new GitUser()
            {
                Username = "Test User",
                Email = "test@example.com",
                SshKeyPath = "c:\fake-key"
            };

            mockSerializer.Setup(mock => mock.DeserializeFromFile<List<GitUser>>(GitUserManager.GitUserFile))
                .Throws(new FileNotFoundException());

            manager = new GitUserManager(mockSerializer.Object, mockFileHasher.Object);
        }

        [Test]
        public void InitialState()
        {
            Assert.AreEqual(0, manager.GetUsers().Count);
            Assert.Null(manager.GetUserByUsername("Any User"));
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
    }
}
