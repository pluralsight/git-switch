using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitSwitch
{
    class GitUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string SshKeyPath { get; set; }
        public string SshKeyHash { get; set; }
    }
}
