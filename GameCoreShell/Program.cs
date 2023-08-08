using GameCore;
using GameCore.Core;


namespace GameCoreShell;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 0)
        {
            var file = args[0];
            Runner.RunFile(file);
            return;
        }
        
        Console.WriteLine("Welcome to the Game Core Shell");
        Console.WriteLine("Type 'help' for a list of commands");
        
        var commands = new[]
        {
            new Command("help", () =>
            {
                Console.WriteLine("help: list commands");
                Console.WriteLine("repl: run the Game Core REPL");
                Console.WriteLine("run: run a file");
                Console.WriteLine("exit: exits\nclear: clears");
            }),
            new Command("repl", () =>
            {
                var regs = new RegList();
                Runner.RunRepl(regs);
            }),
            new Command("run", () =>
            {
                Console.Write("file > ");
                var file = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(file))
                    return;

                Runner.RunFile(file);
            }),
            new Command("clear", Console.Clear),
            new Command("exit", () => Environment.Exit(0))
        };

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var command = commands.FirstOrDefault(c => c.Name == line);
            if (command == null)
            {
                Console.WriteLine("Not a valid command");
                continue;
            }
            
            command.OnRun.Invoke();
        }
    }
}