# File Utilities - getdatetime

Returns the timestamp offset from the local timestamp by the specified parameters.

```css
Usage:
  getdatetime [format] ([[{+/-},{part spec},{num}]/[{=,{part spec},{num}]])

  [format] - standard .net date/time formatting pattern 
     "yyyyMMddHHmmss" will be used if no [format] param supplied

  (modifier) - optional, allows to alter date time, may be repeated, 
               evaluated from left to right

   Shift Modifier:
    [{+/-},{part spec},{num}] - allows to shift date time
      {+/-}  add/subtract
      {part spec}  date/time part to change
      {num}  value of change (integer)

   Set Modifier:
    [{=},{part spec},{num}] - allows to set date time part to a specific value
      {=}  set
      {part spec}  date/time part to change
      {num}  new value (integer)

   part spec:
      {m/d/y/h/i/s}
          m  - month
          d  - day
          y  - year
          h  - hour
          i  - minute
          s  - second


Example:

 getdatetime yyyyMMddHHmmss  -,d,15
  returns date and time 15 days ago from current system date time stamp

 getdatetime yyyyMMddHHmmss  -,d,15 =,h,1 =,i,0 =,s,0
  returns date 15 days ago from current system date, time is set to 1:00 AM
```
