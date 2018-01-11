# Laconic Configuration Specification

Laconic Configuration (the formal format name is LACONFIG) is a text data format, similar to JSON or XML.
It is primarily used for configuration, but can also be used for data persistence.

Laconic is closer to XML than JSON as it has a mandatory root section node with child subsections.
A section is a named entity with children of two kinds: sub-sections or attributes. Attributes can not have
children, whereas sections can. Both sections and attributes can have values. The block constructs are formed
by curly braces. 

While compared to XML, Laconic format is more convenient as it is less verbose, ans supports single/multi-line 
comments, Unicode escapes and verbatim strings a-la C#. Strings may be taken in single or double quotes.

The identifiers and values are strings. Strings must be taken in quotes if they have spaces or special
characters which are: '{', '}', and '='



```js
root=value
{
  sec-a
  {
    attr1=1
    attr2=xyz
    attr3='x y z'
  }

  sec-b="value with space"
  {
    attr3=123
    sec-c {}
  }
}
```



Laconfig parser understands single or multi-line comments which are similar to the ones in C#
with the addition of another block comment `|*   *|` construct

```js
root
{
  age=32 // single line comment
  
  /* another one */
  
  /* 
    multiline 
    |*
      and nested
      commented block within
    *|  
    comment 
  */
}
```

Refer to configuration documentation (link)

TODO:  This must be moved into CONFIGURATION section, as it has nothing to do with Laconic

An `_override` attribute is used for more sophisticated control over configuration section merge process.
It has the following valid values:

* attributes - attributes-only override
* sections - sections-only override
* all - sections and attributes override
* replace - overriding section replaces base completely
* stop - no section can modify anything in this one
* fail - an exception is thrown when a child tries to override this section

```js
root
{
  sec-a
  {
    _override='attributes'
    attr1=1
    attr2='xyz'
  }
  
  sec-b
  {
    _override='sections'
    attr3=123
    sec-c {}
  }
}
```









