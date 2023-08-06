namespace GameCoreShell;

public class Command
{
    public string Name { get; set; }
    public Action OnRun { get; set; }

    public Command(string name, Action onRun)
    {
        Name = name;
        OnRun = onRun;
    }
}