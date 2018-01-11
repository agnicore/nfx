# File Utilities - delbydate

Deletes files according to the mask and time filter expression specified:


```css
 Usage:
    delbydate "path" "expr"

    path - wildcards permitted
    expr - standard C# date time expression. 
           'file' - is the reference to current file date time

 Example:

  delbydate "c:\temp\*.bak" "((file.Hour==5)||(file<DateTime.Now.AddMonths(-6)))"
  This will delete any file that has .BAK extension and was written to last time either 
  at 5 am or over 6 month ago
```
