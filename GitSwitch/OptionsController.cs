using System.IO;

namespace GitSwitch
{
    public class OptionsController : IOptionsController
    {
        public const string OptionsFile = "gitswitch-options.xml";
        
        private readonly IFileHandler fileHandler;
        private GitSwitchOptions options;

        public OptionsController(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            try
            {
                options = fileHandler.DeserializeFromFile<GitSwitchOptions>(OptionsFile);
            }
            catch (FileNotFoundException)
            {
                options = new GitSwitchOptions
                {
                    KillSshAgent = false
                };
            }
        }

        private void SaveToFile()
        {
            fileHandler.SerializeToFile(OptionsFile, options);
        }

        public bool KillSshAgent { get { return options.KillSshAgent; } }

        public void ToggleKillSshAgent()
        {
            options.KillSshAgent = !options.KillSshAgent;
            SaveToFile();
        }
    }
}
