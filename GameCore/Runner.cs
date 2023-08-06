using GameCore.Core;

namespace GameCore;

public static class Runner
{
    public static RegList RunFile(string path, RegList? regs = null)
    {
        regs ??= new RegList();
        var text = File.ReadAllLines(path);
        var cmdHandler = new CommandHandler();
        cmdHandler.ExecuteCommands(text, ref regs);
        return regs;
    }

    public static RegList RunRepl(RegList? regs = null)
    {
        regs ??= new RegList();
        var commands = Commands.GetBasicCommands().ToDictionary(cmd => cmd.Name);
        var isRunning = true;
        commands["exit"] = new Command("exit", (_, _) => isRunning = false);
        var cmdHandler = new CommandHandler(commands.Values);
        while (isRunning)
        {
            Console.Write(">>> ");
            var text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text))
                continue;
            cmdHandler.ExecuteCommands(new[] { text }, ref regs);
        }
        return regs;
    }

    public static RegList Run(string[] lines, RegList? regs = null)
    {
        regs ??= new RegList();
        var cmdHandler = new CommandHandler();
        cmdHandler.ExecuteCommands(lines, ref regs);
        return regs;
    }
}