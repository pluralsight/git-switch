using System.Diagnostics;

namespace GitSwitch
{
    public interface IProcessKiller
    {
        void KillSshAgent();
    }

    public class ProcessKiller : IProcessKiller
    {
        public void KillSshAgent()
        {
            foreach (var process in Process.GetProcessesByName(AppConstants.SshAgentProcessName))
                process.Kill();
        }
    }
}
