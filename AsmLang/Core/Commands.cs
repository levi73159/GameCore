namespace AsmLang.Core;
using System.Diagnostics;


public static class Commands
{
    public static Dictionary<string, int> Labels { get; set; }



    public static IEnumerable<Command> GetBasicCommands()
    {
        return new[]
        {
            new Command("set", Set),
            new Command("setAV", SetAV),
            new Command("setLV", SetLV),
            new Command("add", Add),
            new Command("div", Div),
            new Command("mod", Mod),
            new Command("sub", Sub),
            new Command("mul", Mul),
            new Command("xor", Xor),
            new Command("and", And),
            new Command("or", Or),
            new Command("regs", GetRegs),
            new Command("show", Show),
            new Command("movSrc", MoveSrc),
            new Command("sf", SetFlags),
            new Command("mov", Move),
            new Command("inc", Increment),
            new Command("dec", Decrement),
            new Command("cmp", Compare),
            new Command("cmpSrc", CompareSrc),
            new Command("clear", Clear),
            new Command("push", Push),
            new Command("pop", Pop),
            new Command("writeln", WriteLn),
            new Command("write", Write),
            new Command("jmp", JumpUnconditional),
            new Command("jz", JumpEqual),
            new Command("jnz", JumpNotEqual),
            new Command("jg", JumpGreater),
            new Command("jl", JumpLess),
            new Command("jf", JumpFail),
            new Command("jnf", JumpNotFail),
            new Command("jo", JumpOverflow),
            new Command("readline", ReadLine),
            new Command("getkey", GetKey),
            new Command("writeSrc", WriteSrc),
            new Command("writeAV", WriteAV),
            new Command("writeLV", WriteLV),
            new Command("newline", NewLine),
            new Command("wait", Wait),
            new Command("call", Call),
            new Command("ret", Return),
            new Command("exit", Exit),
            new Command("convertAV", ConvertSrcToAV),
            new Command("shl", ShiftLeft),
            new Command("shr", ShiftRight),
            new Command("rand", Random),
            new Command("beep", Beep),
            new Command("getOS", GetOS),
            new Command("not", Not),
            new Command("cmov", ConditionalMove),
            new Command("loop", Loop),
            new Command("readMem", ReadMem),
            new Command("writeRawMem", WriteRawMem),
            new Command("writeMem", WriteMem),
            new Command("printRawMem", PrintRawMem),
            new Command("printMem", PrintMem),
            new Command("fillMem", FillMem),
            new Command("copyMem", CopyMem),
            new Command("cmpMem", CompareMemory),
            new Command("cmpStr", CompareString),
            new Command("break", BreakPoint)
        };
    }

    // Read a value from memory at the specified address and store it in the destination register's AV
    public static void ReadMem(RegList regs, string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Error: readMem command expects 2 argument.");
            return;
        }

        if (!int.TryParse(args[0], out var destAddress)) destAddress = regs.GetAV(args[0]);
        var memory = Util.ReadMemory(regs.Memory, (uint)destAddress);
        
