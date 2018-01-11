# MVC

The processing of MVC-related requests with using of WAVE framework passes in following stages:
According to adjusted configuration server matches appropriate MVC-handler. Typical configuration of MVC handler looks like:

```js
handler
{
  order=2
  type="NFX.Wave.Handlers.MVCHandler, NFX.Wave"
  type-location
  {
    assembly="Example.exe"
    ns { name="Example.Controllers" }
  }
  match
  {
    path="/{type=mycontroller}/{mvc-action=myaction}"
    var { query-name="*" }
  }
}
```

MVC-handler has information about location of corresponding controller and tries to find appropriate MVC-action under this controller by comparing action 
attributes `NFX.Wave.MVC.ActionAttribute` that are assigned to methods in custom controller class that must be inherited from `NFX.Wave.MVC.Controller`. 
The controller that matches aforementioned handler may look like:

```csharp
public class MyController: Controller
{
  [Action(name : "myaction", order : 2, matchScript : "match{methods=GET}")]
  public object GetPerson (JSONDataMap req)
  {
    var row = new Person
    {
      ID = req["PersonID"],
      FirstName = "Yuriy",
      LastName = "Gagarin"
    };
    return new ClientRecord(row, null);
  }
}
```

To customize the Action attribute following parameters can be used:
* `Name` - when set, specifies the invocation name override, null by default which means that the name of decorated member should be used.
* `Order` - dictates the matchmaking order of actions within the group.
* `MatchScript` - specify match script in Laconic config format.
* `StrictParamBinding` - must be set as true if default parameter binder should not perform indirect value conversions, i.e. integer tick number as date time.

In addition controller classes and actions can be specified with following attributes:
* `NoCacheAttribute` - decorates controller classes or actions that set NoCache headers in response.
* `SessionCSRFCheckAttribute` - decorates controller classes or actions that need to check CSRF token on POST against the user session.

Handler invokes MVC-action (if matched) and include action result in response to client. 
There are several predefined MVC action result types that implement `IActionResult` interface, they decorate entities that get returned by MVC actions and can get executed to generate some result action (command pattern):
* `FileDownload` - downloads a local file.
* `Redirect` - redirects user to some URL.
* `Picture` - returns/downloads an image.
* `ClientRecord` - returns row as JSON object for WAVE.RecordModel.Record constructor on client side.
* `JSONResult` - returns JSON object with JSON writing options. If JSON options are not needed then just return CLR object directly from controller action without this wrapper
* `Http404NotFound` - returns HTTP 404 - not found, it can be used in place of returning exceptions where needed as it is faster
* `Http403Forbidden` - returns HTTP 403 - forbidden, it can be used in place of returning exceptions where needed as it is faster.

Besides in most cases action result is web page which is generated as instance of the class inherited from `NFX.Wave.Templatization.WaveTemplate`.