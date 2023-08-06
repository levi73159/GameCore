namespace GameCore.Core;

public class RegList : Dictionary<string, Reg>
{
    private List<int> _stackList; // List to represent the stack
    public int[] Memory;
    public Dictionary<string, uint> MemoryLabels;

    public RegList()
    {
        Memory = Array.Empty<int>();
        MemoryLabels = new Dictionary<string, uint>();
        _stackList = new List<int>();
        Add("eax", new Reg("eax", 0));
        Add("ebx", new Reg("ebx", 1));
        Add("ecx", new Reg("ecx", 2));
        Add("edx", new Reg("edx", 3));
        Add("esi", new Reg("esi", 4));
        Add("edi", new Reg("edi", 5));
        Add("ebp", new Reg("ebp", 6));
        Add("esp", new Reg("esp", 7));
        Add("eip", new Reg("eip", 8));

        // Segment registers
        Add("cs", new Reg("cs", 9));
        Add("ds", new Reg("ds", 10));
        Add("es", new Reg("es", 11));
        Add("fs", new Reg("fs", 12));
        Add("gs", new Reg("gs", 13));
        Add("ss", new Reg("ss", 14));

        // Control registers
        Add("cr0", new Reg("cr0", 15));
        Add("cr1", new Reg("cr1", 16));
        Add("cr2", new Reg("cr2", 17));
        Add("cr3", new Reg("cr3", 18));
        Add("cr4", new Reg("cr4", 19));
        Add("cr5", new Reg("cr5", 20));
        Add("cr6", new Reg("cr6", 21));
        Add("cr7", new Reg("cr7", 22));

        // Debug registers
        Add("dr0", new Reg("dr0", 23));
        Add("dr1", new Reg("dr1", 24));
        Add("dr2", new Reg("dr2", 25));
        Add("dr3", new Reg("dr3", 26));
        Add("dr4", new Reg("dr4", 27));
        Add("dr5", new Reg("dr5", 28));
        Add("dr6", new Reg("dr6", 29));
        Add("dr7", new Reg("dr7", 30));
        

        // special registers
        Add(".flags", new Reg(".flags", 31));
        Add(".mem", new Reg(".mem", 32));
    }

    public int Eip
    {
        get => this["eip"].Av;
        set => this["eip"].Av = value;
    }

    public bool StackIsEmpty => _stackList.Count == 0;

    public void PushStack(int value)
    {
        _stackList.Add(value);
    }

    public int PopStack()
    {
        if (_stackList.Count == 0)
        {
            Console.WriteLine("Error: Stack is empty.");
            return 0; // Return a default value or throw an exception, depending on your requirements
        }

        int lastIndex = _stackList.Count - 1;
        int value = _stackList[lastIndex];
        _stackList.RemoveAt(lastIndex);
        return value;
    }

    public void SetRegisterValue(string regName, object? v)
    {
        if (TryGetValue(regName, out var regs))
            regs.Value = v;
        else
            Console.WriteLine($"Warning: Invalid register name, {regName}");
    }
    
    public void SetRegisterValue(string regName, int v)
    {
        if (TryGetValue(regName, out var reg))
            reg.Av = v;
        else
            Console.WriteLine($"Warning: Invalid register name, {regName}");
    }
    
    public void SetRegisterValue(string regName, long v)
    {
        if (TryGetValue(regName, out var reg)) 
            reg.Lv = v;
        else
            Console.WriteLine($"Warning: Invalid register name, {regName}");
    }

    public void SetRegisterFlags(string regName, int flags)
    {
        if (TryGetValue(regName, out var reg)) 
            reg.Fv = flags;
        else
            Console.WriteLine($"Warning: Invalid register name, {regName}");
    }

    public void SetFlag(Flags flag, bool value)
    {
        if (value)
            this[".flags"].Fv |= (int)flag;
        else
            this[".flags"].Fv &= ~(int)flag;
    }

    public bool GetFlag(Flags flag)
    {
        return (this[".flags"].Fv & (int)flag) != 0;
    }

    public object? GetValue(string regName) => TryGetValue(regName, out var reg) ? reg.Value : null;
    public int GetAv(string regName) => TryGetValue(regName, out var reg) ? reg.Av : -1;
    public int GetFv(string regName) => TryGetValue(regName, out var reg) ? reg.Fv : 0;
    public long GetLv(string regName) => TryGetValue(regName, out var reg) ? reg.Lv : -1;
}