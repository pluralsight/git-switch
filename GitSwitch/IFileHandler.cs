using System.Collections.Generic;

namespace GitSwitch
{
    public interface IFileHandler
    {
        T DeserializeFromFile<T>(string filePath);
        void SerializeToFile<T>(string filePath, T TInput);
        IEnumerable<string> ReadLines(string filePath);
        void WriteLinesUnixStyle(string filePath, IEnumerable<string> lines);
    }
}
