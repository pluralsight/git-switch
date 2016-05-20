using System;
using System.Windows.Forms;

namespace GitSwitch
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var fileHandler = new FileHandler();
            var gitUserManager = new GitUserManager(fileHandler, new Sha1FileHasher(), new GitConfigEditor(fileHandler), new SshConfigEditor(fileHandler));
            var optionsManager = new OptionsManager(fileHandler);
            var iconRepository = new IconRepository(new IconDownloader(), fileHandler, new GravatarUrlBuilder());
            CustomApplicationContext context = new CustomApplicationContext(gitUserManager, optionsManager, iconRepository, new ProcessKiller());

            Application.Run(context);
        }
    }
}
