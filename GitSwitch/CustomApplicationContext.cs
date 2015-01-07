using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitSwitch
{
    class CustomApplicationContext : ApplicationContext
    {
        private NotifyIcon notifyIcon;
        private GitUserManager gitUserManager;
        private EditUsersForm editUsersForm;

        public CustomApplicationContext()
        {
            InitializeTrayIcon();
            gitUserManager = new GitUserManager(new Serializer(), new Sha1FileHasher());
        }

        private void InitializeTrayIcon()
        {
            notifyIcon = new NotifyIcon()
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon("gitswitch64x64.ico"),
                Text = "GitSwitch",
                Visible = true
            };
            notifyIcon.ContextMenuStrip.Opening += OnContextMenuStripOpening;
        }

        private void OnContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            notifyIcon.ContextMenuStrip.Items.Clear();
            gitUserManager.GetUsers().ForEach(x => notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler(x.Username, OnGitUserClick)));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Edit Users...", OnEditUsers));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&Exit", OnExit));
        }

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler, string tooltipText = null)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null) { item.Click += eventHandler; }

            item.ToolTipText = tooltipText;
           
            return item;
        }

        private void OnGitUserClick(object sender, EventArgs e)
        {
            // TODO: Set the user.
            string username = sender.ToString();
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

        private void OnExit(object sender, EventArgs e)
        {
            ExitThread();
        }

        protected override void ExitThreadCore()
        {
            notifyIcon.Visible = false;
            base.ExitThreadCore();
        }
    }
}
