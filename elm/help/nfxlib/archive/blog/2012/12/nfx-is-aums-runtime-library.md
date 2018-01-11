# NFX is Aum's Runtime Library

## What is NFX?

NFX stands for .NET Framework Extension. It is written in C# and uses only **very-BCL** types (such as: IEnumerable, List, Dictionary, string, DateTime). I have explained this in the prior post here. The purpose of NFX is to provide UNISTACK - unified stack of libraries/software for solving real application programming tasks. There is also JFX which is very similar to NFX and I used it for building some Android apps. But NFX is much more than that.


## What is Aum?

Aum is the multi-paradigm programming language which has been an internal research project at IT Adapter since 2004. It is somewhat similar to modern C# in its feature set adding aspect-oriented-programming, message passing and pattern matching (a la ObjectiveC, Delphi and Erlang). This post is definitely not the right place for Aum language description, so I'll include just a few lines.

Why new language? - because, in my opinion, there are no alternatives. Java is very sluggish in terms of feature adoption (where are lambdas in 2012?), yet C# is very good but really community must obey Microsoft. Mono is trying to add additional features, but let's be honest - how many projects are Mono-primary (vs VS/Win-Primary). Other languages are not general purpose enough i.e. Erlang was never designed to handle fast math, and even basic integer arithmetic is a few times slower than in a C app. See this perf test: [Parallel integer summation challenge Erlang vs CLR/C#](/archive/blog/2013/05/parallel-integer-summation-challenge.html).

After Microsoft added lambda functions and closures, C# code started to look much more functional, and honestly after Parallel Task Library addition many Erlang benefits started to vanish.

Yes! C++ is very fast but it is not a modern language. Very easy to abuse, very hard to read, and still no way to do even simplest form of reflection converting string->type instance. I remember, back in 1995 when Delphi 1 came around, we were really excited to see things that allowed us to get method pointer from it's name. Not many .NET developer realize that much of .NET convenience actually came from Delphi RTL/VCL.

Coming back to Aum language - the main idea is to build everything around AST. There is no intermediate code a-la Java bytecode or CIL or Dalvik code. Aum modules (compiled assemblies .dll/.class files analogue) are generically serialized abstract-syntax-trees. Aum supports aspect oriented programming with AST pattern matching so aspects may be injected in "compiled" code.

Currently we have constructed lexer,parser and most of semantic analyzer. The first code gen is basically an AST-walking interpreter. We have plans to keep working on this project and start using LLVM for machine code JIT compilation. One of the compilation targets is native executable (no VM) just like with C/C++.


## What does Aum Language have to do with NFX?

I stopped investing much time in Aum language compiler back in 2008 because I realized something else. A language even a fast one, with a good compiler does not mean anything without organic library support. Since Aum is somewhat similar to C# in it's features, I decided to invest more into NFX.

NFX is built around .NET CLR, but it is **not a typical .NET framework at all!** NFX is really a run-time/base class library for Aum language. We just decided to spend more time now building real applications that use NFX (since CLR is kindly written by Microsoft).

---
Dmitriy Khmaladze  
December 10, 2012