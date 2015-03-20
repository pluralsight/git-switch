using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GitSwitch
{
    public class GitConfigEditor : IGitConfigEditor
    {
        private readonly IFileHandler fileHandler;

        public GitConfigEditor(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
        }

        public string ConfigFilePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\.gitconfig";
            }
        }

        public void SetGitUsernameAndEmail(string username, string email)
        {
            string newConfig;

            try
            {
                IEnumerable<string> configLines = fileHandler.ReadLines(ConfigFilePath);
                newConfig = ProcessConfigFile(configLines, username, email);
            }
            catch (FileNotFoundException)
            {
                newConfig = string.Format("[user]\n\tname = {0}\n\temail = {1}\n", username, email);
            }

            fileHandler.WriteFile(ConfigFilePath, newConfig);
        }

        private string ProcessConfigFile(IEnumerable<string> configLines, string username, string email)
        {
            var output = new List<string>();
            bool didFindUserSection = false;
            bool inUserSection = false;

            foreach (var line in configLines)
            {
                output.Add(line);
                if (Regex.IsMatch(line, @"^\s*\["))
                {
                    inUserSection = Regex.IsMatch(line, @"^\s*\[user\]");
                    if (inUserSection)
                    {
                        didFindUserSection = true;
                        output.Add("\tname = " + username);
                        output.Add("\temail = " + email);
                    }
                }
                if (inUserSection && Regex.IsMatch(line, @"^\s*(name|email)\s+=\s+"))
                {
                    output.RemoveAt(output.Count - 1);
                }
            }

            if (!didFindUserSection)
            {
                if (output.Count > 0 && output.Last() == "")
                {
                    output.RemoveAt(output.Count - 1);
                }
                output.Add("[user]");
                output.Add("\tname = " + username);
                output.Add("\temail = " + email);
                output.Add("");
            }

            if (output.Last() != "")
            {
                output.Add("");
            }

            return string.Join("\n", output);
        }
    }
}
