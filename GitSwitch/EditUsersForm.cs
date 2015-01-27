using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            ErrorsLabel.Text = "";

            if (currentGitUser == null)
            {
                UsersListBox.ClearSelected();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var userForValidation = CreateGitUserFromForm();
            var validationErrors = GetValidationErrors(userForValidation);
            if (validationErrors.Count > 0)
            {
                ErrorsLabel.Text = string.Join("\r\n", validationErrors);
            }
            else
            {
                UpdateCurrentUser(userForValidation);
                SaveCurrentGitUser();
            }
        }

        private GitUser CreateGitUserFromForm()
        {
            var user = new GitUser();
            user.Username = UsernameTextBox.Text.Trim();
            user.Email = EmailTextBox.Text.Trim();
            user.SshKeyPath = SshKeyFileTextBox.Text;
            return user;
        }

        private List<string> GetValidationErrors(GitUser gitUser)
        {
            var validationErrors = new List<string>();
            if (string.IsNullOrEmpty(gitUser.Username))
            {
                validationErrors.Add("You must provide a git user.name");
            }
            if (string.IsNullOrEmpty(gitUser.Email))
            {
                validationErrors.Add("You must provide a git user.email");
            }
            if (string.IsNullOrEmpty(gitUser.SshKeyPath))
            {
                validationErrors.Add("You must select an SSH key file");
            }
            return validationErrors;
        }

        private void UpdateCurrentUser(GitUser newGitUser)
        {
            if (currentGitUser == null)
            {
                currentGitUser = newGitUser;
            }
            else
            {
                currentGitUser.Email = newGitUser.Email;
                currentGitUser.Username = newGitUser.Username;
                currentGitUser.SshKeyPath = newGitUser.SshKeyPath;
            }
        }

        private void SaveCurrentGitUser() {
            try
            {
                gitUserManager.AddUser(currentGitUser);
            }
            catch (FileNotFoundException fnf)
            {
                MessageBox.Show(fnf.Message);
            }

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
