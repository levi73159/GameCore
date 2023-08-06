namespace GameCore.Core;

[Flags]
public enum Flags
{
    Zero      = 0b0000001,       // Bit 0 represents Zero flag
    Positive  = 0b0000010,   // Bit 1 represents Positive flag
    Negative  = 0b0000100,   // Bit 2 represents Negative flag
    Carry     = 0b0001000,      // Bit 3 represents Carry flag
    Overflow  = 0b0010000,   // Bit 4 represents Overflow flag
    Copied    = 0b0100000,
    Failure   = 0b1000000,
}
