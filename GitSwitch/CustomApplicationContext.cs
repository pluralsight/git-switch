using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GitSwitch
{
    public class CustomApplicationContext : ApplicationContext
    {
        readonly IGitUserManager gitUserManager;
        readonly IOptionsManager optionsManager;
        readonly IIconRepository iconRepository;
        readonly IProcessKiller processKiller;

        NotifyIcon notifyIcon;
        EditUsersForm editUsersForm;
        SelectManyUsersForm selectManyUsersForm;
        HelpForm helpForm;
        Icon defaultIcon;

        public CustomApplicationContext(IGitUserManager gitUserManager, IOptionsManager optionsManager, IIconRepository iconRepository, IProcessKiller processKiller)
        {
            this.gitUserManager = gitUserManager;
            this.optionsManager = optionsManager;
            this.iconRepository = iconRepository;
            this.processKiller = processKiller;

            InitializeTrayIcon();
        }

        void InitializeTrayIcon()
        {
            var pathToIcon = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.ApplicationIcon);
            defaultIcon = new Icon(pathToIcon);

            notifyIcon = new NotifyIcon
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = defaultIcon,
                Text = "GitSwitch",
                Visible = true
            };

            notifyIcon.ContextMenuStrip.Opening += OnContextMenuStripOpening;
        }

        void OnContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            notifyIcon.ContextMenuStrip.Items.Clear();

            var activeUsers = gitUserManager.ActiveUsers;
            gitUserManager.Users.ForEach(x => notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(x.Name, OnGitUserClick, activeUsers.Contains(x))));

            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Select a pair or mob...", OnSelectMany));
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Logout", OnLogout, !activeUsers.Any()));

            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Edit Users...", OnEditUsers));
            notifyIcon.ContextMenuStrip.Items.Add(CreateOptionsSubMenu());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&Exit", OnExit));
        }

        ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler, bool isChecked = false)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null)
                item.Click += eventHandler;

            item.CheckState = isChecked ? CheckState.Checked : CheckState.Unchecked;

            return item;
        }

        ToolStripMenuItem CreateOptionsSubMenu()
        {
            var submenu = new ToolStripMenuItem("Help && Options");
            submenu.DropDownItems.Add(ToolStripMenuItemWithHandler("Help", OnHelp));
            submenu.DropDownItems.Add(new ToolStripSeparator());
            submenu.DropDownItems.Add(ToolStripMenuItemWithHandler("Kill ssh-agent on user change", OnKillSshAgent, optionsManager.KillSshAgent));

            return submenu;
        }

        void OnGitUserClick(object sender, EventArgs e)
        {
            ConfigureForSingleUser(sender.ToString());

            if (optionsManager.KillSshAgent)
                processKiller.KillSshAgent();
        }

        void ConfigureForSingleUser(string name)
        {
            var gitUser = gitUserManager.GetUserByName(name);

            try
            {
                gitUserManager.ConfigureForSingleUser(gitUser);
                notifyIcon.Icon = iconRepository.GetIconForUser(gitUser);
            }
            catch (FileNotFoundException)
            {
                ShowErrorMessage("SSH key file could not be found at path: " + gitUser.SshKeyPath);
            }
            catch (DirectoryNotFoundException)
            {
                ShowErrorMessage("SSH key file could not be found at path: " + gitUser.SshKeyPath);
            }
            catch (SshKeyHashException)
            {
                ShowErrorMessage("SSH key file has changed at path: " + gitUser.SshKeyPath);
            }
        }

        void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "GitSwitch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        void OnSelectMany(object sender, EventArgs e)
        {
            if (selectManyUsersForm != null)
            {
                selectManyUsersForm.Activate();
            }
            else
            {
                selectManyUsersForm = new SelectManyUsersForm(gitUserManager, iconRepository, notifyIcon);
                selectManyUsersForm.Closed += OnCloseSelectManyUsersForm;
                selectManyUsersForm.Show();
            }
        }

        void OnCloseSelectManyUsersForm(object sender, EventArgs e)
        {
            selectManyUsersForm = null;
        }

        void OnEditUsers(object sender, EventArgs e)
        {
            if (editUsersForm != null)
            {
                editUsersForm.Activate();
            }
            else
            {
                editUsersForm = new EditUsersForm(gitUserManager);
                editUsersForm.Closed += OnCloseEditUsersForm;
                editUsersForm.Show();
            }
        }

        void OnCloseEditUsersForm(object sender, EventArgs e)
        {
            editUsersForm = null;
        }

        void OnLogout(object sender, EventArgs e)
        {
            gitUserManager.ConfigureForSingleUser(new NullGitUser());
            notifyIcon.Icon = defaultIcon;
        }

        void OnHelp(object sender, EventArgs e)
        {
            if (helpForm != null)
            {
                helpForm.Activate();
            }
            else
            {
                helpForm = new HelpForm();
                helpForm.Closed += OnCloseHelpForm;
                helpForm.Show();
            }
        }

        void OnCloseHelpForm(object sender, EventArgs e)
        {
            helpForm = null;
        }

        void OnExit(object sender, EventArgs e)
        {
            OnLogout(sender, e);
            ExitThread();
        }

        protected override void ExitThreadCore()
        {
            notifyIcon.Visible = false;
            base.ExitThreadCore();
        }

        void OnKillSshAgent(object sender, EventArgs e)
        {
            optionsManager.ToggleKillSshAgent();
        }
    }
}
