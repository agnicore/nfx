# ORM Condemnation (....Continued)

Did I tell you that I am not a fan of OR Mapping solutions at all?  
Why?  
Because of concept and code bloat and very little benefit (if any). I am teaching developers this way:

Majority of business applications are comprised of 3 major parts:
1. **Master Data** - such as dictionaries and support tables
2. **Normal Processing/OLTP**  - used every day, this is what app is used for, usually real-world data is keyed on [1] and stored depending on some sort of a date/time key
3. **Reports** - query transactional-accumulated (or live) data (entered in [2]) , join it with master data for descriptions and output to user

The fact of life is that "Master Data" tables (hundreds of them in big systems) are very dummy in the most cases, and any credible architect would build interfaces/CRUD logic in a template-based way. For those templates (base classes probably) SQLs are usually very simple, the row count is mostly <1000 per table (not always), and really there is NO BUSINESS logic needed to be applied but map SQL columns to on-screen controls (be it Forms, Web or anything else). Making developer maintain a bloat of 100s of classes just to map A to A does not make much sense. What does make sense - build your general ancestor class smart enough to supply tons of metadata from your back-end automatically (such as max-length, required (not null), format etc...). I don't really need to map DB to object for that.

Now about [b] item - OLTP screens or entry processes(be it GUI or ETL or...who knows). Those definitely need to go through good interface in terms of classes, but again, if you have ever built "big" software that contains 100s of data-entry screens - the pattern becomes trending - you are NOT dealing with simple SQLs - now when user hits  "Save" you probably touch 3-10 tables etc. I personally never user ORM for that but write classes by hand with plenty of XML comments and human methods that are centered around business , not around schema. Database schema is designed for normal-forms compliance (most of the times), not class/interface usability by developers. So, in a financial system, I would have "Portfolio" class and "Save" method, and maybe even "IStorage" provider that implements direct SQL backing store so portfolios could save ,or maybe middle-tier store. So SQL is not the sole solution now.  
The hand-written code makes sense though, and there is no bloat. Using LINQ to SQL would not have helped really as "Save" method for portfolios touches 10s of tables due to business rules. And I don't really want to have database code in C#, maybe database orchestration code I do, I'd rather use stored proc, or maybe some other middle tier process as an abstraction layer. So now I need to use ORM there, again, I need to write good well-though queries that don't put unneeded locks etc...  
So what benefit does LINQ to SQL/Hibernate/EF or any other tool would give in my case?  
**None.**

Check out this link regarding Microsoft ORM technologies trending: <a href="http://www.infoq.com/news/2008/11/DLINQ-Future" target="_blank">Is LINQ to SQL Truly Dead?</a>

---
Dmitriy Khmaladze  
March 29, 2010