# C# CLR JIT Optimization - Register allocation for primitives

Today I have come across a strange effect in C#

This code took 30 ms to run, and C++ version took 5-8msec. How come?
```cs
void Runtest()
{
  const int CNT = 10000000; //ten million
  long sum = 0;

  var w = Stopwatch.StartNew();

  for(int i=0; i < CNT, i++)
  {
    sum += 1;
  }
  w.Stop();

  var rate = CNT / (w.Elapsed.TotalSeconds);

  Text = string.Format("Total: {0} in {1} ms  at {2}/sec ", CNT, 
                       w.Elapsed.TotalMilliseconds, rate);

  Text += sum.ToString();
}
```

The loop body does not do anything, and hopefully CLR JITs it into registers right?

If yes, why is it so many times slower than C++?

The answer lies with the last line:  "s.ToString();".  It treats "sum" as a struct, because structs are not really primitive types, although "long" fits in the register 1:1,
 the "objecty" nature of the "sum.ToString()" makes JIT generate memory-accessing code.I have confirmed this in WinDBG. The loop control var "i" is stored in the register but "sum" is not.

Solution:
```cs
var s = sum;
Text += s.ToString();
```

Result: **2 msec per 10,000,000 iterations instead of 30 ms. 15 times faster.**

Here I have put an upper loop bound to Int.MaxValue for the code to run a bit longer so I can manage to ctrl+break in debugger.

Disassembly (loop body - lines 4-7):
```no-highlight
1: 000007ff`0014055d 33c0            xor     eax,eax           //i=0
2: 000007ff`0014055f 33ed            xor     ebp,ebp           //sum=0
3: LOOP LABEL:
4: 000007ff`00140561 48ffc5          inc     rbp               //sum++   
5: 000007ff`00140564 ffc0            inc     eax               //i++   
6: 000007ff`00140566 3dffffff7f      cmp     eax,7FFFFFFFh     //check int.Max  
7: 000007ff`0014056b 7cf4            jl      000007ff`00140561 //jmp if less LOOP LABEL can not beat that!
```

The bottom line is this: JIT does optimize primitives in CPU registers just like C++ does, however because of unified type system it is easy to treat a primitive as struct or object - and that introduces penalty.  
The truth of the matter is that until the loop exit they (Microsoft) could have kept it in register and then moved to RAM, but there are many edge cases like this and who knows what other issues JIT developers had to solve.

---
Dmitriy Khmaladze  
May 14, 2013