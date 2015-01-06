using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GitSwitch
{
    class FileHasher
    {
        public static string HashFile(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                SHA1Managed sha1 = new SHA1Managed();
                byte[] bytes = sha1.ComputeHash(stream);
                return BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();
            }
        }

        public static bool IsHashCorrectForFile(string hash, string filePath)
        {
            return hash.ToLower() == HashFile(filePath);
        }
    }
}
