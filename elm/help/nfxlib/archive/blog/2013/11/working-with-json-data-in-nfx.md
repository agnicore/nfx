# Working with JSON data in NFX

## JSON support in NFX
There is no life this day and age without JSON support. If you are a seasoned developer you know better - JSON serialization is a pain. There are many options for it but there is always something that either does not work right (i.e. dates are not serialized per ISO standard), or slow, or both.

As I have already mentioned [here, NFX is a "Unistack" concept, meaning](/archive/blog/2013/08/what-is-nfx-what-is-unistack.html "What is NFX? What is Unistack?") - it has to have all vital functions natively integrated in it's core. JSON support is certainly qualified as such. We can not afford to spend time figuring out why some structure does not serialize the way Twitter or some other service expects. It has to be small, and simple. It has to perform very well.

## How is JSON Support Implemented in NFX
I wrote JSON lexer and parser in less than 8 hrs, that includes Unicode escapes, intricate string escapes and much more. I also managed to write around 50 unit tests the next day. The reason why I was able to write this so fast is because NFX has a nice "CodeAnalysis" concept that provides abstract support for writing language analysis tools/compilers. This certainly deserves it's own blog post, but I'll just say that low-level mechanisms of tokenezation, source code management (read from buffer, from file), compiler pipeline contexts (Lexer->Parser->Semantics->Code Generator), warnings/errors, source positioning, and other things are already there, so I just needed to add JSON support.

Here are a few JSON lexer unit tests:
```cs
[TestCase]
public void TokenClassifications()
{
  var src = @"a 'string' : 12 //comment";

  var tokens = new JL(new StringSource(src)).Tokens;

  Assert.IsTrue(tokens[0].IsBOF);
  Assert.IsTrue(tokens[0].IsNonLanguage);
  Assert.IsFalse(tokens[0].IsPrimary);
  Assert.AreEqual(TokenKind.BOF, tokens[0].Kind);

  Assert.AreEqual(JSONTokenType.tIdentifier, tokens[1].Type);
  Assert.IsFalse(tokens[1].IsNonLanguage);
  Assert.IsTrue(tokens[1].IsPrimary);
  Assert.AreEqual(TokenKind.Identifier, tokens[1].Kind);

  Assert.AreEqual(JSONTokenType.tStringLiteral, tokens[2].Type);
  Assert.IsFalse(tokens[2].IsNonLanguage);
  Assert.IsTrue(tokens[2].IsPrimary);
  Assert.IsTrue(tokens[2].IsTextualLiteral);
  Assert.AreEqual(TokenKind.Literal, tokens[2].Kind);

  Assert.AreEqual(JSONTokenType.tColon, tokens[3].Type);
  Assert.IsFalse(tokens[3].IsNonLanguage);
  Assert.IsTrue(tokens[3].IsPrimary);
  Assert.IsTrue(tokens[3].IsOperator);
  Assert.AreEqual(TokenKind.Operator, tokens[3].Kind);

  Assert.AreEqual(JSONTokenType.tIntLiteral, tokens[4].Type);
  Assert.IsFalse(tokens[4].IsNonLanguage);
  Assert.IsTrue(tokens[4].IsPrimary);
  Assert.IsTrue(tokens[4].IsNumericLiteral);
  Assert.AreEqual(TokenKind.Literal, tokens[4].Kind);

  Assert.AreEqual(JSONTokenType.tComment, tokens[5].Type);
  Assert.IsFalse(tokens[5].IsNonLanguage);
  Assert.IsFalse(tokens[5].IsPrimary);
  Assert.IsTrue(tokens[5].IsComment);
  Assert.AreEqual(TokenKind.Comment, tokens[5].Kind);
}
 
[TestCase]
public void BasicTokens2()
{
  var src = @"{a: 2, b: true, c: false, d: null, e: ['a','b','c']}";

  var lxr = new JL(new StringSource(src));

  var expected = new JSONTokenType[]
  { 
    JSONTokenType.tBOF, JSONTokenType.tBraceOpen,
    JSONTokenType.tIdentifier, JSONTokenType.tColon,
    JSONTokenType.tIntLiteral, JSONTokenType.tComma,
    JSONTokenType.tIdentifier, JSONTokenType.tColon,
    JSONTokenType.tTrue, JSONTokenType.tComma,
    JSONTokenType.tIdentifier, JSONTokenType.tColon,
    JSONTokenType.tFalse, JSONTokenType.tComma,
    JSONTokenType.tIdentifier, JSONTokenType.tColon,
    JSONTokenType.tNull, JSONTokenType.tComma,
    JSONTokenType.tIdentifier, JSONTokenType.tColon,
    JSONTokenType.tSqBracketOpen,
    JSONTokenType.tStringLiteral, JSONTokenType.tComma,
    JSONTokenType.tStringLiteral, JSONTokenType.tComma,
    JSONTokenType.tStringLiteral, JSONTokenType.tSqBracketClose,
    JSONTokenType.tBraceClose, JSONTokenType.tEOF
  };
    
  Assert.IsTrue( lxr.Select(t => t.Type).SequenceEqual(expected) );
}
 
[TestCase]
public void IntLiterals()
{
  Assert.AreEqual(12, new JL(new StringSource(@"12")).Tokens.First(t=>t.IsPrimary).Value);
  Assert.AreEqual(2,  new JL(new StringSource(@"0b10")).Tokens.First(t=>t.IsPrimary).Value);
  Assert.AreEqual(16, new JL(new StringSource(@"0x10")).Tokens.First(t=>t.IsPrimary).Value);
  Assert.AreEqual(8,  new JL(new StringSource(@"0o10")).Tokens.First(t=>t.IsPrimary).Value);
}
```
Did you see something weird? YES, this is not JSON, this is **superset of JSON!**

