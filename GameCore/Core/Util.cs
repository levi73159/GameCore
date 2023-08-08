using System.Text;

namespace GameCore.Core;

internal static class Util
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
            string[] parts = trimmedLine.Substring(1).Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

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

    private const int MinMemory = 10000;
    public static void InitializeMemory(ref int[] memory, ref Dictionary<string, uint> memoryLabels, uint memSize)
    {
        if (memSize < MinMemory)
            return;
        memory = new int[memSize];
        memoryLabels = new Dictionary<string, uint>();
        memory[0] = 't';
        memory[1] = 'e';
        memory[2] = 's';
        memory[3] = 't';
        memory[4] = '\n';
        memory[5] = '\0';
        memoryLabels["Text"] = 0x00;

        memoryLabels["Reserved"] = memSize - 5000; // how many bits in reserved space?
    }

    public static (int startAddress, int endAddress) GetFreeMemory(int[] memory, int desiredLength = 0, int padding = 0)
    {
        var start = -1;
        var end = -1;
        var currentLength = 0;

        for (var i = 0; i < memory.Length; i++)
        {
            if (memory[i] != 0)
            {
                currentLength = 0;
                start = -1;
                end = -1;
                continue;
            }

            if (start == -1)
            {
                start = i;
            }

            currentLength++;
            end = i;


            if (desiredLength <= 0 || currentLength < desiredLength) continue;
            
            if (padding == 0)
                return (start, end);

            var startPadding = start - padding;
            var endPadding = end + padding;
            var notCleared = false;
            
            if (startPadding >= 0)
            {
                for (var j = startPadding; j < start; j++)
                {
                    if (memory[j] == 0)
                        continue;

                    start++;
                    currentLength = 0;
                    end = -1;
                    notCleared = true;
                    break;
                }
                
                if (notCleared) continue;
            }

            if (endPadding >= memory.Length) return (start, end);
            
            for (var j = endPadding; j > end; j--)
            {
                if (memory[j] == 0)
                    continue;

                start++;
                currentLength = 0;
                end = -1;
                notCleared = true;
                break;
            }

            if (notCleared) continue;

            return (start, end);
        }

        return (start, end);
    }


    
    // Read a value from memory at the specified address
    public static int ReadMemory(int[] memory, uint address)
    {
        if (address < memory.Length)
        {
            return memory[address];
        }

        Console.WriteLine($"Error: Attempted to read from invalid memory address 0x{address:X}");
        return 0;
    }

    public static string? ReadString(int[] memory, uint address)
    {
        if (address >= memory.Length)
        {
            Console.WriteLine($"Error: Attempted to read from invalid memory address 0x{address:X}");
            return null;
        }

        var sb = new StringBuilder();
        
        for (var i = address; i < memory.Length; i++)
        {
            if (memory[i] == '\0')
                break;
            
            sb.Append(Convert.ToChar(memory[i]));
        }

        return sb.ToString();
    }

    // Write a value to memory at the specified address
    public static void WriteMemory(int[] memory, uint address, int value)
    {
        if (address < memory.Length)
        {
            memory[address] = value;
        }
        else
        {
            Console.WriteLine($"Error: Attempted to write to invalid memory address {address}");
        }
    }

    // Print a range of memory addresses and their values
    public static void PrintRawMemory(int[] memory, uint startAddress, uint length)
    {
        var endAddress = startAddress + length;
        if (startAddress <= endAddress && endAddress < memory.Length)
        {
            for (uint address = startAddress; address <= endAddress; address++)
            {
                Console.WriteLine($"{memory[address]}");
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid memory range specified.");
        }
    }
    
    public static void PrintMemory(int[] memory, uint startAddress)
    {
        for (var i = startAddress; i < memory.Length; i++)
        {
            if (memory[i] == '\0')
                return;
            
            Console.Write(Convert.ToChar(memory[i]));
        }
    }

    public static int CompareMemory(int[] memory, uint addressA, uint addressB, uint size)
    {
        for (uint i = 0; i < size; i++)
        {
            var valueA = memory[addressA + i];
            var valueB = memory[addressB + i];

            if (valueA < valueB)
                return -1; // Memory block at addressA is less than memory block at addressB
            if (valueA > valueB)
                return 1; // Memory block at addressA is greater than memory block at addressB
        }

        return 0; // Memory blocks are equal
    }

    public static int CompareString(int[] memory, uint addressA, uint addressB)
    {
        int valueA, valueB;
        uint offset = 0;

        do
        {
            valueA = memory[addressA + offset];
            valueB = memory[addressB + offset];

            if (valueA < valueB)
                return -1; // String at addressA is less than string at addressB
            if (valueA > valueB)
                return 1; // String at addressA is greater than string at addressB

            offset++;
        }
        while (valueA != '\0' && valueB != '\0');

        return 0; // Strings are equal
    }
    
    // Fill a range of memory addresses with a given value
    public static void FillMemory(int[] memory, uint startAddress, uint endAddress, int value)
    {
        if (startAddress <= endAddress && endAddress < memory.Length)
        {
            for (uint address = startAddress; address <= endAddress; address++)
            {
                memory[address] = value;
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid memory range specified.");
        }
    }

    // Copy a block of memory from one address to another
    public static void CopyMemory(int[] memory, uint sourceAddress, uint destinationAddress, uint length)
    {
        if (sourceAddress + length <= memory.Length && destinationAddress + length <= memory.Length)
        {
            Array.Copy(memory, sourceAddress, memory, destinationAddress, length);
        }
        else
        {
            Console.WriteLine("Error: Invalid memory range specified.");
        }
    }
}