// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Security.AccessControl;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using log4net;
using log4net.Core;

namespace Serilog.Sinks.Log4Net
{
    class Log4NetSink : ILogEventSink
    {
        class NullDisposable : IDisposable
        {
            public void Dispose() { }
        }

        private const string ContextMessage = "Serilog-Log4NetSink";
        private const int DefaultSkipFrames = 3;
        readonly string _defaultLoggerName;
        readonly IFormatProvider _formatProvider;
        private readonly bool _supplyContextMessage;
        private readonly int _skipFrames = 3;

        public Log4NetSink(string defaultLoggerName, IFormatProvider formatProvider = null, bool supplyContextMessage = false, int skipFrames = DefaultSkipFrames)
        {
            if (defaultLoggerName == null)
                throw new ArgumentNullException(nameof(defaultLoggerName));
            _defaultLoggerName = defaultLoggerName;
            _formatProvider = formatProvider;
            _supplyContextMessage = supplyContextMessage;
            if (_skipFrames < 0)
            {
                throw new ArgumentException(nameof(skipFrames));
            }
            _skipFrames = skipFrames;
        }

        private Type GetCallingType()
        {
            return new StackFrame(_skipFrames, false).GetMethod().DeclaringType;
        }

        public void Emit(LogEvent logEvent)
        {
            var loggerName = _defaultLoggerName;

            LogEventPropertyValue sourceContext;
            if (logEvent.Properties.TryGetValue(Constants.SourceContextPropertyName, out sourceContext))
            {
                var sv = sourceContext as ScalarValue;
                if (sv != null && sv.Value is string)
                    loggerName = (string)sv.Value;
            }

            var message = logEvent.RenderMessage(_formatProvider);
            var exception = logEvent.Exception;

            var logger = LogManager.GetLogger(loggerName);

            using (_supplyContextMessage ? ThreadContext.Stacks["NDC"].Push(ContextMessage) : new NullDisposable())
            {
                var type = GetCallingType();
                switch (logEvent.Level)
                {
                    case LogEventLevel.Verbose:
                    case LogEventLevel.Debug:
                        logger.Logger.Log(type, Level.Debug, message, exception);
                        break;
                    case LogEventLevel.Information:
                        logger.Logger.Log(type, Level.Info, message, exception);
                        break;
                    case LogEventLevel.Warning:
                        logger.Logger.Log(type, Level.Warn, message, exception);
                        break;
                    case LogEventLevel.Error:
                        logger.Logger.Log(type, Level.Error, message, exception);
                        break;
                    case LogEventLevel.Fatal:
                        logger.Logger.Log(type, Level.Fatal, message, exception);
                        break;
                    default:
                        SelfLog.WriteLine("Unexpected logging level, writing to log4net as Info");
                        logger.Logger.Log(type, Level.Info, message, exception);
                        break;
                }
            }
        }
    }
}
