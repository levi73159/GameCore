using AsmLang.Core;

namespace AsmLang;

public class Program
{
    private static RegList _regs = new RegList();

    public static void Main(string[] args)
    {
        var path = "";
        if (args.Length == 0)
        {
            Console.WriteLine("1. Run File");
            Console.WriteLine("2. Run REPL");
            var choice = Console.ReadKey(true).Key;

            if (choice != ConsoleKey.D1)
                RunRepl();
                
            Console.Write("Path > ");
            path = Console.ReadLine();
        }
        else
            path = args[0];

        if (path == null) return;
        
        var code = File.ReadAllLines(path);

        var commandHandler = new CommandHandler();
        commandHandler.ExecuteCommands(code, ref _regs);
    }

    private static void RunRepl()
    {
        while (true)
        {
            Console.Write(">>> ");
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            var cmdHandler = new CommandHandler();
            cmdHandler.ExecuteCommands(new[] { line }, ref _regs);
        }
    }
}