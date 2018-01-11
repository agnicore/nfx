# JSONplus

JSONplus is **NFX’s** superset of JSON, it is a regular JSON grammar that understands the following extra things

* One-line comments

* Single-quoted strings

* Absence of quotes (when not needed)

* **NFX** writes a 100% compliant JSON.

`JSONWritingOptions` class allows for precise control of

* Date Format

* Row format: arrays vs maps

* NLS - Native Language Support. When enabled only emits string values in the desired culture with fallback to default language

* Pretty printing: tabifications and spacing

**NFX** JSON serializer automatically writes any `IEnumerable` as an array, IDictionary as a map and classes as maps (including anonymous classes). 
It also understands `IJSONWritable` interface that allows for custom entity representation in JSON.

**NFX** JSON de-serializer can saturate bcak `IJSONDataObject` (with auto-cast to `dynamic`) be it `JSONDataArray` or `JSONDataMap`. 
It also maps JSON back to Row instances using their schema. 
JSON deserializer does not support deserialization into arbitrary .NET types as this capability was never needed in any practical application. 
Keep in mind: in **NFX**, JSON is ONLY used for Web Interfaces and never internally as there are more efficient/convenient textual formats (i.e. LACONIC).
