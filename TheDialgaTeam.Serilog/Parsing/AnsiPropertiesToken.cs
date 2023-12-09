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

namespace TheDialgaTeam.Serilog.Parsing;

internal sealed class AnsiPropertiesToken : AnsiMessageTemplateToken<PropertyToken>
{
    private readonly MessageTemplate _messageTemplate;

    public AnsiPropertiesToken(PropertyToken propertyToken, MessageTemplate messageTemplate) : base(propertyToken)
    {
        _messageTemplate = messageTemplate;
    }

    private static bool TemplateContainsPropertyName(MessageTemplate template, string propertyName)
    {
        var templateTokens = template.Tokens;

        foreach (var templateToken in templateTokens)
        {
            if (templateToken is PropertyToken propertyToken && propertyToken.PropertyName == propertyName)
            {
                return true;
            }
        }

        return false;
    }

    public override void Render(LogEvent logEvent, TextWriter output, IFormatProvider? formatProvider = null)
    {
        var propertiesToInclude = logEvent.Properties
            .Where(pair => !TemplateContainsPropertyName(logEvent.MessageTemplate, pair.Key) && !TemplateContainsPropertyName(_messageTemplate, pair.Key))
            .Select(pair => new LogEventProperty(pair.Key, pair.Value));

        Render(new Dictionary<string, LogEventPropertyValue>
        {
            { MessageTemplateToken.PropertyName, new StructureValue(propertiesToInclude) }
        }, output, formatProvider);
    }
}