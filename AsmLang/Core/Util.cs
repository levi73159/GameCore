namespace AsmLang.Core;

public static class Util
{
    public static Dictionary<string, int> FindLabels(string[] code)
    {
        var labels = new Dictionary<string, int>();
        var currentLine = 0;

        foreach (var line in code)
        {
            string trimmedLine = line.Trim();
            if (!string.IsNullOrEmpty(trimmedLine))
            {
                string[] parts = trimmedLine.Split(' ');
                string command = parts[0];

                if (command.EndsWith(":"))
                {
                    // It's a label, add it to the labels dictionary with its line number
                    labels[command.TrimEnd(':')] = currentLine;
                }

                currentLine++;
            }
        }

        return labels;
    }

    public static Dictionary<string, string> FindMacros(string[] code)
    {
        var macros = new Dictionary<string, string>();
        foreach (var line in code)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine)) continue;
            if (!trimmedLine.StartsWith("#")) continue;
            
            // Remove the '#' character and split the line into name and value parts
            string[] parts = trimmedLine.Substring(1).Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid macro format: " + trimmedLine);
                continue;
            }

            string macroName = parts[0];
            string macroValue = parts[1];

            // Add the macro to the dictionary
            macros[macroName] = macroValue;
        }

        return macros;
    }

}