using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitSwitch
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            SetHelpText();
        }

        public void SetHelpText()
        {
            var helpText = "GitSwitch version " + Assembly.GetExecutingAssembly().GetName().Version +
                           "\n\nUse the \"Edit Users...\" form to add user information." +
                           " Then click on the user from the right-click menu to change your git user.name, user.email and the SSH key used for github.com." +
                           " You can keep your SSH key on a USB drive, provided that the drive letter is more-or-less constant" +
                           "\n\nGitSwitch will warn you if the SSH key has changed or cannot be found." +
                           " If this happens, make sure to edit your user configuration via the \"Edit Users...\" form." +
                           "\n\nVisit the GitSwitch GitHub repository for more information: https://github.com/pluralsight/git-switch";
            HelpRichTextBox.Text = helpText;
        }
    }
}
