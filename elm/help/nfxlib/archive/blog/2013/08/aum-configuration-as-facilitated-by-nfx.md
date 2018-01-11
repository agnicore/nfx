# Aum Configuration as Facilitated By NFX

## Problems with "built-in" approaches
Configuration is just another pain that everyone is constantly struggling with. I have never liked the .NET built-in configuration mechanisms. Why? Because, it does not have the so-needed functions that developers (we) have to compensate for daily. Although a typical .NET or Java developer is used to working with those limits, it is time to reconsider.

It is a fact of life - that many systems to this day (in 2013!!!!) use INI-like files with hand-written primitive string parsers. It is a total fiasco when it comes to supporting config files for DEV, TEST, SANDBOX environments.

Here, I have compiled a small list of things lacking in general config frameworks I have been working with in the past 15 years:
* Centralized network configuration - what if I have to configure 10 servers? Do I copy files? How to keep all configs in one place, say SQL db?
* Absurdly complex file locations, I once spent 1 hr trying to find the app.config file for some desktop app of mine on Windows Vista computer as installer put the file in the abyss or Profile..... folders
* Microsoft .NET configuration framework has more than 100 classes, it is very complex yet very inflexible. Many people parse text files by hand
* Inability to evaluate variables, i.e. in .NET there is no variable support in configuration, one must parse it by hand. INI files lack it. Registry is even more mess and hard to deal with
* Painful APIs, i.e. an attempt to read a non-existing node must be always precluded by IF statements in .NET. Int values are hard to get as bool, dates as numbers etc...
* Absence of unified configuration tree that could be hydrated from different sources, be it XML, INI, JSON, or even command-line args
* And finally - the **absence of unified configuration application** - every component needs to configure itself "by hand"

## Welcome to NFX Configuration
NFX library provides a unified configuration approach by providing:
* Format-abstract configuration **tree in-memory**
* Support for **navigation** (similar to x-path) within the tree
* **Variable evaluation**, node inter-referencing, infinite cycles are detected and stopped
* Environment variable (external vars) evaluation built-in
* Pluggable variable value providers
* Structural **merging, overrides** with rules. Prohibition (sealed sections) of overrides
* Pluggable variable evaluation macros (i.e. ::NOW)
* Support for XML, Laconic, Command-Line Args formats
* Full support for **imperative constructs** (macros) - loops, vars, Ifs, blocks
* Unified model to configure classes,services, properties,fields etc. from configuration
* **Aspect injection with configuration Behaviors** - named kits of config values that may be applied to different nodes indirectly. This approach addresses **cross-cutting concerns** on the configuration level
* Multiple getters for different nodes and data types (i.e. ValueAs: String/Date/Enum/Int...) with defaults

Example of an XML-based configuration:
```xml
<nfx log-root="c:\nfx\" log-csv="NFX.Log.Destinations.CSVFileDestination, NFX" log-debug="NFX.Log.Destinations.DebugDestination, NFX" debug-default-action="LogAndThrow" debug-conf-refresh="false" app-name="test-client">
 
  <log name="Logger" default-failover="destFailures">
 
    <destination type="$(/$log-csv)" name="$(/$app-name)" filename="$(@/$log-root)$(::now fmt=yyyyMMdd)-$($name).csv.log" create-dir="true" min-level="Info"></destination>

    <destination type="$(/$log-csv)" name="$(/$app-name)-perf" filename="$(@/$log-root)$(::now fmt=yyyyMMdd)-$($name).csv.log" create-dir="true" min-level="PerformanceInstrumentation" max-level="PerformanceInstrumentation"></destination>

    <destination type="$(/$log-debug)" name="$(/$app-name)-debug" filename="$(@/$log-root)$(::now fmt=yyyyMMdd)-$($name).log" min-level="Debug" max-level="TraceZ"></destination>
  </log>
</nfx>
```

