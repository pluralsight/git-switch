using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private GitUser currentUser = new NullGitUser();

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
            currentUser = user ?? new NullGitUser();
            gitConfigEditor.SetGitUsernameAndEmail(currentUser.Username, currentUser.Email);
            sshConfigEditor.SetGitHubKeyFile(currentUser.SshKeyPath);

            if (!(currentUser is NullGitUser) && !fileHasher.IsHashCorrectForFile(currentUser.SshKeyHash, currentUser.SshKeyPath))
            {
                throw new SshKeyHashException();
            }
        }

        public GitUser GetCurrentUser()
        {
            return currentUser;
        }
    }
}
