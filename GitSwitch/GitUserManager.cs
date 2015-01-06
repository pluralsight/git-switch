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
        private const string GitUserFile = "gitusers.xml";

        private ISerializer serializer = new Serializer();
        private List<GitUser> users = new List<GitUser>();

        public GitUserManager(ISerializer serializer)
        {
            this.serializer = serializer;
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
            gitUser.SshKeyHash = FileHasher.HashFile(gitUser.SshKeyPath);
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
            serializer.Write(GitUserFile, users);
        }

        private void LoadFromFile()
        {
            try
            {
                users = serializer.Read<List<GitUser>>(GitUserFile);
            }
            catch (FileNotFoundException)
            {
                // Could not load from file; so we just have an empty users list.
            }
        }
    }
}
