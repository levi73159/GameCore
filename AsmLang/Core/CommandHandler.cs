namespace AsmLang.Core;

public class CommandHandler
{
    private readonly Dictionary<string, Command> _commands;
    private Dictionary<string, string> _macros;


    public CommandHandler(IEnumerable<Command> commands)
    {
        _commands = new Dictionary<string, Command>();
        
        foreach (var command in commands)
        {
            _commands.Add(command.Name, command);
        }
    }

    public CommandHandler() : this(Commands.GetBasicCommands())
    {
    }

    public void ExecuteCommands(string[] code, ref RegList regs)
    {
        code = ReplaceImports(code);
        code = code.Select(line => RemoveComment(line)).Where(line => !string.IsNullOrWhiteSpace(line) && !string.IsNullOrEmpty(line)).ToArray();
        
        Commands.GetOS(regs, Array.Empty<string>());
        Commands.Labels = Util.FindLabels(code);
        _macros = Util.FindMacros(code);


        // Replace macros in the code with their values (if they exist) or remove them if they don't
        for (var i = 0; i < code.Length; i++)
        {
            code[i] = ReplaceMacro(code[i]);
        }
        
        regs.Eip = 0;
        while (regs.Eip < code.Length)
        {
            string line = code[regs.Eip].Trim();

            string[] parts = line.Split(' ');
            string command = parts[0];

            if (command.EndsWith(":") || line.StartsWith("#"))
            {
                regs.Eip++;
                continue;
            }

            if (!CallCommand(ref regs, line))
            {
                Console.WriteLine("Program terminated...");
                return;
            }

            regs.Eip++;
        }
    }

    private string[] ReplaceImports(string[] code)
    {
        var lines = new List<string>();
        foreach (var line in code)
        {
            if (line.StartsWith("#import "))
            {
                var fileName = line.Substring(8).Trim();
                if (File.Exists(fileName))
                {
                    var importedCode = File.ReadAllLines(fileName);
                    lines.AddRange(importedCode);
                }
                else
                {
                    Console.WriteLine($"Error: File '{fileName}' not found.");
                }
            }
            else
            {
                lines.Add(line);
            }
        }

        return lines.ToArray();
    }


    private string ReplaceMacro(string line)
    {
        var macroIndex = line.IndexOf('%');
        while (macroIndex >= 0)
        {
            var macroEndIndex = line.IndexOf('%', macroIndex + 1);
            if (macroEndIndex < 0)
            {
                // Invalid macro format, remove the '%' sign
                line = line.Remove(macroIndex, 1);
                macroIndex = line.IndexOf('%', macroIndex);
                continue;
            }
        
            var macroName = line.Substring(macroIndex + 1, macroEndIndex - macroIndex - 1);

            if (_macros.TryGetValue(macroName, out var macro))
            {
                line = line.Remove(macroIndex, macroEndIndex - macroIndex + 1);
                line = line.Insert(macroIndex, macro);
            }
            else
            {
                line = line.Remove(macroIndex, macroEndIndex - macroIndex + 1);
            }

            macroIndex = line.IndexOf('%', macroIndex);
        }

        return line;
    }

    
    private string RemoveComment(string line)
    {
        int commentIndex = line.IndexOf(';');
        if (commentIndex >= 0)
        {
            return line.Substring(0, commentIndex).Trim();
        }
        return line;
    }

    public bool CallCommand(ref RegList regs, string commandString)
    {
        var spl = commandString.Split();
        var name = spl[0];
        var @params = spl.Skip(1).ToArray();

        if (!_commands.ContainsKey(name))
        {
            Console.WriteLine($"Error: command '{name}' not found");
            return false;
        }

        _commands[name].Invoke(regs, @params);
        return true;
    }
    
    
}