# Telemetry Viewer

NOTE: This tool is **deprecated** in favour of web-based instrumentation UI.


Provides a telemetry server endpoint bound to `ITelemetryReceiver` implementation
 with WinForms-based GUI (charts).

Point the `TelemetryInstrumentationProvider` at the server endpoint to start the visualization.

```xml
<instrumentation name="Instruments" interval-ms="5000">
  <provider  name="Telemetry Instrumentation Provider"
             type="NFX.Instrumentation.Telemetry.TelemetryInstrumentationProvider"
             use-log="false"
             receiver-node="sync://127.0.0.1:8300"
  />
</instrumentation>
```

