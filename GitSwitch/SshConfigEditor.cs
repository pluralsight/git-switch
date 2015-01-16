using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GitSwitch
{
    public class SshConfigEditor : ISshConfigEditor
    {
        private readonly IFileHandler fileHandler;

        public SshConfigEditor(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
        }

        public string SshConfigFilePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\.ssh\config";
            }
        }

        public void SetGitHubKeyFile(string sshKeyPath)
        {
            string unixSshKeyPath = WindowsToUnixPath(sshKeyPath);
            string defaultSshConfig = "Host github.com\n\tIdentityFile " + unixSshKeyPath + "\n";
            
            try
            {
                IEnumerable<string> configFileLines = fileHandler.ReadLines(SshConfigFilePath);
                var newSsshConfig = ProcessSshFile(configFileLines, unixSshKeyPath, defaultSshConfig);
                fileHandler.WriteFile(SshConfigFilePath, newSsshConfig);
            }
            catch (FileNotFoundException)
            {
                fileHandler.WriteFile(SshConfigFilePath, defaultSshConfig);
            }
        }

        private static string ProcessSshFile(IEnumerable<string> configFileLines, string unixSshKeyPath, string defaultSshConfig)
        {
            List<string> output = new List<string>();
            bool didFindGitHub = false;
            bool inGitHubSection = false;

            foreach (var line in configFileLines)
            {
                output.Add(line);
                if (Regex.IsMatch(line, @"^\s*Host\s+"))
                {
                    inGitHubSection = line.Contains("github.com");
                    if (inGitHubSection)
                    {
                        didFindGitHub = true;
                        output.Add("\tIdentityFile " + unixSshKeyPath);
                    }
                }
                if (inGitHubSection && Regex.IsMatch(line, @"^\s*IdentityFile\s+"))
                {
                    output.RemoveAt(output.Count - 1);
                }
            }

            if (output.Last() != "")
            {
                output.Add("");
            }

            if (!didFindGitHub)
            {
                output.Add(defaultSshConfig);
            }

            return string.Join("\n", output);
        }

        private string WindowsToUnixPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            return Regex.Replace(path.Replace("\\", "/"), "^([A-Za-z]):", "/$1");
        }
    }
}
