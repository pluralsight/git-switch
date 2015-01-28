namespace GitSwitch
{
    public interface IOptionsController
    {
        bool KillSshAgent { get; }
        void ToggleKillSshAgent();
    }
}