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

        public CustomApplicationContext()
        {
            InitializeTrayIcon();
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
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&Add User...", OnAddUser));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&Exit", onExit));
        }

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler, string tooltipText = null)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null) { item.Click += eventHandler; }

            item.ToolTipText = tooltipText;
           
            return item;
        }

        private void OnAddUser(object sender, EventArgs e)
        {
            // TODO: Show the form to add a new user.
        }

        private void onExit(object sender, EventArgs e)
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
