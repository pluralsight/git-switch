using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GitSwitch
{
    public interface IFileHandler
    {
        T DeserializeFromFile<T>(string filePath);
        void SerializeToFile<T>(string filePath, T TInput);
        IEnumerable<string> ReadLines(string filePath);
        void WriteFile(string filePath, string data);
        bool DoesFileExist(string filePath);
    }

    public class FileHandler : IFileHandler
    {
        public T DeserializeFromFile<T>(string filePath)
        {
            var settings = new XmlReaderSettings
            {
                CloseInput = true,
                ConformanceLevel = ConformanceLevel.Document,
                IgnoreComments = true
            };

            using (var reader = XmlReader.Create(filePath, settings))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        public void SerializeToFile<T>(string filePath, T TInput)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, TInput);
            }
        }

        public IEnumerable<string> ReadLines(string filePath)
        {
            return File.ReadLines(filePath);
        }

        public void WriteFile(string filePath, string data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, data);
        }

        public bool DoesFileExist(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
