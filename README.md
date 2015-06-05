# Serilog.Sinks.Log4Net

[![Build status](https://ci.appveyor.com/api/projects/status/bi8o8f5jteqvb0e5/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-log4net/branch/master)

Duplicates Serilog events through the log4net pipeline to allow integration with existing code and libraries.

**Package** - [Serilog.Sinks.Log4Net](http://nuget.org/packages/serilog.sinks.log4net)
| **Platforms** - .NET 4.5

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Log4Net()
    .CreateLogger();
```

#### Context Message Marker
Passing `true` for supplyContextMessage, will add a context message of `Serilog-Log4NetSink` scoped just for the log call under the `NDC` stack, which you can utilise for Log4Net filters and other purposes. [See Log4Net documentation](https://logging.apache.org/log4net/release/manual/contexts.html#stacks).

e.g.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Log4Net(supplyContextMessage: true)
    .CreateLogger();
```

This can let you specify a filter on an appender in Log4Net. The example below, disables the Seq appender for Log4Net if the context message is present. Useful to stop doubling up logs, for when you have Log4Net and Serilog both pushing to the same sink during migration of logging practices.

```xml
<log4net>
  <appender name="TraceAppender" type="log4net.Appender.TraceAppender">
    <layout type="log4net.Layout.PatternLayout">
    <conversionPattern value="%newline%-5level %logger %newline - %message" />
    </layout>
  </appender>
  <appender name="SeqAppender" type="Seq.Client.Log4Net.SeqAppender, Seq.Client.Log4Net">
    <filter type="log4net.Filter.PropertyFilter">
      <key value="NDC" />
      <stringToMatch value="Serilog-Log4NetSink" />
      <acceptOnMatch value="false" />
    </filter>
    <bufferSize value="1" />
    <serverUrl value="http://localhost:5341/" />
  </appender>
  <root>
    <level value="All" />
    <appender-ref ref="TraceAppender" />
    <appender-ref ref="SeqAppender" />
  </root>
</log4net>
```

[(More information.)](http://nblumhardt.com/2013/06/serilog-sinks-log4net/)

