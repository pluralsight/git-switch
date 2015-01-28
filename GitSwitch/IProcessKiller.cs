namespace GitSwitch
{
    public interface IProcessKiller
    {
        void KillAllProcessesByName(string processName);
    }
}
