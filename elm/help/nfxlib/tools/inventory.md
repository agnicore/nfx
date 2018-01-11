# Inventory Data Extractor

The tool is designed to extract the metadata from assembly items decorated with [Inventory] attribute.
Inventory tool is utilized by <a href="https://en.wikipedia.org/wiki/Configuration_management_database" target="_target">Enterprise CMDB Solutions</a>

Metadata Example:

```cs
namespace NFX.IO
{
    [Inventory(Concerns=SystemConcerns.Testing | SystemConcerns.MissionCriticality)]
    public abstract class ReadingStreamer : Streamer
    {
     ...
    }
}    
```

CLI use:

```css
Usage:
   inventory assembly_list [/h | /? | /help]
              [/filter | /f [attributed= true|false]
                            [tiers=value]
                            [concerns=value]
                            [technology=value]
                            [schema=value]
                            [tool=value]
                            [startdate=value]
                            [enddate=value] ] 
              [{/s | /strat | /strategy  fully_qualified_type_name }]                                                       

 assembly_list - a list of assembly file names delimited by ";"

 Options:

 /h | /help | /? - displays help message 
 /f | /filter - specifies filter.
                 Multiple filters get combined using AND logical operator:
          tiers - a comma-separated set of flags: 
               { GUI, AppServer, DBServer, SystemServer, NotificationBus, All}

          concerns - a comma-separated set of flags : 
              { Security, Development, Testing, Deployment, Release,
                 Testability, Configuration, Performance, Maintainability,
                 MissionCriticality, Licensing, Features, Requirements, Luck}

          technology - tecnology name such as "Oracle" or "MongoDB"
          schema     - schema name, i.e. a name of database schema
          tool       - name of tool that inventory is performed for
          attributed - specifies whether to include only types
                       that are tagged with [Inventory] attribute 
          startdate - datetime compared to inventory start date that
                       has to be greater or equal than/to this value
          enddate - datetime compared to inventory end date that 
                      has to be less or equal than/to this value 
 /s | /strat | /strategy - injects inventorization strategy type, 
             this switch may repeat for multiple types.
             If omitted, then Basic,RecordModel strategies are used by default. 
             fully_qualified_type_name - must be fully qualified

Examples:


  inventory NFX.dll;Aum.dll -f attributed=false
   Inventorizes all types, regardless of Inventory attribute NFX and Aum dlls
   using default set of inventorization strategies 


  inventory MyTypes.dll /f technology="Oracle" schema="products"
   Inventorizes Inventory-attributed types in MyTypes.dll
   that have particular technology and schema attribute fields specified

  inventory MyTypes.dll 
    /f technology="MongoDB" schema="products" tiers=DBServer
    /s "NFX.Inventorization.RecordModelInventorization, NFX"
   Inventorizes Inventory-attributed types in MyTypes.dll, using only 
   RecordModelInventorization strategy declared in NFX.dll,
   that have particular technology and schema attribute fields specified
```
