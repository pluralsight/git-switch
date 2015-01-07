namespace GitSwitch
{
    public interface IFileHandler
    {
        T DeserializeFromFile<T>(string filePath);
        void SerializeToFile<T>(string filePath, T TInput);
    }
}