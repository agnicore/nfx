# Navigation

To get an access to some node in your configuration tree you can use two ways:

1. Using DOM. This way can be used in application only and supposes the using of next methods of `NFX.Environment.ConfigNode`:
indexer `[subsection_name]` - for access to subsection,
`AttrByName(name_of_attribute)` or `AttrByIndex(index_of_attribute)` - for access to attribute.

2. Paths. This way can be use directly in the configuration. Use:
`/` - as leading char for root,
`..` - for step up,
`$` - for attribute name,
`[int]` - for access to subsection or attribute by index,
`section[value]` - for access using value comparison of named section,
`section[attr=value]` - for access using value of sections named attr.

Multiple paths may be coalesced using `|` or `;`. 
If path starts from `!` then exception will be thrown if such node does not exist. 
In application you can use `NFX.Environment.ConfigNode.Navigate(path)` which navigates the path and return the appropriate node.
To return path from the root to the node `NFX.Environment.ConfigNode.RootPath()` is used.

Let `confStr` string variable contains the following configuration:

```js
root=person
{
  name="Scott Freeman" {}
  age=43

  contacts
  {
    e-mail=sf@mail.net
    phone=mobile { value=893457218902 }
  }

  income
  {
    type=salary { value=1000 }
    type=bonus { value=100 }
  }
}
```

The code

```cs
var root = confStr.AsLaconicConfig();
var result = root.Navigate(path).Value;
```

will output navigation result depending on `path` variable value:

1. /name - "Scott Freeman"

2. /$age - "43"

3. /contacts/$e-mail - "sf@mail.net"

4. /contacts/phone - "mobile"

5. /contacts/phone/$value - "893457218902"

6. /contacts/[0] - "mobile"

7. /contacts/phone/$[0] - "893457218902"

8. /income/type[salary]/$value - "1000"

9. /income/type[value=100] - "bonus"