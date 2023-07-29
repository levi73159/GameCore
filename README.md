# Game Core (.gc)

## Introduction

This document provides an overview of the features and commands available in *Game Core*. The language is designed to be simple and supports basic arithmetic operations, conditional jumps, and register manipulation.

## General Syntax

Game Core uses a simplified syntax for writing commands. Each command consists of a keyword followed by its arguments. Arguments are separated by spaces.

### Comments

Comments can be added to the code using the `;` symbol. Anything after the `;` on the same line will be treated as a comment and ignored during execution.

### Macros

Macros can be used by first defining a macro by using the `#` symbol then the name of the macro then the value like this `#name 30`.

***Note**: Macros can be named and have a value of anything.*
Macros can be accessed by using the `%` symbol and then the name of the macro like this `%name%` then it will be replaced with the value of the macro

```nasm
#PI Pie-314
writeln Pie is %PI%
```
### Importing files
You can import files using the `#import` directive, which will replace the import statement with the file text. It can be used in the following syntax:
```nasm
#import filename
```
where `filename` is the path to the file containing the code.

For example, consider a file named `functions.gc` with the following content:
```nasm
jmp functions_end

sayHello:
    writeln hello, world!
    ret

functions_end:
```
_The reason why we need to jump to the end of the file is because it will run everything in the file so it will say `hello, world!` write away if we don't_

To import the function into your code, you can use `#import` directive:
```nasm
#import functions.gc
call sayHello
```

## Registers

The language has a set of registers that can be used to store and manipulate data. Each register is identified by a unique name,
and it can store `AV` (actual value typeof int), `LV` (long value typeof long), `FV` (flags), and also a `SrcValue` which can be anything.

### Available Registers

The following registers are available:

### Basic Registers
- `eax`: General-purpose register for arithmetic operations.
- `ebx`: General-purpose register for arithmetic operations.
- `ecx`: General-purpose register for arithmetic operations.
- `edx`: General-purpose register for arithmetic operations.
- `esi`: General-purpose register for storing data.
- `edi`: General-purpose register for storing data that is mostly use for functions returns.
- `esp`: Stack pointer register.
- `eip`: Instruction pointer register (used for control flow).

### Segment Registers
- `cs`: unused
- `ds`: unused
- `es`: unused
- `fs`: unused
- `gs`: unused
- `ss`: unused

### Control Registers (used for input)
- `cr0`: default register for `readline`
- `cr1`: default register for `getkey`
- `cr2`: unused
- `cr3`: unused
- `cr4`: unused
- `cr5`: unused
- `cr6`: unused
- `cr7`: unused

### debug registers
- `dr0`: used for os info
  - `SRC`: This will store the version
  - `AV`: 2 for Windows NT or later, 4 for Unix, 7 for Other
- `dr1`: unused
- `dr2`: unused
- `dr3`: unused
- `dr4`: unused
- `dr5`: unused
- `dr6`: unused
- `dr7`: used as a exit code

### special registers (all special registers begin with a '.')
- `.flags`: the global flags register

## Commands

The language supports various commands that perform arithmetic operations, control flow, and register manipulation. Here are the available commands:

### Arithmetic Commands (stores everything in the AV)

- `add dest value`: Adds the `value` to the value stored in the `dest` register.
- `sub dest value`: Subtracts the `value` from the value stored in the `dest` register.
- `mul dest value`: Multiplies the value in the `dest` register by the `value`.
- `div dest value`: Divides the value in the `dest` register by the `value`.
- `mod dest value`: Computes the modulus of the value in the `dest` register by the `value`.
- `and dest value`: Performs bitwise AND between the value in the `dest` register and the `value`.
- `or dest value`: Performs bitwise OR between the value in the `dest` register and the `value`.
- `xor dest value`: Performs bitwise XOR between the value in the `dest` register and the `value`.
- `inc dest`: Adds 1 to the `dest` register.
- `dec dest`: Subtracts 1 from the `dest` register.
- `shl reg value`: Shifts `reg` to the left by `value` *(`value` can also be a register)*
- `shr reg value`: Shifts `reg` to the right by `value` *(`value` can also be a register)*
- `cmp regA regB`: Compares `regA's` `AV` with `regB's` `AV`, if `regA` != `regB` then `Zero` flag is set, if `regA` > `regB` then `Positive` flag is set, if `regA` < `regB` then `Negative` flag is set
- `cmpSrc regA regB`: Compares `regA's` `SrcValue` with `regB's` `SrcValue`, if `regA` != `regB` then `Zero` flag is set, if `regA` and `regB` are not null then `Positive` flag is set, if `regA` and `regB` are both equal to true then `Negative` flag is set

