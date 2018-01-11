# Evaluation of Variables

In your configurations you can use variables which can take values of configuration tree nodes. 
To get the value of some node construction `$(path_to_the_node)` is used (for more details about paths see [Navigation](./navigation.md) section). 
Besides that you can work with environment variables `(~name_of_variable)` and you can even make your own variables resolver. 
Also to expand the functionality of variables macro inserts `(::macro_name)` can be added.

Let `confStr` string variable contains the following configuration section:

```js
root
{
  name="Jake"
  last_name="Snowman"
  full_name="$(/$name) $(/$last_name)" {}
  DOB="1982/05/12"
   
  details
  {
    doc1=c:\documents\
    doc2=\snowman

    var1=$(../var2) {}
    var2=$(../var1) {}

    env_var=$(~USERNAME) // "~" - environment variable
    nowis=$(::now)       // "::" - macro
  }
}
```

The code

```cs
var conf = LaconicConfiguration.CreateFromString(confStr);
var result = pattern.EvaluateVarsInConfigScope(conf);
```

will output navigation result depending on `pattern` variable value:

1. $(/$name) - "Jake"

2. $(/full_name) - "Jake Snowman"

3. $(/details/$doc1)$(/details/$doc2) - "c:\documents\\snowman"

4. $(/details/var1) - endless loop (throws exception)

5. $(/non-existent|/$name) - "Jake"

6. $(!/non-existent) - nonexisting required (throws exception)

7. $(/details/$env_var) - "User"

8. $(/details/$nowis) - current datetime

9. $(/just_name::as-string dflt="Anderson") - "Anderson"