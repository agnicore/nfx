# ORM Idiotizm

For the over 10 past years I have been a strong critic of ORM concept. I feel that majority of people just don't understand the root of the problem with ORM, don't understand what ORM is for, and even should it be there in the first place. I truly think that it should not, the whole concept of ORM is wrong and here is why. I'll be talking in very practical terms. 

What is ORM? ORM allows us to map relational entities (read "tables" with keys and references) into objects in your code. Sounds very cool. Let me ask you a "stupid" question -  **why do you need to have those objects in your code to begin with?** Huh? No, wait, don't close this page, keep on  reading... why? Ah... because you need to show the grid of customers then change selected customer and save it back to db.

 Ok, let's start from display grid(or any kind of list, such as a treeview), so grid is object-oriented piece of software that "draws" lines on screen (or HTML)  that mimics a table, also it draws little string values in those "cells", so grid needs to get some data to draw. OK, but don't we have data in database? WE DO. Can we read this data? WE CAN. Can we read different column types? WE CAN. Can we know what columns are in there (in database/table). Yes, WE CAN. Can we build grid directly from database. WE CAN. Can we do this automatically relying on database meta-data (data about columns, sizes, requirement etc...)? WE CAN.

 So, why do we need stupid POJO or POCO objects in between to draw grids? We do not. We need a "smart adapter", - a class that is absolutely universal - it takes "data descriptor" (think of DataTable,DataSet,DataRow in .NET) and build grid columns including their default width, maybe even color to indicate importance, etc... This class ("adapter") may also take custom style-rule set that will hide certain columns or make them smaller - but this is just  for DISPLAY ONLY purpose.

 Back in 1995 I have used client-server database and I needed to build an application with 200 data-entry screens. What I did, I created a grid-derivative that was "so smart" - in 90% of cases I just wrote a select SQL - formed a dataSource and then grid showed it to me in design time (I used Delphi 1) where I dragged and colored columns to make them look pretty. Thats it!  No Line of code was written in business layer, no line of code in the grid form, just NONE. No strings to configure, and when you add columns to database - they may or may not automatically appear in the grid for corresponding table, depending on 'AutoRebuildColumns' boolean property. It also worked much faster than Hibernate (that did not exist back then) because it did not create any redundant copies of data in memory(no POJO/POCO).

Same approach for displaying detail forms: textboxes, checkmarks, radios etc...  All you need is this: allow developer to place controls on form to satisfy client's particular layout requirements, wire-up every data-entry control DIRECTLY to database column by name (i'll explain how) and write NO BOILER PLATE CODE. If column "Sex" is required in table "Customer" then radio-button should automatically be named "SEX" and be required upon 'Save". You don't need to write any validation code, any "Validators" - this is all extra work and really not needed - everything is already explicitly defined in your database schema. 

How to connect, say textbox directly to TABLE.COLUMN in database? Easy - you need to write some "framework" code that make your textbox "smart" by associating this data-entry field with particular column by STRING NAME, make a property "FieldName", and then everything should be read from metadata and applied to "object-oriented" properties of a text box, such as: Readonly, MaxSize  etc...   Some properties do not exist in DB, i.e. "text color" but that can be specified by developer in concrete form.

You feel what I am getting at here? Yes, AUTOMATIC DATABASE REFLECTION, with 100% CUSTOMIZATION ability by developer in every screen, but this is much more rare that just displaying/saving data back and forth in "default way" - without special customization.

Ok, now another part - working with "BUSINESS" objects. The truth is that in may cases you just don't need a business object because you are interfacing with table that has limited functionality, column and row count (think US_STATES table). For complex objects such as "Patient Medical Record" Hibernate and the like dont do you much good anyway, reason being - you need to have 100% control over SQL code/stored procedures being sent to database, and yes HQL is a language of Hibernate, but wait, why do I need it, I already know PL/SQL or t-SQL and know it well, and it has operators that Hibernate does not even dream about (think of compilation hints, things like grouping sets, CONNECT BY etc...).  So when I save 'Patient Medical Record" I certainly use CRUD-like helpers but definitely not a monster like Hibernate or MS Entity framework.

One last thought -  when you use LINQ that you all love so much, don't forget that LINQ knows nothing about your collections in terms of indexes.... binary searches... consequently it is all very slow linear search... some food for thought... LINQ to SQL? I'd rather use SQL to C# (it's a joke).

Let's use excavator to dig swimming pool and spoon to eat cake but not vice-versa.

Read this link below, it will open you eyes!  
<a href="http://incubator.apache.org/empire-db/empiredb/hibernate.htm" target="_blank">What's wrong with Hibernate and JPA</a>

---
Dmitriy Khmaladze  
January 27, 2010