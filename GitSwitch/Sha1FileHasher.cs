using System;
using System.IO;
using System.Security.Cryptography;

namespace GitSwitch
{
    public class Sha1FileHasher : IFileHasher
    {
        public string HashFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    SHA1Managed sha1 = new SHA1Managed();
                    byte[] bytes = sha1.ComputeHash(stream);
                    return BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();
                }
            }

            throw new FileNotFoundException(string.Format("File '{0}' does not exist", filePath));
        }

        public bool IsHashCorrectForFile(string hash, string filePath)
        {
            return hash.ToLower() == HashFile(filePath);
        }
    }
}
