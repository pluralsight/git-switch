namespace GitSwitch
{
    public interface ISerializer
    {
        T Read<T>(string filePath);
        void Write<T>(string filePath, T TInput);
    }
}