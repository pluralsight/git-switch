using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GitSwitch
{
    public interface IGitConfigEditor
    {
        void SetGitNameAndEmail(string name, string email);
    }

    public class GitConfigEditor : IGitConfigEditor
    {
        readonly IFileHandler fileHandler;

        public GitConfigEditor(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
        }

        internal string ConfigFilePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + AppConstants.GitConfigFile;
            }
        }

        public void SetGitNameAndEmail(string name, string email)
        {
            string newConfig;

            try
            {
                IEnumerable<string> configLines = fileHandler.ReadLines(ConfigFilePath);
                newConfig = ProcessConfigFile(configLines, name, email);
            }
            catch (FileNotFoundException)
            {
                newConfig = string.Format("[user]\n\tname = {0}\n\temail = {1}\n", name, email);
            }

            fileHandler.WriteFile(ConfigFilePath, newConfig);
        }

        string ProcessConfigFile(IEnumerable<string> configLines, string name, string email)
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
                        output.Add("\tname = " + name);
                        output.Add("\temail = " + email);
                    }
                }

                if (inUserSection && Regex.IsMatch(line, @"^\s*(name|email)\s+=\s+"))
                    output.RemoveAt(output.Count - 1);
            }

            if (!didFindUserSection)
            {
                if (output.Count > 0 && output.Last() == "")
                    output.RemoveAt(output.Count - 1);

                output.Add("[user]");
                output.Add("\tname = " + name);
                output.Add("\temail = " + email);
                output.Add("");
            }

            if (output.Last() != "")
                output.Add("");

            return string.Join("\n", output);
        }
    }
}
