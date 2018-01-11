# Laconic JavaScript (LJS)
[NFX Template Compiler Tool](/tools/ntc.html) supports the LJS (Laconic Java Script - LJS) sytax inserts right in the template content.
The LJS is based on [laconic-syntax](/specs/configuration/laconic.html). 
LJS is a Laconic representation of HTML DOM elements used for custom controls (i.e. dialogs, widgets etc.) dynamic rendering.
LJS compiler translates LJS into Javascript statements that build DOM elements. 
Specify LJS compiler on `ntc` cli: `NFX.Templatization.TextJSTemplateCompiler` - compiles java script files or `NFX.Templatization.NHTCompiler` - compiles the whole page:

```
ntc.exe src.js -ext ".result" /src /c "NFX.Templatization.TextJSTemplateCompiler, NFX"
ntc.exe my_page.nht -r -ext ".auto.cs" /src /c "NFX.Templatization.NHTCompiler, NFX"
```

<br>
LJS code is marked with `/***` and `***/` delimiters. In the terms of Laconic config, every config segment becomes a DOM element and
config section attributes become become attributes of the DOM element, nested sub-section become sub-elements, section value becomes an inner text of the DOM element.
You can specify which element will be root for the new tree - first argument of function with LJS (id or element itself). 
Other arguments also can be used in LJS code through `? js_code` syntax but in this case the first argument of the function will be considered as a root 
even if you are not going specify it so you should add corresponding parameter.

```js
function buildChangeSNameForm(root, dfltName) {
  /***
  form {
    id=frmChangeSName
    action="#"
    method=post
    div="Please enter new screen name:" {}
    input {
      type=text
      name=sname
      value=?dfltName
    }
    input {
      type=submit
      value=Submit
      class=uiSmallButton
    }
  }
  ***/
}
```


After compilation that LJS code turns into:


```js
function buildChangeSNameForm(root, dfltName) {
  var Ør = arguments[0];
  //try to find the root element by id
  if (WAVE.isString(Ør)) Ør = WAVE.id(Ør);

  var Ø1 = WAVE.ce('form'); // create new element-form
  Ø1.setAttribute('id', 'frmChangeSName');
  Ø1.setAttribute('action', '#');
  Ø1.setAttribute('method', 'post');

  var Ø2 = WAVE.ce('div');
  Ø2.innerText='Please enter new screen name:';
  Ø1.appendChild(Ø2);

  var Ø3 = WAVE.ce('input');
  Ø3.setAttribute('type', 'text');
  Ø3.setAttribute('name', 'sname');
  Ø3.setAttribute('value', dfltName);
  Ø1.appendChild(Ø3);

  var Ø4 = WAVE.ce('input');
  Ø4.setAttribute('type', 'submit');
  Ø4.setAttribute('value', 'Submit');
  Ø4.setAttribute('class', 'uiSmallButton');
  Ø1.appendChild(Ø4);

  // if some root is specified, then append the new tree to it
  if (WAVE.isObject(Ør)) Ør.appendChild(Ø1);
  return Ø1;
}
```
And somewhere in your code you can call this function: `var form = buildChangeSNameForm(null, user.first_name)`.

<br>
More complicated than references to the arguments javascript statements (conditions, loops, etc.) should be defined as laconic section:
```js
function withCondition(root, count) {
  /***
  "? var a=count*2;" {}
  input {
    id=limitExceeded
    type=checkbox
    checked="?(a>10)"
  }
  "?if (a>10)" {
    div {
      class="? a>100 ? 'divCritical' : 'divWarning'"
    }
  }
  ***/
}
```
After compilation we have:
```js
function withCondition(root, count) {
  var Ør = arguments[0];
  if (WAVE.isString(Ør)) Ør = WAVE.id(Ør);

  var Ø1 = WAVE.ce('nfx');
  var a = count * 2;
  var Ø2 = WAVE.ce('input');
  Ø2.setAttribute('id', 'limitExceeded');
  Ø2.setAttribute('type', 'checkbox');
  Ø2.checked = (a > 10);
  Ø1.appendChild(Ø2);
  if (a > 10) {
    var Ø3 = WAVE.ce('div');
    Ø3.setAttribute('class', a > 100 ? 'divCritical' : 'divWarning');
    Ø1.appendChild(Ø3);
  }
  if (WAVE.isObject(Ør)) Ør.appendChild(Ø1);
  return Ø1;
}
```

<br>
You can use `?this` as a pointer on current html element:
```js
/***
table {
  "? buildRow(?this);" {}
}
***/
```
And we get:
```js
var Ø1 = WAVE.ce('table');
buildRow(Ø1);
```

<br>

Also you can create pointer to any item in tree with the construct `ljsid=<your_identifier>` and use it:
```js
/***
div {
  ljsid=root
  div = "some text" {}
  "? if(root.hasChild())" {
     ...
   }
}
***/
```

Result:
```js
var Ø1 = WAVE.ce('div');
var Ø2 = WAVE.ce('div');
Ø2.innerText = 'some text';
Ø1.appendChild(Ø2);
if (Ø1.hasChild()) {
  ...
}
```

<br>
Besides that there is a possibility to make "inline" LJS insertions:
```js
var div = "*#* div { h2='Header' {} p='Paragraph' {} } *#*";
```

turns into:

```js
  var div = (
    function() {
      var Ø1 = WAVE.ce('div');
      var Ø2 = WAVE.ce('h2');
      Ø2.innerText = 'Header';
      Ø1.appendChild(Ø2);
      var Ø3 = WAVE.ce('p');
      Ø3.innerText ='Paragraph';
      Ø1.appendChild(Ø3);
      return Ø1;
    })();
```

<br>
Usage of NHT code in LJS looks like:
```js
/***
  span="?'?[:SomeVariable]'"
***/
```
And output file will contain something like that:
```csharp
const string s1 = @"var Ø1=WAVE.ce('span'); Ø1.innerText='";
const string s2 = @"';return Ø1;";

Target.Write(s1);
Target.Write(SomeVariable);
Target.Write(s2);
```
