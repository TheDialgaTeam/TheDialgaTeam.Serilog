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

using Serilog.Events;
using Serilog.Parsing;
using TheDialgaTeam.Serilog.Events;

namespace TheDialgaTeam.Serilog.Parsing;

internal sealed class AnsiLevelToken(PropertyToken propertyToken) : AnsiMessageTemplateToken<PropertyToken>(propertyToken)
{
    private static readonly string[][] TitleCaseLevelMap =
    [
        ["V", "Vb", "Vrb", "Verb"],
        ["D", "De", "Dbg", "Dbug"],
        ["I", "In", "Inf", "Info"],
        ["W", "Wn", "Wrn", "Warn"],
        ["E", "Er", "Err", "Eror"],
        ["F", "Fa", "Ftl", "Fatl"]
    ];

    private static readonly string[][] LowercaseLevelMap =
    [
        ["v", "vb", "vrb", "verb"],
        ["d", "de", "dbg", "dbug"],
        ["i", "in", "inf", "info"],
        ["w", "wn", "wrn", "warn"],
        ["e", "er", "err", "eror"],
        ["f", "fa", "ftl", "fatl"]
    ];

    private static readonly string[][] UppercaseLevelMap =
    [
        ["V", "VB", "VRB", "VERB"],
        ["D", "DE", "DBG", "DBUG"],
        ["I", "IN", "INF", "INFO"],
        ["W", "WN", "WRN", "WARN"],
        ["E", "ER", "ERR", "EROR"],
        ["F", "FA", "FTL", "FATL"]
    ];

    public override void Render(LogEvent logEvent, TextWriter output, IFormatProvider? formatProvider = null)
    {
        var formatStringSpan = (MessageTemplateToken.Format ?? string.Empty).AsSpan();

        if (formatStringSpan.Length < 2)
        {
            LogEventPropertyValues.Clear();
            LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(Enum.GetName(logEvent.Level)));
            Render(LogEventPropertyValues, output, formatProvider);
            return;
        }

        if (int.TryParse(formatStringSpan[1..], out var formatLength))
        {
            if (formatLength < 1) return;

            switch (formatStringSpan[0])
            {
                case 'w':
                {
                    if (formatLength > 4)
                    {
                        var result = (Enum.GetName(logEvent.Level) ?? string.Empty).ToLowerInvariant();

                        LogEventPropertyValues.Clear();
                        LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(result.Length >= formatLength ? result : result[..formatLength]));
                        Render(LogEventPropertyValues, output, formatProvider);
                    }
                    else
                    {
                        LogEventPropertyValues.Clear();
                        LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(LowercaseLevelMap[(int) logEvent.Level][formatLength - 1]));
                        Render(LogEventPropertyValues, output, formatProvider);
                    }

                    break;
                }

                case 'u':
                {
                    if (formatLength > 4)
                    {
                        var result = (Enum.GetName(logEvent.Level) ?? string.Empty).ToUpperInvariant();

                        LogEventPropertyValues.Clear();
                        LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(result.Length >= formatLength ? result : result[..formatLength]));
                        Render(LogEventPropertyValues, output, formatProvider);
                    }
                    else
                    {
                        LogEventPropertyValues.Clear();
                        LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(UppercaseLevelMap[(int) logEvent.Level][formatLength - 1]));
                        Render(LogEventPropertyValues, output, formatProvider);
                    }

                    break;
                }

                case 't':
                {
                    if (formatLength > 4)
                    {
                        var result = Enum.GetName(logEvent.Level) ?? string.Empty;

                        LogEventPropertyValues.Clear();
                        LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(result.Length >= formatLength ? result : result[..formatLength]));
                        Render(LogEventPropertyValues, output, formatProvider);
                    }
                    else
                    {
                        LogEventPropertyValues.Clear();
                        LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(TitleCaseLevelMap[(int) logEvent.Level][formatLength - 1]));
                        Render(LogEventPropertyValues, output, formatProvider);
                    }

                    break;
                }

                default:
                {
                    LogEventPropertyValues.Clear();
                    LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(Enum.GetName(logEvent.Level)));
                    Render(LogEventPropertyValues, output, formatProvider);
                    break;
                }
            }
        }
        else
        {
            LogEventPropertyValues.Clear();
            LogEventPropertyValues.Add(MessageTemplateToken.PropertyName,  new LiteralScalarValue(Enum.GetName(logEvent.Level)));
            Render(LogEventPropertyValues, output, formatProvider);
        }
    }
}