## JSON+ (is) a JSON on Steroids (or vodka)
NFX supports reading of JSON superset. It naturally happened so that my JSON parser was built per NFX.CodeAnalysis namespace, so I got support for the following things for free:
* Single line comments
* Multiline comment blocks
* Compiler directives
* Verbatim strings
* Hex, Bin, Octal prefixes in integer literals

Instead of removing those features because JSON does not support them, I decided to leave them as-is, so now I can use JSON+(that's how I call NFX.JSON superset) for other things. For example:
```cs
[TestCase]
public void ParallelDeserializationOfManyComplexObjects()
{
  const int TOTAL = 1000000;
  var src =
@"
 {FirstName: ""Oleg"",  //comments dont hurt
  'LastName': ""Ogurtsov"",
  ""Middle Name"": 'V.',
  ""Crazy\nName"": 'Shamanov',
  LuckyNumbers: [4,5,6,7,8,9], 
  /* comments
  do not break stuff */
  |* in this JSON superset *|
  History: 
  [
    #HOT_TOPIC    //ability to use directive pragmas
    {Date: '05/14/1905', What: 'Tsushima'},
    #MODERN_TOPIC
    {Date: '09/01/1939', What: 'WW2 Started', Who: ['Germany','USSR', 'USA', 'Japan', 'Italy', 'Others']}
  ] ,
  Note:
$'This note text
can span many lines
and
this \r\n is not escape'
}";

  var watch = Stopwatch.StartNew();
      
  System.Threading.Tasks.Parallel.For
  (0, TOTAL,
    (i)=>
    {
      var obj = src.JSONToDynamic();
      Assert.AreEqual("Oleg", obj.FirstName);
      Assert.AreEqual("Ogurtsov", obj.LastName);
      Assert.AreEqual("V.", obj["Middle Name"]);
      Assert.AreEqual("Shamanov", obj["Crazy\nName"]);
      Assert.AreEqual(6, obj.LuckyNumbers.Count);
      Assert.AreEqual(6, obj.LuckyNumbers.List.Count);
      Assert.AreEqual(7, obj.LuckyNumbers[3]);
      Assert.AreEqual("USSR", obj.History[1].Who[1]);
    }
  );
 
  var time = watch.ElapsedMilliseconds;
  Console.WriteLine("Long JSON->dynamic deserialization test took {0}ms for {1} objects @ {2}op/sec"
         .Args(time, TOTAL, TOTAL / (time / 1000d)));
}
```
This approach is 100% compatible with "regular" JSON as "regular" JSON does not have comments and verbatim strings. The only "dangling" feature is compiler pragmas that I left there - they are just ignored for now and we may use them for something else in future. The bottom line is, that I spent <20 hours writing lexer, parser and around 90 unit tests in total. The tests are hand-written and test many edge cases like: comment within a string or string inside comment, Unicode escapes etc.

## JSON Pattern Matching
I guess you already figured that we do know about benefits of programming in functional languages (such as Erlang) and message oriented systems. Since our JSON support is based on NFX.CodeAnalysis, we have a feature right out of the box - pattern matching. Pattern matching is really cool because we can use it to quickly filter/reject JSON messages that we do/do not need. Let look at the code:
```cs
// mathc a person - a message with "Last name"
public class JSONPersonMatchAttribute : JSONPatternMatchAttribute
{
  public override bool Match(NFX.CodeAnalysis.JSON.JSONLexer content)
  {
    return content.LazyFSM(
      (s,t) => s.LoopUntilMatch(
       (ss, tk) => tk.LoopUntilAny("First-Name","FirstName","first_name"),
       (ss, tk) => tk.IsAnyOrAbort(JSONTokenType.tColon),
       (ss, tk) => tk.IsAnyOrAbort(JSONTokenType.tStringLiteral),
       (ss, tk) => FSMI.TakeAndComplete
             ),
      (s,t) => FSMI.Take
    ) != null;  
  }
}
```
And now, we can quickly filter by doing this:
```cs
[JSONPersonMatch] //<---- OUR MATCHER!
[TestCase]
public void JSONPatternMatchAttribute3()
{
  var src = @"{ code: 1121982, color: red, 'first_name': 'Alex', DOB: null}";
  var lxr = new JL(new StringSource(src));
  var match = JSONPatternMatchAttribute.Check(MethodBase.GetCurrentMethod(), lxr);          
  Assert.IsTrue( match );
}
```
The filter statement above is an example of **imperative filter**. It is a Finate State Machine that gets fed from lexically-analyzed JSON stream. What makes it very fast, is the fact that **JSON lexer is a lazy one** - it parses input only when parser asks for the next token. Suppose we need to parse a message that has 64 kbytes of JSON content. Why would a lexer need to parse all 64 kbytes if our particular business code can only process JSON message that has some certain structure? So, the way it is implemented now, as soon as pattern match fails - there is no need to keep parsing to end. Again, this is not a JSON-specific concept in NFX, rather a general NFX.CodeAnalysis concept that applies to other parsers (C#, Laconic, RelationalScema, etc.)

One more thing about parsing, if you have noticed - I used a pattern attribute on a method declaration. It is purposely done for message-processing cases (i.e. web MVC applications), where method signature may take some JSON data as input and pattern match attribute will guard the method this way. Sounds like Erlang or Delphi message, or better yet ObjC to anyone?

As far as pattern matching is concerned - people ask me "why do you not use regular expressions?". Simple - because, we do use the similar approach with FSM(Finate State Machine) but we analyse tokens, not characters, so our matches are 100% correct in terms of language grammar, whereas RegExp has no clue about tokens as it works on strings. The feature that we do want to add in future though, is an ability to write pattern matches not only imperatively, but also in a reg-exp style and we do have a reservation of special matching terminal symbols in the language. We just did not have time to implement yet.

## Reading JSON data
The best way to describe our features is to show some code:
```cs
[TestCase]
public void ReadSimple2() //using DYNAMIC
{
  var obj = "{a: -2, b: true, c: false, d: 'hello'}".JSONToDynamic();
  
  Assert.AreEqual(-2, obj.a);
  Assert.AreEqual(true, obj.b);
  Assert.AreEqual(false, obj.c);
  Assert.AreEqual("hello", obj.d);
}
 
[TestCase]
public void ReadSimpleNameWithSpace() //using DYNAMIC
{
  var obj = @"{a: -2, 'b or \'': 'yes, ok', c: false, d: 'hello'}".JSONToDynamic();
  
  Assert.AreEqual(-2, obj.a);
  Assert.AreEqual("yes, ok", obj["b or '"]);
  Assert.AreEqual(false, obj.c);
  Assert.AreEqual("hello", obj.d);
}
 
[TestCase]
public void RootObject() //using JSONData
{
  var src = @"{a: 1, b: true, c: null}";
 
  var parser = new JP(  new JL( new StringSource(src) )  );
 
  parser.Parse();
 
  Assert.IsInstanceOf(typeof(JSONDataMap), parser.ResultContext.ResultObject);
  var obj = (JSONDataMap)parser.ResultContext.ResultObject;
 
  Assert.AreEqual(3, obj.Count);
  Assert.AreEqual(1, obj["a"]);
  Assert.AreEqual(true, obj["b"]);
  Assert.AreEqual(null, obj["c"]);
}
```
In the code above I read JSON content into dynamic and JSONData hashtable. And now, the shocking confession: **NFX.JSON does not support reading JSON into some arbitrary CLR classes!** Why? Because it is not needed, and it is impossible to implement correctly as JSON is a total "impedance" mismatch for CLR complex types. I do know about Newtonsoft etc, but I have never ever had a need to deserialize JSON into type-safe structure because either: a). you need to code that by hand anyway or b). your CLR structure must be dumb-simple in order to map to JSON 1:1. NFX design does not endorse the creation of garbage DTO (data transfer objects) just for the purpose of being able to read form JSON. Dynamic languages are far more superior for these purposes, so I decided NOT TO implement JSON deserialization into customCLR types. Think about it, and you would agree that working with "dynamic" keyword is far more convenient than creating 100s of junk classes.

