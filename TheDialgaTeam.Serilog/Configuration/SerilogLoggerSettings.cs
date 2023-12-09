// MIT License
// 
// Copyright (c) 2023 Yong Jian Ming
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace TheDialgaTeam.Serilog.Configuration;

internal sealed class SerilogLoggerSettings : ILoggerSettings, IDisposable
{
    private LogLevelOptions _logLevelOptions;
    private readonly IDisposable?[] _disposables;

    public SerilogLoggerSettings(IOptionsMonitor<LogLevelOptions> logLevelOptions)
    {
        _logLevelOptions = logLevelOptions.CurrentValue;

        _disposables = new[]
        {
            logLevelOptions.OnChange(options => _logLevelOptions = options)
        };
    }

    public void Configure(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.Filter.ByIncludingOnly(logEvent =>
        {
            string? sourceContext = null;

            if (logEvent.Properties.TryGetValue(Constants.SourceContextPropertyName, out var logEventPropertyValue) && logEventPropertyValue is ScalarValue { Value: string sourceContextValue })
            {
                sourceContext = sourceContextValue;
            }

            return LevelConvert.ToExtensionsLevel(logEvent.Level) >= _logLevelOptions.GetMinimumLogLevel(sourceContext);
        });
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable?.Dispose();
        }
    }
}