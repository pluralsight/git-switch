using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitSwitch
{
    public class GitUserManager
    {
        public const string GitUserFile = "gitusers.xml";

        private IFileHandler serializer;
        private IFileHasher fileHasher;
        private IGitConfigEditor gitConfigEditor;
        private ISshConfigEditor sshConfigEditor;
        private List<GitUser> users = new List<GitUser>();

        public GitUserManager(IFileHandler serializer, IFileHasher fileHasher, IGitConfigEditor gitConfigEditor, ISshConfigEditor sshConfigEditor)
        {
            this.serializer = serializer;
            this.fileHasher = fileHasher;
            this.gitConfigEditor = gitConfigEditor;
            this.sshConfigEditor = sshConfigEditor;
            LoadFromFile();
        }

        public List<GitUser> GetUsers()
        {
            return users;
        }

        public void AddUser(GitUser gitUser)
        {
            ValidateGitUser(gitUser);
            if (!users.Contains(gitUser))
            {
                users.Add(gitUser);
            }
            SaveToFile();
        }

        private void ValidateGitUser(GitUser gitUser)
        {
            gitUser.SshKeyHash = fileHasher.HashFile(gitUser.SshKeyPath);
        }

        public GitUser GetUserByUsername(string username)
        {
            return users.FirstOrDefault(x => x.Username == username);
        }

        public void RemoveUser(GitUser gitUser)
        {
            users.Remove(gitUser);
            SaveToFile();
        }

        private void SaveToFile()
        {
            serializer.SerializeToFile(GitUserFile, users);
        }

        private void LoadFromFile()
        {
            try
            {
                users = serializer.DeserializeFromFile<List<GitUser>>(GitUserFile);
            }
            catch (FileNotFoundException)
            {
                // Could not load from file; so we just have an empty users list.
            }
        }

        public void ConfigureForUser(string username)
        {
            ConfigureForUser(GetUserByUsername(username));
        }

        private void ConfigureForUser(GitUser user)
        {
            if (user == null)
            {
                throw new InvalidUserException();
            }
            gitConfigEditor.SetGitUsernameAndEmail(user.Username, user.Email);
            sshConfigEditor.SetGitHubKeyFile(user.SshKeyPath);
            if (!fileHasher.IsHashCorrectForFile(user.SshKeyHash, user.SshKeyPath))
            {
                throw new SshKeyHashException();
            }
        }
    }
}
