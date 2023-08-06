namespace GameCore.Core;

public class Reg
{
    public string Name { get; private set; }
    public int Id { get; }

    public int Av { get; set; } = 0;
    public int Fv { get; set; } = 0;
    public long Lv { get; set; }= 0;

    public object? Value { get; set; } = 0;

    public Reg(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public override string ToString()
    {
        return $"{Name}-{Id}: {Value} av={Av} fv={Convert.ToString(Fv, 2).PadLeft(7, '0')} lv={Lv}";
    }
}