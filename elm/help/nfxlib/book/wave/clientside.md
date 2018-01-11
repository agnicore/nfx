# Client Side

On the client side WAVE framework is represented by javascript libraries **wv.js** and **wv.gui.js**. 
These libraries provide powerful instruments to implement complex reactive web applications. 
The framework contains:
* Various auxiliary General-Purpose Functions to work with strings, arrays and objects, different type conversion functions.
* Geometry contains a large number of functions to work with geometry tasks.
* Classes to build/render GUI elements like dialogs, toasts, forms, etc.
* `EventManager` mixin that implements the event-handler mechanism and can be added to any class.
* `RecordModel` contains classes for MVVM pattern implementation.
* `UTest` - implementation of unit testing concept.

Walkable mixin that enables function chaining that facilitates lazy evaluation via lambda-functions, contains full set of projection, filtering, partitioning, aggregation, grouping and etc. operations - analogues of corresponding methods of `IEnumerable` in C#.

It is needed to pay more attention to `RecordModel`. It consists of:
* `Record`. The purpose of this class is to represent server-side `NFX.DataAccess.CRUD.Schema` on the client side along with view model/controller. 
`Record` instances are initialized using server `NFX.Wave.Client.RecordModelGenerator` class that turns Schema into JSON object suitable for record initialization on client.
* `Field. Record` consists of fields and Field represents a `FieldDef` from server-side `NFX.DataAccess.CRUD.Schema` on the client side along with view model/controller.
* `RecordView` and `FieldView`. Binds Record model (an instance of `Record` class) with UI builder/rendering library (**wv.gui.js**) which dynamically builds the DOM in the attached view components. 
Thus, changes made to view model/data model in record instance will get automatically represented in the attached UI (dual way binding).

**Example 1:**

```js
var rec = new WAVE.RecordModel.Record({ID: 'REC-1', 
          fields:
          [
            {def: {Name: 'FirstName', Type: 'string'}, val: 'John'},
            {def: {Name: 'LastName', Type: 'string'}, val: 'Smith'},
            {def: {Name: 'Age', Type: 'int'}, val: 33},
            {def: {Name: 'Helper', Type: 'string', Stored: false}}
          ]});

var d = JSON.stringify(rec.data());
```

**Example 2:**

```js
var rec = new WAVE.RecordModel.Record('ID-123456',
            function(){
              new this.Field({Name: 'FirstName',
                              Type: 'string',
                              Required: true});
              new this.Field({Name: 'LastName', Type: 'string'});
              new this.Field({Name: 'Age', Type: 'int'});
            }
          );
```

**Example 3:**

```js
var REC = new WAVE.RecordModel.Record({ID: 'R1',
            fields:
            [
              { def: { Name: 'FirstName', 
                       Type: 'string',
                       Required: true, 
                       DefaultValue: 'Bob',
                       Placeholder: 'Your First Name' },
                val: 'John'},
              { def: { Name: 'LastName', 
                       Type: 'string',
                       Required: false,
                       Placeholder: 'Your Last Name'},
                val: 'Smith'},
              { def: { Name: 'MusicType', 
                       Type: 'string',
                       Case: WAVE.RecordModel.CASE_UPPER,
                       LookupDict: {
                         RСK: 'Rock',
                         RAP: 'Rap',
                         CLS: 'Classical music'}
                     }
              }
            ]}
          );

var RVIEW = new WAVE.RecordModel.RecordView('V1', REC);
```

HTML client-side code:

```html
<form data-wv-rid="V1">
  <div class="fView" data-wv-fname="FirstName"></div>
  <div class="fView" data-wv-fname="LastName"></div>
  <div class="fView" data-wv-fname="MusicType"></div>
        
  Record-level errors:
  <div class="fView" data-wv-fname="#"></div>
</form>
```