## Writing JSON data
Writing is a whole another story. JSONWriter class serializes any CLR complex type, IEnumerable or IDictionary into JSON:
```cs
[TestCase]
public void RootDictionary_object()
{
  var dict = new Dictionary<object, object>
      {
        {"name", "Lenin"}, 
        {"in space", true},
        {1905, true},
        {1917, true},
        {1961, false},
        {"Bank", null}
      };
  var json = JW.Write(dict);
  Console.WriteLine(json);
  Assert.AreEqual("{\"name\":\"Lenin\",\"in space\":true,\"1905\":true,\"1917\":true,\"1961\":false,\"Bank\":null}",  json);
}
```
A more complex cases:
```cs
[TestCase]
public void RootListOfDictionaries_object_SpaceSymbols()
{
 var lst = new List<object>
     {
       12,
       16,
       new Dictionary<object, object>{ {"name", "Lenin"}, {"in space", true}},
       new Dictionary<object, object>{ {"name", "Solovei"}, {"in space", false}},
       true,
       true,
       -1789,
       new Dictionary<object, object>{ {"name", "Dodik"}, {"in space", false}}
     };
 var json = JW.Write(lst, new JSONWritingOptions{SpaceSymbols=true});
 Console.WriteLine(json);
 Assert.AreEqual("[12, 16, {\"name\": \"Lenin\", \"in space\": true}, {\"name\": \"Solovei\", \"in space\": false}, true, true, -1789, {\"name\": \"Dodik\", \"in space\": false}]", json);
}

[TestCase]
public void RootDictionaryWithLists_object()
{
  var lst = new Dictionary<object, object>
      {
        {"Important", true},
        {"Patient", new Dictionary<string, object>
                    {
                      {"LastName", "Kozloff"},
                      {"FirstName", "Alexander"}, 
                      {"Occupation", "Idiot"}
                    }
        },
        {"Salaries", new List<object>{30000, 78000, 125000, 4000000} },
        {"Cars", new List<object>{"Buick", "Ferrari", "Lada", new Dictionary<string, object>
                                                              {
                                                                {"Make", "Zaporozhets"},
                                                                {"Model", "Gorbatiy"},
                                                                {"Year", 1971}
                                                              }
        }},
      };
  var json = JW.Write(lst, JSONWritingOptions.PrettyPrint);
  Console.WriteLine(json);
 
  var expected =
@"
{
  ""Important"": true, 
  ""Patient"": 
    {
      ""LastName"": ""Kozloff"", 
      ""FirstName"": ""Alexander"", 
      ""Occupation"": ""Idiot""
    }, 
  ""Salaries"": [30000, 78000, 125000, 4000000], 
  ""Cars"": [""Buick"", ""Ferrari"", ""Lada"", 
      {
        ""Make"": ""Zaporozhets"", 
        ""Model"": ""Gorbatiy"", 
        ""Year"": 1971
      }]
}";
  Assert.AreEqual(expected, json);
}
```
And now, from JSONDynamicObject:
```cs
[TestCase]
public void Dynamic1()
{
  dynamic dob = new JDO(NFX.Serialization.JSON.JSONDynamicObjectKind.Map);
  
  dob.FirstName = "Serge";
  dob.LastName = "Rachmaninoff";
  dob["Middle Name"] = "V";
  
  var json = JW.Write(dob);
  
  Console.WriteLine(json);
  
  Assert.AreEqual("{\"FirstName\":\"Serge\",\"LastName\":\"Rachmaninoff\",\"Middle Name\":\"V\"}", json);
}
```
How about a full loop write->JSON->read:
```cs
[TestCase]
public void Dynamic3_WriteRead()
{
  dynamic dob = new JDO(NFX.Serialization.JSON.JSONDynamicObjectKind.Map);
  dob.FirstName = "Al";
  dob.LastName = "Kutz";
  dob.Autos = new List<string>{"Buick", "Chevy", "Mazda", "Oka"};
  
  string json = JW.Write(dob);
  
  var dob2 = json.JSONToDynamic();
  Assert.AreEqual(dob2.FirstName, dob.FirstName);
  Assert.AreEqual(dob2.LastName, dob.LastName);
  Assert.AreEqual(dob2.Autos.Count, dob.Autos.Count);
}
```
And some crazy Unicode content, notice the option to write JSON using ASCII-only:
```cs
[TestCase]
public void StringEscapes_2_ASCII_NON_ASCII_Targets()
{
  var lst = new List<object>{ "Hello\n\rDolly!", "Главное за сутки"};
  var json = JW.Write(lst, JSONWritingOptions.CompactASCII ); //ASCII-only
  Console.WriteLine(json);
  Assert.AreEqual("[\"Hello\\n\\rDolly!\",\"\\u0413\\u043b\\u0430\\u0432\\u043d\\u043e\\u0435 \\u0437\\u0430 \\u0441\\u0443\\u0442\\u043a\\u0438\"]", json);
  json = JW.Write(lst, JSONWritingOptions.Compact );
  Console.WriteLine(json);
  Assert.AreEqual("[\"Hello\\n\\rDolly!\",\"Главное за сутки\"]", json);
}
```
How about anonymous classes? Here ya go:
```cs
[TestCase]
public void RootAnonymousClass_withArrayandSubClass()
{
  var data = new {
                   Name="Kuklachev",
                   Age=99,
                   IsGood = new object [] { 1, new {Meduza="Gargona", Salary=123m},true}
                 }; 
  var json = JW.Write(data);
  Console.WriteLine(json);
  Assert.AreEqual("{\"Name\":\"Kuklachev\",\"Age\":99,\"IsGood\":[1,{\"Meduza\":\"Gargona\",\"Salary\":123},true]}", json);
}
```
And a "regular" .NET CLR POCO class:
```cs
internal class ClassWithAutoPropFields
{
  public string Name{ get; set;}
  public int Age{ get; set;}
}

[TestCase]
public void RootAutoPropFields()
{
  var data = new ClassWithAutoPropFields{Name="Kuklachev", Age=99}; 
  
  var json = JW.Write(data);
  Console.WriteLine(json);
  Assert.AreEqual("{\"Name\":\"Kuklachev\",\"Age\":99}", json);
}
```
And a few more cool features, mostly for performance and portability:
```cs
/// <summary>
/// Denotes a CLR type-safe entity (class or struct) that can directly write itself as JSON content string. 
/// This mechanism bypasses all of the reflection/dynamic code.
/// This approach may be far more performant for some classes that need to serialize their state/data in JSON format, 
/// than relying on general-purpose JSON serializer that can serialize any type but is slower
/// </summary>
public interface IJSONWritable
{
  /// <summary>
  /// Writes entitie's data/state as JSON string
  /// </summary>
  ///<param name="wri">
  ///TextWriter to write JSON content into
  ///
  /// <param name="nestingLevel">
  /// A level of nesting that this instance is at, relative to the graph root.
  /// Implementations may elect to use this parameter to control indenting or ignore it
  /// 
  /// <param name="options">
  /// Writing options, such as indenting.
  /// Implementations may elect to use this parameter to control text output or ignore it
  /// 
  void WriteAsJSON(TextWriter wri, int nestingLevel, JSONWritingOptions options = null);
}
```
And here is the place where it is used:
```cs
/// <summary>
/// Provides base for rowset implementation. 
/// Rowsets are mutable lists of rows where all rows must have the same schema, however a rowset may contain a mix of
///  dynamic and typed rows as long as they have the same schema.
/// Rowsets are not thread-safe
/// </summary>
[Serializable]
public abstract class RowsetBase : IList<row>, IComparer<row>, IJSONWritable
{
  .........
  /// <summary>
  /// Writes rowset as JSON including schema information. 
  /// Do not call this method directly, instead call rowset.ToJSON() or use JSONWriter class
  /// </summary>
  public void WriteAsJSON(System.IO.TextWriter wri,
                          int nestingLevel,
                          JSONWritingOptions options = null)
  {
    var tp = GetType();
 
    var map = new Dictionary<string, object>
    {
      {"Instance", m_InstanceGUID.ToString("D")},
      {"Type", tp.FullName},
      {"IsTable", typeof(Table).IsAssignableFrom(tp )},
      {"Schema", m_Schema},
      {"Rows", m_List}
    };
    JSONWriter.WriteMap(wri, map, nestingLevel, options);
  } 
}  
```

