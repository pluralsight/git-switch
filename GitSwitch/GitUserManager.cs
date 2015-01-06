using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitSwitch
{
    class GitUserManager
    {
        private const string GitUserFile = "gitusers.xml";

        private List<GitUser> users = new List<GitUser>();

        public List<GitUser> GetUsers()
        {
            return users;
        }
    }
}
