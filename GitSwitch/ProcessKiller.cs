using System.Diagnostics;

namespace GitSwitch
{
    public class ProcessKiller : IProcessKiller
    {
        public void KillAllProcessesByName(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