Example of a Laconic configuration with the same tree as the above:
```
nfx
{
  log-root="c:\nfx\"
  log-csv="NFX.Log.Destinations.CSVFileDestination, NFX"
  log-debug="NFX.Log.Destinations.DebugDestination, NFX"
  debug-default-action="LogAndThrow"
  debug-conf-refresh="false"
  app-name="test-client"
  
  log
  {
    name="Logger" default-failover="destFailures"

    destination {
      type="$(/$log-csv)"
      name="$(/$app-name)"
      filename="$(@/$log-root)$(::now fmt=yyyyMMdd)-$($name).csv.log"
      create-dir="true"
      min-level="Info" }


    destination {
      type="$(/$log-csv)"
      name="$(/$app-name)-perf"
      filename="$(@/$log-root)$(::now fmt=yyyyMMdd)-$($name).csv.log"
      create-dir="true"
      min-level="PerformanceInstrumentation"
      max-level="PerformanceInstrumentation" }

    destination {
      type="$(/$log-debug)"
      name="$(/$app-name)-debug"
      filename="$(@/$log-root)$(::now fmt=yyyyMMdd)-$($name).log"
      min-level="Debug"
      max-level="TraceZ"}
  }
}//nfx - notice the use of comments
```

Example of an command-line configuration used to inject some compiler settings. Yes, same framework for that:
```
gluec "c:\mysrc\contacts" -assemblies NFX.dll MyBusiness.dll
                          -out "c:\templates\"
                          -opt single-file=true crlf=dos comments=false
                               override=true base=MyPages.SimplePageBase
                          -sign name="Dmitriy" date=now 
```

Because all of the things above come down to the same tree in memory, we can use those completely disjoint ways of specifying settings in the same way to configure classes in code like so:
```cs
/// <summary>
/// Implements log destination that sends emails
/// </summary>
public class SMTPDestination : Destination
{
  ......
  [Config("$smtp-host")]
  public string SmtpHost { get; set; }

  [Config("$smtp-port")]
  public int SmtpPort { get; set; }

  [Config("$smtp-ssl")]
  public bool SmtpSSL { get; set; }

  [Config("$from-address")]
  public string FromAddress { get; set; }

  [Config("$from-name")]
  public string FromName { get; set; }
  .......
}
```

The configuration is a tree in memory, but how do we bind it to the actual code structures (properties, fields etc..)? How do we use it in our code? For that we have a number of ways.

* Of course you can write regular code to bind any configuration value into any variable at runtime.
* You can create Settings- derived type safe class that wraps configuration tree in a type-safe way. This is needed primarily for performance reasons when some tight code block may suffer from frequent access to text-based values * that involve string parsing
* You can use ConfigurationAttribute.Apply(object, section) method to apply config section data to some object
* You can implement IConfigurable.Configure(section) in your class and apply section data by code to your class. This is useful for handling dynamic configuration structures when, for example a "parent" class manages many subordinate child classes that are polymorphic and may be injected by configuration

This topic organically touches inversion-of-control (really dependency injection) container of NFX.ApplicationModel. In Aum (and NFX) any process is hosted in a IApplication derivative, which injects all of its services into its own properties. There are many cases when one needs to configure their own particular components, for that FactoryUtils class is used which "manufactures" object instances as specified by the configuration and checks certain contract assertions supplied:
```cs
/// <summary>
/// Creates and configures an instance of appropriate configurable object
/// as specified in supplied config node. Applies configured behaviors
/// </summary>
public static T MakeAndConfigure<t>(IConfigSectionNode node, Type defaultType = null,
                                    IApplication application = null, object[] args = null)
       where T : IConfigurable
...............
 
protected override void DoConfigure(IConfigSectionNode node)
{
  base.DoConfigure(node);

  foreach (var snode in
               node.Children.Where(n => n.IsSameName(CONFIG_SINK_SECTION)))
    RegisterSink(FactoryUtils.MakeAndConfigure(snode) as LogSink);
}
```

## Aum Cluster Configuration
Aum Clusterware is built on top of NFX (on CLR platform), so it is all based on the NFX.Environment.Configuration classes, however it takes that configuration capabilities to the next level - the **hierarchical configuration tree** that the cluster is configured from. Because of it, we do not need to maintain configuration for 1000s of servers, we project configuration segments down the tree to arrive to final configuration section that is built for particular node/service/end point. We do come all the way down to endpoint level.

This topic definitely deserves a post of its own that I'll do in the next month.

---
Dmitriy Khmaladze  
August 14, 2013
