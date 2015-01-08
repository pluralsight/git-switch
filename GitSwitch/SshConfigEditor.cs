using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitSwitch
{
    public class SshConfigEditor
    {
        private IFileHandler fileHandler;

        public string SshConfigFilePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\.ssh\config";
            }
        }

        public SshConfigEditor(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
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

            if (!didFindGitHub)
            {
                if (output[output.Count - 1] != "")
                {
                    output.Add("");
                }
                output.Add(defaultSshConfig);
            }
            return string.Join("\n", output);
        }

        private string WindowsToUnixPath(string path)
        {
            return Regex.Replace(path.Replace("\\", "/"), "^([A-Za-z]):", "/$1");
        }
    }
}
