namespace GitSwitch
{
    public class GitUser
    {
        public GitUser(string name, string email, string sshKeyPath)
        {
            Name = name;
            Email = email;
            SshKeyPath = sshKeyPath;
        }

        protected GitUser()
        { }

        public string Name { get; set; }
        public string Email { get; set; }
        public string SshKeyPath { get; set; }
        public string SshKeyHash { get; set; }
    }

    public class NullGitUser : GitUser
    { }
}
