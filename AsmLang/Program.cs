using AsmLang.Core;

namespace AsmLang;

public class Program
{
    private static RegList _regs = new RegList();

    public static void Main()
    {
        var code = File.ReadAllLines("code.gc");


        var commandHandler = new CommandHandler();
        commandHandler.ExecuteCommands(code, ref _regs);
    }
}