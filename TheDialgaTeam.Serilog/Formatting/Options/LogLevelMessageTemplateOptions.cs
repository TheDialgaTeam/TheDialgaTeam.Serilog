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

using Microsoft.Extensions.Logging;

namespace TheDialgaTeam.Serilog.Formatting.Options;

public sealed class LogLevelMessageTemplateOptions
{
    public LogLevelMessageTemplate Default { get; set; } = new();

    public Dictionary<string, LogLevelMessageTemplate> Overrides { get; set; } = new();

    public string GetMessageTemplate(string? sourceContext, LogLevel logLevel)
    {
        var messageTemplate = GetLogLevelMessageTemplate(sourceContext);

        return logLevel switch
        {
            LogLevel.Trace => messageTemplate.Trace ?? messageTemplate.Default,
            LogLevel.Debug => messageTemplate.Debug ?? messageTemplate.Default,
            LogLevel.Information => messageTemplate.Information ?? messageTemplate.Default,
            LogLevel.Warning => messageTemplate.Warning ?? messageTemplate.Default,
            LogLevel.Error => messageTemplate.Error ?? messageTemplate.Default,
            LogLevel.Critical => messageTemplate.Critical ?? messageTemplate.Default,
            var _ => string.Empty
        };
    }

    private LogLevelMessageTemplate GetLogLevelMessageTemplate(string? sourceContext)
    {
        if (sourceContext is null)
        {
            return Default;
        }

        foreach (var (key, value) in Overrides.OrderByDescending(pair => pair.Key))
        {
            if (sourceContext.StartsWith(key) && (sourceContext.Length == key.Length || sourceContext[key.Length] == '.'))
            {
                return value;
            }
        }

        return Default;
    }
}

public sealed class LogLevelMessageTemplate
{
    public string Default { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss} {Message:l}{NewLine}{Exception}";

    public string? Trace { get; set; }

    public string? Debug { get; set; }

    public string? Information { get; set; }

    public string? Warning { get; set; }

    public string? Error { get; set; }

    public string? Critical { get; set; }
}