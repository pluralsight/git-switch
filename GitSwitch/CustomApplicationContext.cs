using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GitSwitch
{
    class CustomApplicationContext : ApplicationContext
    {
        private readonly GitUserManager gitUserManager;
        private readonly OptionsController optionsController;
        private NotifyIcon notifyIcon;
        private EditUsersForm editUsersForm;
        private HelpForm helpForm;
        private IconRepository iconRepository;
        private Icon defaultIcon;

        public CustomApplicationContext()
        {
            InitializeTrayIcon();
            var fileHandler = new FileHandler();
            gitUserManager = new GitUserManager(fileHandler, new Sha1FileHasher(), new GitConfigEditor(fileHandler), new SshConfigEditor(fileHandler));
            optionsController = new OptionsController(fileHandler);
            iconRepository = new IconRepository(new IconDownloader(), fileHandler);
        }

        private void InitializeTrayIcon()
        {
            defaultIcon = new Icon("gitswitch64x64.ico");
            notifyIcon = new NotifyIcon()
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = defaultIcon,
                Text = "GitSwitch",
                Visible = true
            };
            notifyIcon.ContextMenuStrip.Opening += OnContextMenuStripOpening;
        }

        private void OnContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            notifyIcon.ContextMenuStrip.Items.Clear();
            var currentUser = gitUserManager.GetCurrentUser();
            gitUserManager.GetUsers().ForEach(x => notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(x.Username, OnGitUserClick, (x == currentUser))));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Edit Users...", OnEditUsers));
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Logout", OnLogout, (currentUser is NullGitUser)));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(CreateOptionsSubMenu());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&Exit", OnExit));
        }

        private ToolStripMenuItem CreateOptionsSubMenu()
        {
            var submenu = new ToolStripMenuItem("Help && Options");
            submenu.DropDownItems.Add(ToolStripMenuItemWithHandler("Help", OnHelp));
            submenu.DropDownItems.Add(new ToolStripSeparator());
            submenu.DropDownItems.Add(ToolStripMenuItemWithHandler("Kill ssh-agent on user change", OnKillSshAgent, optionsController.KillSshAgent));
            return submenu;
        }

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler, bool isChecked = false)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null) { item.Click += eventHandler; }

            item.CheckState = isChecked ? CheckState.Checked : CheckState.Unchecked;
           
            return item;
        }

        private void OnGitUserClick(object sender, EventArgs e)
        {
            ConfigureForUser(sender.ToString());

            if (optionsController.KillSshAgent)
            {
                new ProcessKiller().KillSshAgent();
            }
        }

        private void ConfigureForUser(string username)
        {
            try
            {
                gitUserManager.ConfigureForUser(username);
                notifyIcon.Icon = iconRepository.GetIconForUser(gitUserManager.GetCurrentUser());
            }
            catch (FileNotFoundException)
            {
                ShowErrorMessage("SSH key file could not be found at path: " + gitUserManager.GetUserByUsername(username).SshKeyPath);
            }
            catch (DirectoryNotFoundException)
            {
                ShowErrorMessage("SSH key file could not be found at path: " + gitUserManager.GetUserByUsername(username).SshKeyPath);
            }
            catch (SshKeyHashException)
            {
                ShowErrorMessage("SSH key file has changed at path: " + gitUserManager.GetUserByUsername(username).SshKeyPath);
            }
            catch (InvalidUserException)
            {
                ShowErrorMessage("Error loading user: " + username);
            }
        }

        private void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "GitSwitch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OnEditUsers(object sender, EventArgs e)
        {
            if (editUsersForm == null)
            {
                editUsersForm = new EditUsersForm(gitUserManager);
                editUsersForm.Closed += OnCloseEditUsersForm;
                editUsersForm.Show();
            }
            else
            {
                editUsersForm.Activate();
            }
        }

        private void OnCloseEditUsersForm(object sender, EventArgs e)
        {
            editUsersForm = null;
        }

        private void OnLogout(object sender, EventArgs e)
        {
            gitUserManager.ConfigureForUser("");
            notifyIcon.Icon = defaultIcon;
        }

        private void OnHelp(object sender, EventArgs e)
        {
            if (helpForm == null)
            {
                helpForm = new HelpForm();
                helpForm.Closed += OnCloseHelpForm;
                helpForm.Show();
            }
            else
            {
                helpForm.Activate();
            }
        }

        private void OnCloseHelpForm(object sender, EventArgs e)
        {
            helpForm = null;
        }

        private void OnExit(object sender, EventArgs e)
        {
            ExitThread();
        }

        protected override void ExitThreadCore()
        {
            notifyIcon.Visible = false;
            base.ExitThreadCore();
        }

        private void OnKillSshAgent(object sender, EventArgs e)
        {
            optionsController.ToggleKillSshAgent();
        }
    }
}
