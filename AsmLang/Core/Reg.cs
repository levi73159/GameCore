namespace AsmLang.Core;

public class Reg
{
    public string Name { get; private set; }
    public int Id { get; }

    public int AV { get; set; } = 0;
    public int FV { get; set; } = 0;
    public long LV { get; set; }= 0;

    public object? Value { get; set; } = 0;

    public Reg(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public override string ToString()
    {
        return $"{Name}-{Id}: {Value} av={AV} fv={Convert.ToString(FV, 2).PadLeft(7, '0')} lv={LV}";
    }
}