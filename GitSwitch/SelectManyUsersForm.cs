using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GitSwitch
{
    public partial class SelectManyUsersForm : Form
    {
        readonly IGitUserManager gitUserManager;
        readonly IIconRepository iconRepository;
        readonly NotifyIcon notifyIcon;

        public SelectManyUsersForm(IGitUserManager gitUserManager, IIconRepository iconRepository, NotifyIcon notifyIcon)
        {
            this.gitUserManager = gitUserManager;
            this.iconRepository = iconRepository;
            this.notifyIcon = notifyIcon;

            InitializeComponent();
            InitializeUsersListBox();
        }

        void InitializeUsersListBox()
        {
            UsersListBox.ClearSelected();
            UsersListBox.Items.Clear();

            gitUserManager.Users.ForEach(x =>
            {
                UsersListBox.Items.Add(x.Name);

                if(gitUserManager.ActiveUsers.Contains(x))
                    UsersListBox.SelectedItems.Add(x.Name);
            });
        }

        void SaveButton_Click(object sender, EventArgs e)
        {
            MarkActiveUsers();

            Close();
        }

        void MarkActiveUsers()
        {
            var users = GetSelectedUsers();

            try
            {
                gitUserManager.ActiveUsers.Clear();
                users.ForEach(u => gitUserManager.MakeUserActive(u, isPairOrMob: true));
                gitUserManager.ConfigureForActiveUsers();

                notifyIcon.Icon = iconRepository.GetIconForUser(gitUserManager.ActiveUsers.First());
            }
            catch (FileNotFoundException fnf)
            {
                MessageBox.Show(fnf.Message);
            }
        }

        List<GitUser> GetSelectedUsers()
        {
            var selectedUsers = new List<GitUser>();

            foreach (var item in UsersListBox.SelectedItems)
            {
                var user = gitUserManager.GetUserByName(item.ToString());
                selectedUsers.Add(user);
            }

            return selectedUsers;
        }
    }
}
