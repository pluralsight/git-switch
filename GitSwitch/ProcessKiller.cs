using System.Diagnostics;

namespace GitSwitch
{
    public class ProcessKiller : IProcessKiller
    {
        private const string SshAgentProcessName = "ssh-agent";

        public void KillSshAgent()
        {
            foreach (var process in Process.GetProcessesByName(SshAgentProcessName))
            {
                process.Kill();
            }
        }
    }
}
