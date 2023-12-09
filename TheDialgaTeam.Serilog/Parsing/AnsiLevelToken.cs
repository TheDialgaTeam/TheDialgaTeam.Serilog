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

internal sealed class AnsiLevelToken : AnsiMessageTemplateToken<PropertyToken>
{
    private static readonly string[][] TitleCaseLevelMap =
    {
        new[] { "V", "Vb", "Vrb", "Verb" },
        new[] { "D", "De", "Dbg", "Dbug" },
        new[] { "I", "In", "Inf", "Info" },
        new[] { "W", "Wn", "Wrn", "Warn" },
        new[] { "E", "Er", "Err", "Eror" },
        new[] { "F", "Fa", "Ftl", "Fatl" }
    };

    private static readonly string[][] LowercaseLevelMap =
    {
        new[] { "v", "vb", "vrb", "verb" },
        new[] { "d", "de", "dbg", "dbug" },
        new[] { "i", "in", "inf", "info" },
        new[] { "w", "wn", "wrn", "warn" },
        new[] { "e", "er", "err", "eror" },
        new[] { "f", "fa", "ftl", "fatl" }
    };

    private static readonly string[][] UppercaseLevelMap =
    {
        new[] { "V", "VB", "VRB", "VERB" },
        new[] { "D", "DE", "DBG", "DBUG" },
        new[] { "I", "IN", "INF", "INFO" },
        new[] { "W", "WN", "WRN", "WARN" },
        new[] { "E", "ER", "ERR", "EROR" },
        new[] { "F", "FA", "FTL", "FATL" }
    };

    public AnsiLevelToken(PropertyToken propertyToken) : base(propertyToken)
    {
    }

    public override void Render(LogEvent logEvent, TextWriter output, IFormatProvider? formatProvider = null)
    {
        var formatStringSpan = (MessageTemplateToken.Format ?? string.Empty).AsSpan();

        if (formatStringSpan.Length < 2)
        {
            Render(new Dictionary<string, LogEventPropertyValue>
            {
                { MessageTemplateToken.PropertyName, new LiteralScalarValue(Enum.GetName(logEvent.Level)) }
            }, output, formatProvider);
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

                        Render(new Dictionary<string, LogEventPropertyValue>
                        {
                            { MessageTemplateToken.PropertyName, new LiteralScalarValue(result.Length >= formatLength ? result : result[..formatLength]) }
                        }, output, formatProvider);
                    }
                    else
                    {
                        Render(new Dictionary<string, LogEventPropertyValue>
                        {
                            { MessageTemplateToken.PropertyName, new LiteralScalarValue(LowercaseLevelMap[(int) logEvent.Level][formatLength - 1]) }
                        }, output, formatProvider);
                    }

                    break;
                }

                case 'u':
                {
                    if (formatLength > 4)
                    {
                        var result = (Enum.GetName(logEvent.Level) ?? string.Empty).ToUpperInvariant();

                        Render(new Dictionary<string, LogEventPropertyValue>
                        {
                            { MessageTemplateToken.PropertyName, new LiteralScalarValue(result.Length >= formatLength ? result : result[..formatLength]) }
                        }, output, formatProvider);
                    }
                    else
                    {
                        Render(new Dictionary<string, LogEventPropertyValue>
                        {
                            { MessageTemplateToken.PropertyName, new LiteralScalarValue(UppercaseLevelMap[(int) logEvent.Level][formatLength - 1]) }
                        }, output, formatProvider);
                    }

                    break;
                }

                case 't':
                {
                    if (formatLength > 4)
                    {
                        var result = Enum.GetName(logEvent.Level) ?? string.Empty;

                        Render(new Dictionary<string, LogEventPropertyValue>
                        {
                            { MessageTemplateToken.PropertyName, new LiteralScalarValue(result.Length >= formatLength ? result : result[..formatLength]) }
                        }, output, formatProvider);
                    }
                    else
                    {
                        Render(new Dictionary<string, LogEventPropertyValue>
                        {
                            { MessageTemplateToken.PropertyName, new LiteralScalarValue(TitleCaseLevelMap[(int) logEvent.Level][formatLength - 1]) }
                        }, output, formatProvider);
                    }

                    break;
                }

                default:
                {
                    Render(new Dictionary<string, LogEventPropertyValue>
                    {
                        { MessageTemplateToken.PropertyName, new LiteralScalarValue(Enum.GetName(logEvent.Level)) }
                    }, output, formatProvider);
                    break;
                }
            }
        }
        else
        {
            Render(new Dictionary<string, LogEventPropertyValue>
            {
                { MessageTemplateToken.PropertyName, new LiteralScalarValue(Enum.GetName(logEvent.Level)) }
            }, output, formatProvider);
        }
    }
}