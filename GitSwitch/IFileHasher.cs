namespace GitSwitch
{
    public interface IFileHasher
    {
        string HashFile(string filePath);
        bool IsHashCorrectForFile(string hash, string filePath);
    }
}
