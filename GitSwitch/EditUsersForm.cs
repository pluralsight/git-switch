using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GitSwitch
{
    public partial class EditUsersForm : Form
    {
        readonly IGitUserManager gitUserManager;

        GitUser currentUser;

        public EditUsersForm(IGitUserManager gitUserManager)
        {
            this.gitUserManager = gitUserManager;

            InitializeComponent();
            InitializeGitUserEditForm(new NullGitUser());
            RefreshUsersListBox();
        }

        void RefreshUsersListBox()
        {
            UsersListBox.ClearSelected();
            UsersListBox.Items.Clear();

            gitUserManager.Users.ForEach(x => UsersListBox.Items.Add(x.Name));
        }

        void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
                SshKeyFileTextBox.Text = dialog.FileName;
        }

        void AddNewUserButton_Click(object sender, EventArgs e)
        {
            InitializeGitUserEditForm(new NullGitUser());
        }

        void InitializeGitUserEditForm(GitUser user)
        {
            currentUser = user;

            NameTextBox.Text = currentUser.Name;
            EmailTextBox.Text = currentUser.Email;
            SshKeyFileTextBox.Text = currentUser.SshKeyPath;
            SaveButton.Text = currentUser is NullGitUser ? "Save New User" : "Update User";
            ErrorsLabel.Text = "";

            if (currentUser is NullGitUser)
                UsersListBox.ClearSelected();
        }

        void SaveButton_Click(object sender, EventArgs e)
        {
            UpdateCurrentUserFromFrom();

            var validationErrors = GetValidationErrors(currentUser);
            if (validationErrors.Any())
                ErrorsLabel.Text = string.Join("\r\n", validationErrors);
            else
                SaveUser(currentUser);
        }

        void UpdateCurrentUserFromFrom()
        {
            var name = NameTextBox.Text.Trim();
            var email = EmailTextBox.Text.Trim();
            var sshKeyPath = SshKeyFileTextBox.Text;

            if(currentUser is NullGitUser)
                currentUser = new GitUser(name, email, sshKeyPath);

            currentUser.Name = name;
            currentUser.Email = email;
            currentUser.SshKeyPath = sshKeyPath;
        }

        List<string> GetValidationErrors(GitUser user)
        {
            var validationErrors = new List<string>();
            if (string.IsNullOrEmpty(user.Name))
                validationErrors.Add("You must provide a name");

            if (string.IsNullOrEmpty(user.Email))
                validationErrors.Add("You must provide an email");

            if (string.IsNullOrEmpty(user.SshKeyPath))
                validationErrors.Add("You must select an SSH key file");

            return validationErrors;
        }

        void SaveUser(GitUser user)
        {
            try
            {
                gitUserManager.AddUser(user);
            }
            catch (FileNotFoundException fnf)
            {
                MessageBox.Show(fnf.Message);
            }

            RefreshUsersListBox();
            InitializeGitUserEditForm(new NullGitUser());
        }

        void UsersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitializeGitUserEditForm(GetSelectedGitUser());
        }

        GitUser GetSelectedGitUser()
        {
            if (UsersListBox.SelectedItem == null)
                return new NullGitUser();

            return gitUserManager.GetUserByName(UsersListBox.SelectedItem.ToString());
        }

        void DeleteButton_Click(object sender, EventArgs e)
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
