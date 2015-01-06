using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitSwitch
{
    public partial class EditUsersForm : Form
    {
        private GitUserManager gitUserManager;
        private GitUser currentGitUser;

        public EditUsersForm(GitUserManager gitUserManager)
        {
            this.gitUserManager = gitUserManager;
            InitializeComponent();
            SetCurrentGitUser(null);
            RefreshUsersListBox();
        }

        private void RefreshUsersListBox()
        {
            UsersListBox.ClearSelected();
            UsersListBox.Items.Clear();
            gitUserManager.GetUsers().ForEach(x => UsersListBox.Items.Add(x.Username));
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SshKeyFileTextBox.Text = dialog.FileName;
            }
        }

        private void AddNewUserButton_Click(object sender, EventArgs e)
        {
            SetCurrentGitUser(null);
        }

        private void SetCurrentGitUser(GitUser gitUser)
        {
            currentGitUser = gitUser;

            GitUser temp = currentGitUser ?? new GitUser();
            UsernameTextBox.Text = temp.Username;
            EmailTextBox.Text = temp.Email;
            SshKeyFileTextBox.Text = temp.SshKeyPath;
            SaveButton.Text = currentGitUser == null ? "Save New User" : "Update User";

            if (currentGitUser == null)
            {
                UsersListBox.ClearSelected();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (currentGitUser == null)
            {
                currentGitUser = new GitUser();
            }
            currentGitUser.Username = UsernameTextBox.Text.Trim();
            currentGitUser.Email = EmailTextBox.Text.Trim();
            currentGitUser.SshKeyPath = SshKeyFileTextBox.Text;
            gitUserManager.AddUser(currentGitUser);

            RefreshUsersListBox();
            SetCurrentGitUser(null);
        }

        private void UsersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCurrentGitUser(GetSelectedGitUser());
        }

        private GitUser GetSelectedGitUser()
        {
            if (UsersListBox.SelectedItem == null)
            {
                return null;
            }
            return gitUserManager.GetUserByUsername(UsersListBox.SelectedItem.ToString());
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            GitUser userToDelete = GetSelectedGitUser();
            if (userToDelete != null)
            {
                gitUserManager.RemoveUser(userToDelete);
                RefreshUsersListBox();
            }
        }
    }
}
