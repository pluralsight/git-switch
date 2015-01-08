namespace GitSwitch
{
    public interface IGitConfigEditor
    {
        void SetGitUsernameAndEmail(string username, string email);
    }
}