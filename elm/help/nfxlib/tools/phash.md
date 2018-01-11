# Password Hash Generator

NFX Security functionality is provided by the various `ISecurityManager` injectable implementations 
which, in turn use injectable `IPasswordManager` providing password generation/hashing functions.
See the [NFX.Security](/docs/NFX.Security.html) topic.
The `phash` tool is the CLI application specifically designed to generate password vectors for password managers.
The strength and algorithm used for hashing is injectable via configuration. The tool also
takes command line arguments.

Tool capabilities:

* Acquires entropy samples from user, unless `-ne` is specified
* Requires certain password strength score, as specified by `-score`
* Allows for various hashing levels via `-lvl`

The configuration file specifies the PasswordManager, available hashing algorithms.
By default the `DefaultPasswordManager` is injected which uses the simplest 
`MD5PasswordHashingAlgorithm`.

Command-line usage:

```css

   phash [/h | /? | /help]
              [/pp | /pretty]
              [/ne | /noentropy]
              [/st | /score]
              [/lvl |/level]


 Options:

 /h | /help | /? - displays help message
 /pp | /pretty  - pretty prints hashed password
 /ne | /noentropy  - precludes entropy acquisition from user
 /st | /score int - strength score threshold

  /lvl | /level  level - the strength level of hash
              level = Default| Minimum |BelowNormal|Normal|AboveNormal|Maximum

 Examples:

  phash -pp -st 90 -lvl Normal
 Pretty print with score threshold 90% hashing strength Normal
```

The example password vector (JSON object) for password "abcd1234", note: the tool was executed with 
 `-st 10` switch that lowered the required password score (as "abcd1234" is a very weak password that would have been rejected without the switch)
 
```json
Hashed Password:

{
  "algo":"MD5",
  "fam":"Text",
  "hash":"MckMtVWvDazQ3Q1+qcm1MA==",
  "salt":"yil5Oom3kepMbyeOOwDGqaFtPbwHIGZYod1Yoxwr"
}
```