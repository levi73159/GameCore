namespace GameCore.Core;

public class CommandHandler
{
    private readonly Dictionary<string, Command> _commands;
    private Dictionary<string, string> _macros;

    public CommandHandler(IEnumerable<Command> commands)
    {
        _commands = new Dictionary<string, Command>();
        _macros = new Dictionary<string, string>();

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
        Util.InitializeMemory(ref regs.Memory, ref regs.MemoryLabels, 65775);
        code = ReplaceImports(code);
        code = code.Select(line => RemoveComment(line)).Where(line => !string.IsNullOrWhiteSpace(line) && !string.IsNullOrEmpty(line)).ToArray();

        Commands.GetOs(regs, Array.Empty<string>());
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
        var spl = commandString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var name = spl[0];
        var paramsString = string.Join(" ", spl.Skip(1));
        var @params = ParseParameters(paramsString);

        if (!_commands.ContainsKey(name))
        {
            Console.WriteLine($"Error: command '{name}' not found");
            return false;
        }

        _commands[name].Invoke(regs, @params);
        return true;
    }

    private string[] ParseParameters(string paramsString)
    {
        var parameters = new List<string>();
        var currentParam = "";
        var inQuote = false;

        for (int i = 0; i < paramsString.Length; i++)
        {
            char c = paramsString[i];

            if (c == '"' && (i == 0 || paramsString[i - 1] != '\\'))
            {
                inQuote = !inQuote;
            }
            else if (c == ' ' && !inQuote)
            {
                parameters.Add(currentParam);
                currentParam = "";
            }
            else
            {
                currentParam += c;
            }
        }

        if (!string.IsNullOrWhiteSpace(currentParam))
        {
            parameters.Add(currentParam);
        }

        return parameters.ToArray();
    }
}