using System.IO;

namespace GitSwitch
{
    public interface IOptionsManager
    {
        bool KillSshAgent { get; }
        string UsersFile { get; }
        void ToggleKillSshAgent();
    }

    public class OptionsManager : IOptionsManager
    {
        readonly IFileHandler fileHandler;

        GitSwitchOptions currentOptions;

        public OptionsManager(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
        }

        public bool KillSshAgent
        {
            get { return CurrentOptions.KillSshAgent; }
        }

        public string UsersFile
        {
            get { return CurrentOptions.UsersFile; }
        }

        GitSwitchOptions CurrentOptions
        {
            get
            {
                if(currentOptions == null)
                    LoadFromFile();

                return currentOptions;
            }
        }

        void LoadFromFile()
        {
            try
            {
                currentOptions = fileHandler.DeserializeFromFile<GitSwitchOptions>(AppConstants.OptionsFile);
            }
            catch (FileNotFoundException)
            {
                currentOptions = new GitSwitchOptions
                {
                    KillSshAgent = false,
                    UsersFile = AppConstants.UsersFile
                };
            }
        }

        public void ToggleKillSshAgent()
        {
            CurrentOptions.KillSshAgent = !CurrentOptions.KillSshAgent;
            SaveToFile();
        }

        void SaveToFile()
        {
            fileHandler.SerializeToFile(AppConstants.OptionsFile, CurrentOptions);
        }
    }
}
