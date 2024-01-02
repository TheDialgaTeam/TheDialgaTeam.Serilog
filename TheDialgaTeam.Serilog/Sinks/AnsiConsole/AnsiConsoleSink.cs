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

using Serilog.Core;
using Serilog.Events;
using TheDialgaTeam.Serilog.Formatting;

namespace TheDialgaTeam.Serilog.Sinks.AnsiConsole;

public sealed class AnsiConsoleSink : ILogEventSink
{
    private readonly AnsiMessageTemplateTextFormatter _textFormatter;
    private readonly object _syncLock = new();

    public AnsiConsoleSink(AnsiMessageTemplateTextFormatter textFormatter)
    {
        _textFormatter = textFormatter;
    }
    
    public AnsiConsoleSink(LogLevelMessageTemplateOptions options, IFormatProvider? formatProvider = null)
    {
        _textFormatter = new AnsiMessageTemplateTextFormatter(options, formatProvider);
    }
    
    public void Emit(LogEvent logEvent)
    {
        var output = logEvent.Level < LogEventLevel.Error ? Console.Out : Console.Error;

        lock (_syncLock)
        {
            _textFormatter.Format(logEvent, output);
            output.Flush();
        }
    }
}