### Data Manipulation Commands

- `set reg value`: Sets the `reg` `SrcValue` register to the specified `value`.
- `mov dest src`: Copies everything from the `src` register to the `dest` register and set the `Copied` flag on `dest`.
- `movSrc dest src`: Copies the `src` register `SrcValue` to the `dest` register but do not set the `Copied` flag.
- `setAV reg value`: Sets the `reg` register's `AV` to the specified `value`.
- `setLV reg value`: Sets the `reg` register's `LV` to the specified `value`.
- `sf reg value`: Set the `reg` register `FV` to the specified `value`.
- `convertAV reg`: converts the `SrcValue` register into `reg's` `AV` slot, when fails it will set the `Failure` flag.

### Control Flow Commands

- `jmp label`: Unconditionally jumps to the specified `label`.
- `jz label`: Jumps to the specified `label` if the `Zero` flag is set
- `jnz label`: Jumps to the specified `label` if the `Zero` flag is not set
- `jg label`: Jumps to the specified `label` if the `Positive` flag is set 
- `jl label`: Jumps to the specified `label` if the `Negative` flag is set
- `jf label`: Jumps to the specified `label` if the `Failure` flag is set
- `jnf label`: Jumps to the specified `label` if the `Failure` flag is not set

### Stack Manipulation Commands

- `push reg`: Pushes the specified registers's AV onto the stack.
- `pop reg`: Pops the top value from the stack and stores it in the `reg` register.

### Input/Output Commands

- `writeSrc reg`: Displays the `SrcValue` of the specified register.
- `writeAV reg`: Displays the `AV` of the specified register.
- `writeLV reg`: Displays the `LV` of the specified register.
- `writeln text`: Displays the specified text on a new line (quotes are not required).
- `write text`: Displays the specified text (quotes are not required).
- `newline`: Add a new line
- `readline (optional: reg)`: Reads a line from the console and puts it into the `SrcValue` of `reg` or `cr0` by default.
- `getkey (optional: reg)`: Reads a key press from the console and puts the keychar into `SrcValue` and keyCode into `AV` and the Modifiers into the `FV` of `reg` or `cr1` by default 
- `beep (optinal: frequency, duration)`: *windows only* makes a beep sound in the console 

### Special Commands

- `call label`: Calls the specified `label` _(function)_ and saves the return address on the stack.
- `ret`: Returns from a function by popping the return address from the stack.
- `exit`: Exits the program with the value stored in the `dr7` register.
- `wait ms`: Waits for a certain amount of milliseconds
- `rand min max`: Generate a random number between `min` and `max`, and put the number in `edi` register
- `getOS`: this function gets the os info and stores it into the `dr0` register, *by default this calls at the start of the program*

## Labels

Labels are used to mark specific locations in the code, allowing for easy control flow using jump commands. A label is any valid identifier followed by a colon `:`. Labels must start in the first column of the line.

## Flags

- Bit 0 represents the `Zero` flag
- Bit 1 represents the `Positive` flag
- Bit 2 represents the `Negative` flag
- ~~Bit 3 represents the `Carry` flag~~
- ~~Bit 4 represents the `Overflow` flag~~
- Bit 5 represents the `Copied` flag
- Bit 6 represents the `Failure` flag

### Input/Output Flags
- Bit 0 represents the `Alt` flag
- Bit 1 represents the `Shift` flag
- Bit 2 represents the `Control` flag

# Examples

Here are some examples of code written in the assembly-like language:

1. Calculating the factorial of 5 and displaying the result:

```nasm
setAV eax 5
setAV ebx 1

.loop:
    mul eax ebx
    dec eax
    cmp eax ecx
    jz .done
    jmp .loop

.done:
    mov ebx eax
    writeAV eax
```

2. Simple function call example:

```nasm
call .myFunction
writeln Returned from function call

.myFunction:
    writeln Inside function
    ret
```


## Conclusion

Game Core provides a basic set of commands for arithmetic operations, control flow, and register manipulation. It is designed to be simple and easy to understand, making it suitable for educational purposes or small projects. However, it lacks more advanced features and optimizations commonly found in real-world assembly languages.