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

[(More information.)](http://nblumhardt.com/2013/06/serilog-sinks-log4net/)

