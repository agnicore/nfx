# Relational Schema

Relational Schema is the conceptual scripting language that allows for declarative database structure creation of:
* Tables
* Indexes
* Foreign Keys
* Domains

The script is based on LACONIC config format and is basically a code transform tool that generates script suitable for particular backend. 
For example one can invoke the [**RSC** (Rel Schema Compiler) tool](/tools/rsc.html) from command line passing the specific target and the SQL DDL (Data Definition Language) specific to that target will be generated.

RSC generates proper prefixes for related objects, for example if a table is called "USER", the foreign key constraint would be called "FK_USER…".

The key feature of RSC is the ability to use injectable domains definitions. This is true EVEN for databases that do not support domains, for example one could use `THumanName`  and define it as `varchar(25)`, so now in multiple places `THumanName` can be used as a domain, resulting script will have `varchar(25)`.
RSC also has capabilities for macro/mixins and scripts. This is very useful for example for including column block such as "customer address".