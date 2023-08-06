namespace GameCore.Core;

public class Command
{
    public string Name { get; set; }
    private readonly Action<RegList, string[]> _action;

    public Command(string name, Action<RegList, string[]> action)
    {
        Name = name;
        _action = action;
    }

    public void Invoke(RegList regs, string[] @params)
    {
        _action(regs, @params);
    }
}