## Performance/Benchmarks
And finally, some numbers. Lets compare NFX.Serialization.JSON with MS-provided stuff. Tests reside in NFX.NUnit.Integration for now:
```no-highlight
***** NFX.NUnit.Integration.Serialization.JSON.Benchmark_Serialize_DataObjectClass()
Serialize.DataObjectClass
    NFX: 15290.5198776758 op/sec 
    MS JSser: 3777.86173026067 op/sec
    MS DataContractSer: 8920.60660124888 op/sec
    Ratio NFX/JS: 4.04740061162079
    Ratio NFX/DC: 1.71406727828746

***** NFX.NUnit.Integration.Serialization.JSON.Benchmark_Serialize_DictionaryPrimitive()
Serialize.DictionaryPrimitive
    NFX: 303030.303030303 op/sec 
    MS JSser: 270270.27027027 op/sec
    MS DataContractSer: 45248.8687782805 op/sec
    Ratio NFX/JS: 1.12121212121212
    Ratio NFX/DC: 6.6969696969697 
     
***** NFX.NUnit.Integration.Serialization.JSON.Benchmark_Serialize_ListObjects()
DataContractJSONSerializer does not support this test case:
[System.Runtime.Serialization.SerializationException] 
Type 'NFX.NUnit.Integration.Serialization.APerson' with data contract name 'APerson:http://schemas.datacontract.org/2004/07/NFX.NUnit.Integration.Serialization' is not expected. Consider using a DataContractResolver or add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to DataContractSerializer.
Serialize.ListObjects
    NFX: 71942.4460431655 op/sec 
    MS JSser: 16366.612111293 op/sec
    MS DataContractSer: 1.0842021724855E-12 op/sec
    Ratio NFX/JS: 4.39568345323741
    Ratio NFX/DC: N/A 
     
***** NFX.NUnit.Integration.Serialization.JSON.Benchmark_Serialize_ListPrimitive()
Serialize.ListPrimitive
    NFX: 344827.586206897 op/sec 
    MS JSser: 370370.37037037 op/sec
    MS DataContractSer: 114942.528735632 op/sec
    Ratio NFX/JS: 0.931034482758621
    Ratio NFX/DC: 3 
     
***** NFX.NUnit.Integration.Serialization.JSON.Benchmark_Serialize_PersonClass()
Serialize.PersonClass
    NFX: 285714.285714286 op/sec 
    MS JSser: 52631.5789473684 op/sec
    MS DataContractSer: 277777.777777778 op/sec
    Ratio NFX/JS: 5.42857142857143
    Ratio NFX/DC: 1.02857142857143 
```

What we see here is that NFX JSON code really beats both Microsoft JavaScript and DataContract serializers, and frankly I had not had a chance yet to optimize JSON lexer in NFX. I guess I can squeeze another good 25% speed boost if I revise string parsing, but this is not important now.

## Conclusion
NFX provides rich support for working with JSON data format. The functionality is built on top of NFX.CodeAnalysis which unifies and simplifies the construction of lexers and parsers and, as a benefit, it allows us to filter/pattern match against JSON data without reading-in the whole JSON content. The library is well tested against edge cases like Unicode escapes, ISO dates, and also supports reading of JSON+ superset that understands comments, hex/bin/octal numeric prefixes that make this format very well suited for config files. The library writes standard JSON with extensive ability to serialize IEnumerable<>, IDictionary<,> and POCO classes specifying indentetion, ASCII vs. Unicode and ISO date options.

---
Dmitriy Khmaladze  
November 24, 2013