using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitSwitch
{
    public interface IGitUserManager
    {
        List<GitUser> Users { get; }
        List<GitUser> ActiveUsers { get; }

        void AddUser(GitUser user);
        GitUser GetUserByName(string name);
        void RemoveUser(GitUser user);
        void MakeUserActive(GitUser user, bool isPairOrMob = false);
        void ConfigureForActiveUsers();
        void ConfigureForSingleUser(GitUser user);
    }

    public class GitUserManager : IGitUserManager
    {
        readonly IFileHandler fileHandler;
        readonly IFileHasher fileHasher;
        readonly IGitConfigEditor gitConfigEditor;
        readonly ISshConfigEditor sshConfigEditor;

        readonly List<GitUser> availableUsers;
        readonly List<GitUser> activeUsers;
        bool hasLoadedUsers;

        public GitUserManager(IFileHandler fileHandler, IFileHasher fileHasher, IGitConfigEditor gitConfigEditor, ISshConfigEditor sshConfigEditor)
        {
            this.fileHandler = fileHandler;
            this.fileHasher = fileHasher;
            this.gitConfigEditor = gitConfigEditor;
            this.sshConfigEditor = sshConfigEditor;

            availableUsers = new List<GitUser>();
            activeUsers = new List<GitUser>();
        }

        public List<GitUser> Users
        {
            get
            {
                if (!availableUsers.Any() && !hasLoadedUsers)
                    LoadAvailableUsers();

                return availableUsers;
            }
        }

        public List<GitUser> ActiveUsers
        {
            get { return activeUsers; }
        }

        void LoadAvailableUsers()
        {
            try
            {
                fileHandler.DeserializeFromFile<List<GitUser>>(AppConstants.UsersFile).ForEach(u => availableUsers.Add(u));
                hasLoadedUsers = true;
            }
            catch (FileNotFoundException)
            {
                // Could not load file -- proceed with an empty users list
            }
        }

        public void AddUser(GitUser user)
        {
            ValidateGitUser(user);

            if (!Users.Contains(user))
                Users.Add(user);

            SaveToFile();
        }

        void ValidateGitUser(GitUser user)
        {
            user.SshKeyHash = fileHasher.HashFile(user.SshKeyPath);
        }

        void SaveToFile()
        {
            fileHandler.SerializeToFile(AppConstants.UsersFile, Users);
        }

        public GitUser GetUserByName(string name)
        {
            return Users.FirstOrDefault(x => x.Name == name) ?? new NullGitUser();
        }

        public void RemoveUser(GitUser user)
        {
            Users.Remove(user);
            SaveToFile();
        }

        public void MakeUserActive(GitUser user, bool isPairOrMob)
        {
            if (!isPairOrMob || user is NullGitUser)
                activeUsers.Clear();

            if (!(user is NullGitUser) && IsKnownUser(user) && !IsActiveUser(user))
                activeUsers.Add(user);
        }

        bool IsKnownUser(GitUser user)
        {
            return Users.Contains(user);
        }

        bool IsActiveUser(GitUser user)
        {
            return activeUsers.Contains(user);
        }

        public void ConfigureForActiveUsers()
        {
            var primaryUser = activeUsers.FirstOrDefault() ?? new NullGitUser();

            gitConfigEditor.SetGitNameAndEmail(GetNameForConfig(primaryUser), GetEmailForConfig(primaryUser));
            sshConfigEditor.SetGitHubKeyFile(primaryUser.SshKeyPath);

            if (!(primaryUser is NullGitUser) && !fileHasher.IsHashCorrectForFile(primaryUser.SshKeyHash, primaryUser.SshKeyPath))
                throw new SshKeyHashException();
        }

        string GetNameForConfig(GitUser primaryUser)
        {
            return activeUsers.Any() ? string.Join(", ", activeUsers.Select(u => u.Name)) : primaryUser.Name;
        }

        string GetEmailForConfig(GitUser primaryUser)
        {
            return activeUsers.Any() ? string.Join(", ", activeUsers.Select(u => u.Email)) : primaryUser.Email;
        }

        public void ConfigureForSingleUser(GitUser user)
        {
            MakeUserActive(user, isPairOrMob: false);
            ConfigureForActiveUsers();
        }
    }
}
