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
using Serilog.Extensions.Logging;
using Serilog.Formatting;
using TheDialgaTeam.Serilog.Parsing;

namespace TheDialgaTeam.Serilog.Formatting;

public sealed class AnsiMessageTemplateTextFormatter(
    LogLevelMessageTemplateOptions options,
    IFormatProvider? formatProvider = null) : ITextFormatter
{
    private readonly AnsiTemplateTextParser _ansiTemplateTextParser = new();

    public void Format(LogEvent logEvent, TextWriter output)
    {
        string? sourceContext = null;

        if (logEvent.Properties.TryGetValue(Constants.SourceContextPropertyName, out var sourceContextPropertyValue) && sourceContextPropertyValue is ScalarValue { Value: string sourceContextValue })
        {
            sourceContext = sourceContextValue;
        }
        
        var messageTemplate = options.GetMessageTemplate(sourceContext, LevelConvert.ToExtensionsLevel(logEvent.Level));

        foreach (var messageTemplateToken in _ansiTemplateTextParser.GetMessageTemplateTokens(messageTemplate))
        {
            messageTemplateToken.Render(logEvent, output, formatProvider);
        }
    }
}