# Serilog.Sinks.Log4Net

[![Build status](https://ci.appveyor.com/api/projects/status/bi8o8f5jteqvb0e5/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-log4net/branch/master)
[![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Log4Net.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Log4Net/)

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

#### Preserving Stacktrace information
Passing `3 (Default is 3)` for skipFrames, will load the relative number of frames from the call stack. This is required to be configurable to allow the caller class, method name, line number to be pulled. This is best used in scenarios where you have your own custom wrapper over Serilog or log4net.

e.g.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Log4Net(skipFrames: 3)
    .CreateLogger();
```
This change allows the correct values returned for log4net conversion template.

```xml
<conversionPattern value="[%utcdate{yyyy-MM-ddTHH:mm:ssZ}}][%level][%thread][%C(%M),%line][%message]%newline"/>
```
Output received in logs when skipFrames is 3
```txt
[2017-12-19T16:51:53Z}][INFO][1][Serilog.Sinks.Log4Net.Sample.Program(Main),39][SERILOG-custom property added for "ikson01" { firstname: "john", lastname: "doe" }]
```
Output received in logs when skipFrames is 0
```txt
[2017-12-19T16:53:12Z}][INFO][1][Serilog.Core.Sinks.SafeAggregateSink(Emit),0][SERILOG-custom property added for "ikson01" { firstname: "john", lastname: "doe" }]
```

[(More information.)](http://nblumhardt.com/2013/06/serilog-sinks-log4net/)

