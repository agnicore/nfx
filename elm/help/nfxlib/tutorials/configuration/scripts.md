# Scripts

You can make script inserts in your configuration. 
To execute configuration script which contains in `configStr` string variable, apply the following code:

```cs
var src = LaconicConfiguration.CreateFromString(configStr);
var conf = new LaconicConfiguration();
var runner = new ScriptRunner();
runner.TimeoutMs = 500; // for slow computers may be increased
runner.Execute(src, conf);
var result = conf.SaveToString();
```

## Loops

One can easily implement a simple loop in a configuration script.
The script

```css
root
{
  i=3
  _loop='$(/$i) < 5'
  {
    _set{ path=/$i to=$(/$i)+1 }
    sectionLoop
    {
      name=section_$(/$i)
      value='something'
    }
  }
}
```

results in

```css
root
{
  i=3
  sectionLoop
  {
    name=section_4
    value=something
  }
  sectionLoop
  {
    name=section_5
    value=something
  }
}
```

## Functions

Named fragment of script, may be called like a function. The following config script:

```css
root
{
  // named script fragment
  sub_Loop
  {
    script-only=true
    cnt=0 { script-only=true }
    _loop='$(../cnt) < 2'
    {
      _set{ path=/sub_Loop/cnt to=$(/sub_Loop/cnt)+1 }
      _if='$(/sub_Loop/cnt)==1'
      {
        fromSubLoopFOR_ONE
        {
          name=section_$(/sub_Loop/cnt)
          value='1 gets special handling'
        }
      }
      _else
      {
        fromSubLoop
        {
          name=section_$(/sub_Loop/cnt)
          value='something'
        }
      }
    }
  }

  // function call
  _call=/sub_Loop {}
}
```

results in

```css
root
{
  fromSubLoopFOR_ONE
  {
    name=section_1
    value="1 gets special handling"
  }
  fromSubLoop
  {
    name=section_2
    value=something
  }
}
```

## If Statements 

One can easily inplement "If" statements using declared variables. The following config script:

```css
root
{
  // variables are using below in script
  var1=0 { script-only=true }
  var2=175.4 { script-only=true }
  var3=true { script-only=true }
   
  // ternary if
  _block
  {
    _set{ path=/var1 to=(?$(/var2)>10;15;-10)+100 }
    RESULT1=$(/var1) {}
  }

  // ternary if with mixing types
  _block
  {
    _set{ path=/var1 to='((?$(/var3);$(/var2);-10)+100)+test' }
    RESULT2=$(/var1) {}
  }
}
```

results in

```css
root
{
  RESULT1=115
  {
  }
  RESULT2=275.4test
  {
  }
}
```

## Section Generation

It is possible to create dynamic sections within a loop over integer or even real index.
The following script

```css
root
{
  i=3
 
  // variables are using below in script
  var1=0 { script-only=true }
  var2=175.4 { script-only=true }
  var3=true { script-only=true }
   
   // naming section with variable
  _set{ path=/$i to=0 }
  _loop='$(/$i) < 2'
  {
    section_$(/$i) { value='something' }
    _set { path=/$i to=$(/$i)+1 }
  }
   
  // loop with real arithmetic
  _set{ path=/$i to=0.1 }
  _loop='$(/$i)<=0.3'
  {
    section_$(/$i) {}
    _set{ path=/$i to=$(/$i)+0.1 }
  }
}
```

results in

```css
root
{
  i=3
  section_0
  {
    value=something
  }
  section_1
  {
    value=something
  }
  section_0.1
  {
  }
  section_0.2
  {
  }
  section_0.3
  {
  }
}
```