# Parallel integer summation challenge Erlang vs CLR/C#

Today we have done an interesting benchmark in C# and Erlang. Basically the point was to compare the brevity and performance of two inherently different langs/approaches:

Erlang r16b:
```erlang
$ cat t.erl

-module(t).
-export([t/0, t/1, ct/1, ct/2]).

-define(DEF_CNT, 10000000).

t() ->
    t(?DEF_CNT).
t(N) when is_integer(N) ->
    time_it(fun() -> test(N) end, N).

ct(Threads) ->
    ct(?DEF_CNT, Threads).
ct(N, Threads) when is_integer(N), is_integer(Threads) ->
    time_it(fun() -> ctest(N, Threads) end, N * Threads).

time_it(Fun, Divisor) ->
    {T, Result} = timer:tc(Fun),
    {T / (Divisor/1000000) / 1000, Result}.

ctest(N, M) ->
    Pids = spawner(self(), N, M),
    lists:foldl(fun(Pid, S) ->
        receive {Pid, Sum} -> S+Sum end
    end, 0, Pids).

spawner(_, _, 0) ->
    [];
spawner(Owner, N, M) ->
    [spawn(fun() -> loop(Owner, N) end) | spawner(Owner, N, M-1)].

loop(Pid, N) ->
    Sum = test(N),
    Pid ! {self(), Sum}.

test(N) -> test(N, 0).

test(0, M) -> M;
test(N, M) -> test(N-1, M+1).
```

C#, NET 4:
```cs
private void button4_Click(object sender, EventArgs e)
{
  const int CNT = 100000000;
  const int SPLIT = 1000000;

  var tasks = new Task<long>[CNT / SPLIT];

  var w = Stopwatch.StartNew();
           
  for(int i=0,start=0; i < CNT / SPLIT; i++)
  { 
    tasks[i] = new Task<long>( () =>
                          {
                            long lsum = 0;
                            long end = start + SPLIT;
                            for(int c=start; c<end; c++)
                              lsum++;
                            return lsum;
                          });
    tasks[i].Start();
  }

  long sum = tasks.Sum( t => t.Result);
          
  w.Stop();

  var rate = CNT / (w.Elapsed.TotalSeconds);

  Text =
    string.Format("Total: {0} in {1} ms  at {2}/sec  {3} msec/million ",
                  CNT, w.Elapsed.TotalMilliseconds, rate, w.Elapsed.TotalMilliseconds / (CNT / 1000000));

  Text += sum.ToString();
}
```

Test results are really interesting:
* .NET      0.22 msec per million
* Erlang does this is 1.5 msec per million

**CLR is 6.8 faster for this test.**

Some other facts.  Linear loop single threaded that sums integers (not optimized out of code, I confirmed in disassembler)
PHP code:
```php
$CNT = 10000000;
$sum = 0;
$w = microtime(true);
for($i=0; $i<$CNT; $i++) $sum += 1;
$w = round((microtime(true)-$w)*1000,0);
echo($w);
```
```no-highlight
PHP standard runtime:  1040ms 
C#:                    2ms
```

**CLR is 500 times faster that PHP for this simple test**

---
Dmitriy Khmaladze  
May 15, 2013