        regs.SetRegisterValue(".mem", Convert.ToChar(memory));
        regs.SetRegisterValue(".mem", memory);
    }

    // Write a value to memory at the specified address
    public static void WriteRawMem(RegList regs, string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Error: writeMem command expects 2 arguments.");
            return;
        }
        
        if (!int.TryParse(args[1], out var value))
            value = regs.GetAV(args[1]);
        

        if (!int.TryParse(args[0], out var destAddress))
            destAddress = regs.GetAV(args[0]);
        
        Util.WriteMemory(regs.Memory, (uint)destAddress, value);
    }
    
    public static void WriteMem(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: writeMem command expects 2 arguments.");
            return;
        }
        
        if (!int.TryParse(args[0], out var destAddress))
            destAddress = regs.GetAV(args[0]);


        var offset = 0;
        var n = 1;
        
        
        while (n < args.Length)
        {
            var text = args[n];
            if (int.TryParse(text, out var val))
            {
                Util.WriteMemory(regs.Memory, (uint)(destAddress + offset), val);
                offset++;
                n++;
                continue;
            }
            int i;

            for (i = 0; i < text.Length; i++)
            {
                Util.WriteMemory(regs.Memory, (uint)(destAddress + i + offset), text[i]);
            }

            offset += i;
            n++;
        }
    }

    public static void BreakPoint(RegList regs, string[] args)
    {
        Debugger.Break();
    }
    
    // Print memory contents within the specified range
    public static void PrintRawMem(RegList regs, string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Error: printMem command expects 2 arguments.");
            return;
        }

        if (int.TryParse(args[0], out int startAddress) && int.TryParse(args[1], out int length))
        {
            Util.PrintRawMemory(regs.Memory, (uint)startAddress, (uint)length);
        }
        else
        {
            Console.WriteLine($"Error: Invalid memory range specified.");
        }
    }

    public static void PrintMem(RegList regs, string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Error: printMem command expects 2 arguments.");
            return;
        }

        if (int.TryParse(args[0], out int startAddress))
        {
            Util.PrintMemory(regs.Memory, (uint)startAddress);
        }
        else
        {
            Console.WriteLine($"Error: Invalid memory range specified.");
        }
    }
    
    // Fill memory within the specified range with a given value
    public static void FillMem(RegList regs, string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Error: fillMem command expects 3 arguments.");
            return;
        }

        if (!int.TryParse(args[0], out var start))
            start = regs.GetAV(args[0]);
        
        if (!int.TryParse(args[1], out var end))
            end = regs.GetAV(args[1]);
        
        if (!int.TryParse(args[2], out var value))
            value = regs.GetAV(args[2]);

        Util.FillMemory(regs.Memory, (uint)start, (uint)end, value);
    }

    // Copy a block of memory from sourceAddress to destAddress with the specified length
    public static void CopyMem(RegList regs, string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Error: copyMem command expects 3 arguments.");
            return;
        }

        if (!int.TryParse(args[0], out var src))
            src = regs.GetAV(args[0]);
        
        if (!int.TryParse(args[1], out var dest))
            dest = regs.GetAV(args[1]);
        
        if (!int.TryParse(args[2], out var len))
            len = regs.GetAV(args[2]);
        
        Util.CopyMemory(regs.Memory, (uint)src, (uint) dest, (uint)len);
    }

    public static void CompareMemory(RegList regs, string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Error: cmpMem command expects 3 arguments.");
            return;
        }

        if (!int.TryParse(args[0], out var memoryAddressA))
            memoryAddressA = regs.GetAV(args[0]);

        if (!int.TryParse(args[1], out var memoryAddressB))
            memoryAddressB = regs.GetAV(args[1]);

        if (!int.TryParse(args[2], out var size))
            size = regs.GetAV(args[2]);

        // Ensure that memoryAddressA and memoryAddressB are valid addresses
        if (memoryAddressA < 0 || memoryAddressA >= regs.Memory.Length ||
            memoryAddressB < 0 || memoryAddressB >= regs.Memory.Length)
        {
            regs.SetFlag(Flags.Overflow, true);
            return;
        }

        // Ensure that size is within valid range
        if (size <= 0 || memoryAddressA + size > regs.Memory.Length || memoryAddressB + size > regs.Memory.Length)
        {
            regs.SetFlag(Flags.Overflow, true);
            return;
        }

        // Compare memory blocks
        int result = Util.CompareMemory(regs.Memory, (uint)memoryAddressA, (uint)memoryAddressB, (uint)size);

        regs.SetFlag(Flags.Zero, result == 0);
        regs.SetFlag(Flags.Negative, result < 0);
        regs.SetFlag(Flags.Positive, result > 0);
        regs.SetFlag(Flags.Overflow, false);
    }

    public static void CompareString(RegList regs, string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Error: cmpStr command expects 2 arguments.");
            return;
        }

        if (!int.TryParse(args[0], out var memoryAddressA))
            memoryAddressA = regs.GetAV(args[0]);

        var isGen = false;
        if (!int.TryParse(args[1], out var memoryAddressB))
        {
            if (regs.TryGetValue(args[1], out var reg))
            {
                memoryAddressB = reg.AV;
            }
            else
            {
                isGen = true;
                var text = args[1];
                memoryAddressB = (int)regs.MemoryLabels["Reserved"];
                for (var i = 0; i < text.Length; i++)
                    Util.WriteMemory(regs.Memory, (uint)(memoryAddressB + i), text[i]);
                Util.WriteMemory(regs.Memory, (uint)(memoryAddressB + text.Length + 1), '\0');
            }
        }

        // Ensure that memoryAddressA and memoryAddressB are valid addresses
        if (memoryAddressA < 0 || memoryAddressA >= regs.Memory.Length ||
            memoryAddressB < 0 || memoryAddressB >= regs.Memory.Length)
        {
            regs.SetFlag(Flags.Overflow, true);
            return;
        }

        // Compare memory blocks
        var result = Util.CompareString(regs.Memory, (uint)memoryAddressA, (uint)memoryAddressB);
        
        if (isGen)
            Util.FillMemory(regs.Memory, (uint) memoryAddressB, (uint) args[1].Length, 0);

        regs.SetFlag(Flags.Zero, result == 0);
        regs.SetFlag(Flags.Negative, result < 0);
        regs.SetFlag(Flags.Positive, result > 0);
        regs.SetFlag(Flags.Overflow, false);
    }
    
    private static void ConditionalMove(RegList regs, string[] args)
    {
        if (!Enum.TryParse(args[2], out Flags flag))
        {
            regs.SetFlag(Flags.Failure, true);
            return;
        }
        
        if (!regs.GetFlag(flag)) return;
            
        Move(regs, args);
    }

    private static void Not(RegList regs, string[] args)
    {
        regs.SetRegisterValue(args[0], ~regs.GetAV(args[0]));
    }

    public static void GetOS(RegList regs, string[] args)
    {
        var reg = regs["dr0"];

        reg.AV = (int)Environment.OSVersion.Platform;
        reg.Value = Environment.OSVersion.VersionString;
    }

    private static void Beep(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            if (args.Length == 0)
            {
                Console.Beep();
                return;
            }

            Console.WriteLine("Error: Invalid arguments for 'beep'");
            return;
        }
        
        if (!int.TryParse(args[0], out var frequency))
            frequency = regs.GetAV(args[0]);

        if (!int.TryParse(args[1], out var duration))
            duration = regs.GetAV(args[1]);

        if (OperatingSystem.IsWindows())
            Console.Beep(frequency, duration);
        else
            Console.WriteLine("beep only supported for Windows");
    }

    private static void SetLV(RegList regs, string[] args)
    {
        if (!long.TryParse(args[1], out var lv))
            lv = regs.GetLV(args[1]);
        regs.SetRegisterValue(args[0], lv);
    }

    private static void SetAV(RegList regs, string[] args)
    {
        if (!int.TryParse(args[1], out var av))
            av = regs.GetAV(args[1]);
        regs.SetRegisterValue(args[0], av);
    }

    private static void Set(RegList regs, string[] args)
    {
        regs.SetRegisterValue(args[0], string.Join(' ', args.Skip(1)));
    }

    private static void Wait(RegList _, string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Error: Invalid number of arguments for wait");
            return;
        }

        if (!int.TryParse(args[0], out var timeout)) return;
        Thread.Sleep(timeout);
    }

    private static void NewLine(RegList regList, string[] strings)
    {
        Console.WriteLine();
    }

    private static void WriteSrc(RegList regs, string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Error: Invalid number of arguments for writeSrc");
            return;
        }

        Console.Write(regs.GetValue(args[0]));
    }
    
    private static void WriteAV(RegList regs, string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Error: Invalid number of arguments for writeAV");
            return;
        }

        Console.Write(regs.GetAV(args[0]));
    }
    
    private static void WriteLV(RegList regs, string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Error: Invalid number of arguments for writeLV");
            return;
        }

        Console.Write(regs.GetLV(args[0]));
    }

    private static void Exit(RegList regs, string[] _) => Environment.Exit(regs.GetAV("dr7"));

    private static void GetKey(RegList regs, string[] args)
    {
        var regName = "cr1";
        if (args.Length >= 1)
        {
            regName = args[0];
        }

        var keyInfo = Console.ReadKey(true);
        regs[regName].Value = keyInfo.KeyChar;
        regs[regName].AV = (int)keyInfo.Key;
        regs[regName].FV = (int)keyInfo.Modifiers;
    }

    private static void ReadLine(RegList regs, string[] args)
    {
        var regName = "cr0";
        if (args.Length >= 1)
        {
            regName = args[0];
        }

        var text = Console.ReadLine();
        regs[regName].Value = text;
    }

    private static void ConvertSrcToAV(RegList regs, string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please specify a valid register.");
            return;
        }
        
        var reg = regs[args[0]];
        if (!int.TryParse(reg.Value?.ToString(), out var v))
        {
            regs.SetFlag(Flags.Failure, true);
            return;
        }

        regs.SetFlag(Flags.Failure, false);
        reg.AV = v;
    }

    private static void Write(RegList _, string[] args)
    {
        var msg = string.Join(' ', args);
        Console.Write(msg);
    }

    private static void WriteLn(RegList _, string[] args)
    {
        var msg = string.Join(' ', args);
        Console.WriteLine(msg);
    }

    private static void Clear(RegList regList, string[] strings)
    {
        Console.Clear();
    }

    private static void Compare(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for cmp command.");
            return;
        }

        if (!int.TryParse(args[0], out var value1))
            value1 = regs.GetAV(args[0]);
        
        if (!int.TryParse(args[1], out var value2))
            value2 = regs.GetAV(args[1]);

        regs.SetFlag(Flags.Zero, value1 == value2);
        regs.SetFlag(Flags.Positive, value1 > value2);
        regs.SetFlag(Flags.Negative, value1 < value2);
    }
    
    private static void CompareSrc(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for cmp command.");
            return;
        }

        var value1 = regs.GetValue(args[0]);
        var value2 = regs.GetValue(args[1]);

        regs.SetFlag(Flags.Zero, value1 == value2);
        regs.SetFlag(Flags.Positive, value1 != null && value2 != null);
        regs.SetFlag(Flags.Negative,
            value1 != null && value2 != null && bool.TryParse(value1.ToString(), out var b1) && bool.TryParse(value2.ToString(), out var b2) && b1 && b2);
    }


    private static void Decrement(RegList regs, string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Error: Invalid number of arguments for dec command.");
            return;
        }

        var value = regs.GetAV(args[0]);
        regs.SetRegisterValue(args[0], value - 1);
    }

    private static void Random(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for rand.");
            return;
        }
        
        if (!int.TryParse(args[0], out var min))
        {
            min = regs.GetAV(args[0]);
        }
        if (!int.TryParse(args[1], out var max))
        {
            max = regs.GetAV(args[1]);
        }

        var value = System.Random.Shared.Next(min, max);
        regs["edi"].Value = "generated!";
        regs["edi"].AV = value;
    }

    private static void Increment(RegList regs, string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Error: Invalid number of arguments for inc command.");
            return;
        }

        var value = regs.GetAV(args[0]);
        regs.SetRegisterValue(args[0], value + 1);
    }

    private static void Loop(RegList regs, string[] args)
    {
        regs.SetRegisterValue("ecx", regs.GetAV("ecx")-1);
        if (regs.GetAV("ecx") <= 0)
            return;
        
        JumpUnconditional(regs, args); // jumps to the label
    }

    private static void Move(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for mov command.");
            return;
        }

        var sourceValue = regs.GetValue(args[1]);
        var av = regs.GetAV(args[1]);
        var flags = regs.GetFV(args[1]);
        var lv = regs.GetLV(args[1]);

        regs.SetRegisterValue(args[0], sourceValue);
        regs.SetRegisterFlags(args[0], flags | (int)Flags.Copied);
        regs.SetRegisterValue(args[0], av);
        regs.SetRegisterValue(args[0], lv);
    }

    private static void SetFlags(RegList regs, string[] args)
    {
        // set flags for a register
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for sf command.");
            return;
        }

        if (!int.TryParse(args[1], out var flags)) return;

        regs.SetRegisterFlags(args[0], flags);
    }

    private static void MoveSrc(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for mov command.");
            return;
        }

        var sourceValue = regs.GetValue(args[1]);
        regs.SetRegisterValue(args[0], sourceValue);
    }

    public static void Push(RegList regs, string[] args)
    {
        if (args.Length >= 1)
        {
            regs.PushStack(regs.GetAV(args[0]));
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for push command.");
        }
    }

    public static void Pop(RegList regs, string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Error: Invalid amounts of args for pop command.");
            return;
        }
        
        regs.SetRegisterValue(args[0], regs.PopStack());
    }
    public static void Show(RegList regs, string[] args)
    {
        if (!regs.TryGetValue(args[0], out var reg))
        {
            Console.WriteLine($"Error: Invalid RegName, '{args[0]}'");
            return;
        }
        
        Console.WriteLine(reg.ToString());
    }

    public static void GetRegs(RegList regs, string[] _)
    {
        foreach (var (regName, reg) in regs)
            Console.WriteLine(reg.ToString());
    }

    public static void Add(RegList regs, string[] @params)
    {
        if (@params.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for or");
            return;
        }

        if (!int.TryParse(@params[1], out var num))
        {
            num = regs.GetAV(@params[1]);
        }

        var regName = @params[0];
        var aV = regs.GetAV(regName);
        aV += num;

        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }

    public static void Sub(RegList regs, string[] @params)
    {
        if (@params.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for or");
            return;
        }

        if (!int.TryParse(@params[1], out var num))
        {
            num = regs.GetAV(@params[1]);
        }

        var regName = @params[0];
        var aV = regs.GetAV(regName);
        aV -= num;

        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }

    public static void Mul(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for or");
            return;
        }
        if (!int.TryParse(args[1], out var num))
        {
            num = regs.GetAV(args[1]);
        }

        var regName = args[0];
        var aV = regs.GetAV(regName);
        aV *= num;

        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }

    public static void Div(RegList regs, string[] @params)
    {
        if (@params.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for or");
            return;
        }

        if (!int.TryParse(@params[1], out var num))
        {
            num = regs.GetAV(@params[1]);
        }

        var regName = @params[0];
        var aV = regs.GetAV(regName);

        if (num == 0)
        {
            Console.WriteLine("Error: Division by zero is not allowed.");
            return;
        }

        aV /= num;

        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }

    public static void ShiftLeft(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for shl");
            return;
        }
        
        if (!int.TryParse(args[1], out var num))
        {
            num = regs.GetAV(args[1]);
        }

        var regName = args[0];
        var aV = regs.GetAV(regName);
        aV <<= num;
        
        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }
    
    public static void ShiftRight(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for shl");
            return;
        }
        
        if (!int.TryParse(args[1], out var num))
        {
            num = regs.GetAV(args[1]);
        }

        var regName = args[0];
        var aV = regs.GetAV(regName);
        aV >>= num;
        
        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }
    
    public static void Mod(RegList regs, string[] @params)
    {
        if (@params.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for or");
            return;
        }

        if (!int.TryParse(@params[1], out var num))
        {
            num = regs.GetAV(@params[1]);
        }

        var regName = @params[0];
        var aV = regs.GetAV(regName);

        if (num == 0)
        {
            Console.WriteLine("Error: Division by zero is not allowed.");
            return;
        }

        aV %= num;

        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }

    public static void And(RegList regs, string[] args)
    { 
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for and");
            return;
        }

        if (!int.TryParse(args[1], out var num))
        {
            num = regs.GetAV(args[1]);
        }

        var regName = args[0];
        var aV = regs.GetAV(regName);

        aV &= num;

        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }

    public static void Or(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for or");
            return;
        }

        if (!int.TryParse(args[1], out var num))
        {
            num = regs.GetAV(args[1]);
        }

        var regName = args[0];
        var aV = regs.GetAV(regName);

        aV |= num;

        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }

    public static void Xor(RegList regs, string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Error: Invalid number of arguments for xor");
            return;
        }

        if (!int.TryParse(args[1], out var num))
        {
            num = regs.GetAV(args[1]);
        }

        var regName = args[0];
        var aV = regs.GetAV(regName);

        aV ^= num;

        regs.SetRegisterValue(regName, aV);
        regs.SetFlag(Flags.Zero, aV == 0);
        regs.SetFlag(Flags.Positive, aV > 0);
        regs.SetFlag(Flags.Negative, aV < 0);
    }
    
    public static void JumpUnconditional(RegList regs, string[] args)
    {
        if (args.Length >= 1 && Labels.TryGetValue(args[0], out int targetLine))
        {
            regs.Eip = targetLine - 1; // Decrement by 1 to counter the increment in the loop
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for jmp command or label not found.");
        }
    }

    public static void JumpEqual(RegList regs, string[] args)
    {
        if (args.Length >= 1 && Labels.TryGetValue(args[0], out int targetLine))
        {
            if (regs.GetFlag(Flags.Zero))
            {
                regs.Eip = targetLine - 1; // Decrement by 1 to counter the increment in the loop
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for je command or label not found.");
        }
    }

    public static void JumpNotEqual(RegList regs, string[] args)
    {
        if (args.Length >= 1 && Labels.TryGetValue(args[0], out int targetLine))
        {
            if (!regs.GetFlag(Flags.Zero))
            {
                regs.Eip = targetLine - 1; // Decrement by 1 to counter the increment in the loop
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for jne command or label not found.");
        }
    }
    
    public static void JumpFail(RegList regs, string[] args)
    {
        if (args.Length >= 1 && Labels.TryGetValue(args[0], out int targetLine))
        {
            if (regs.GetFlag(Flags.Failure))
            {
                regs.Eip = targetLine - 1; // Decrement by 1 to counter the increment in the loop
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for jf command or label not found.");
        }
    }

    public static void JumpNotFail(RegList regs, string[] args)
    {
        if (args.Length >= 1 && Labels.TryGetValue(args[0], out int targetLine))
        {
            if (!regs.GetFlag(Flags.Failure))
            {
                regs.Eip = targetLine - 1; // Decrement by 1 to counter the increment in the loop
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for jnf command or label not found.");
        }
    }
    
    public static void JumpOverflow(RegList regs, string[] args)
    {
        if (args.Length >= 1 && Labels.TryGetValue(args[0], out int targetLine))
        {
            if (regs.GetFlag(Flags.Overflow))
            {
                regs.Eip = targetLine - 1; // Decrement by 1 to counter the increment in the loop
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for jf command or label not found.");
        }
    }
    
    public static void JumpGreater(RegList regs, string[] args)
    {
        if (args.Length >= 1 && Labels.TryGetValue(args[0], out int targetLine))
        {
            if (regs.GetFlag(Flags.Positive))
            {
                regs.Eip = targetLine - 1; // Decrement by 1 to counter the increment in the loop
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for jne command or label not found.");
        }
    }

    public static void JumpLess(RegList regs, string[] args)
    {
        if (args.Length >= 1 && Labels.TryGetValue(args[0], out int targetLine))
        {
            if (regs.GetFlag(Flags.Negative))
            {
                regs.Eip = targetLine - 1; // Decrement by 1 to counter the increment in the loop
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid argument for jne command or label not found.");
        }
    }

    public static void Call(RegList regs, string[] args)
    {
        if (args.Length < 1 || !Labels.TryGetValue(args[0], out int targetLine))
        {
            Console.WriteLine("Error: Invalid argument for call command or label not found.");
            return;
        }

        
        regs.PushStack(regs.Eip); // Push the current EIP onto the stack
        regs.Eip = targetLine - 1; // Jump to the target line (decremented by 1 to counter the increment in the loop)
    }

    public static void Return(RegList regs, string[] args)
    {
        if (regs.StackIsEmpty)
        {
            Console.WriteLine("------ Exited by calling ret");
            Environment.Exit(-1);
            return;
        }
        regs.Eip = regs.PopStack(); // Pop the return address from the stack and set EIP to return
    }
}