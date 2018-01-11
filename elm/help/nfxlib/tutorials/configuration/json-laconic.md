# JSON-Laconic Conversion

JSON and Laconic formats are completely 
You can easily convert laconic configuration to JSON format and backwards by using methods `NFX.Environment.ConfigNode.ToJSONDataMap()` and `NFX.Serialization.JSON.JSONDataMap.ToConfigNode()`.

Suppose string variable `confStr` contains the following laconic configuration section:

```js
root
{ 
  name="David Crocket"
  contacts
  { 
    address
    {
      zipcode=10010
      city="New York"
    } 
    phone { mobile=891245783968 } 
  }
}
```

After applying of the code

```cs
var node = confStr.AsLaconicConfig();
var map = node.ToJSONDataMap();
var result = map.ToJSON(JSONWritingOptions.PrettyPrint);
```

the `result` variable will conatin JSON representation of initial laconic configuration:

```json
{
  "name": "David Crocket", 
  "contacts": 
  {
    "address": 
    {
      "zipcode": "10010", 
      "city": "New York"
    }, 
    "phone": 
    {
      "mobile": "891245783968"
    }
  }
}
```

The code

```cs
var map = (JSONDataMap)jsonStr.JSONToDataObject();
var node = map.ToConfigNode();
var result = node.ToLaconicString();
```

will do the opposite task.