﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitSwitch
{
    public class GitUserManager
    {
        private const string GitUserFile = "gitusers.xml";

        private List<GitUser> users = new List<GitUser>();

        public List<GitUser> GetUsers()
        {
            return users;
        }

        public void AddUser(GitUser gitUser)
        {
            // TODO: Validate the gitUser and set the ssh key hash.
            if (!users.Contains(gitUser))
            {
                users.Add(gitUser);
            }
        }

        public GitUser GetUserByUsername(string username)
        {
            return users.FirstOrDefault(x => x.Username == username);
        }

        public void RemoveUser(GitUser gitUser)
        {
            users.Remove(gitUser);
        }
    }
}
