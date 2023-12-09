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

using System.Collections.Concurrent;
using Serilog.Formatting.Display;
using Serilog.Parsing;

namespace TheDialgaTeam.Serilog.Parsing;

internal sealed class AnsiTemplateTextParser
{
    private readonly MessageTemplateParser _messageTemplateParser = new();
    private readonly ConcurrentDictionary<string, AnsiMessageTemplateToken[]> _textFormatters = new();

    public AnsiMessageTemplateToken[] GetMessageTemplateTokens(string messageTemplateFormat)
    {
        return _textFormatters.GetOrAdd(messageTemplateFormat, static (key, args) => args.GenerateMessageTemplateTokens(key).ToArray(), this);
    }

    private IEnumerable<AnsiMessageTemplateToken> GenerateMessageTemplateTokens(string messageTemplateFormat)
    {
        var messageTemplate = _messageTemplateParser.Parse(messageTemplateFormat);

        foreach (var token in messageTemplate.Tokens)
        {
            switch (token)
            {
                case TextToken textToken:
                {
                    yield return new AnsiTextToken(textToken);
                    break;
                }

                case PropertyToken propertyToken:
                {
                    yield return propertyToken.PropertyName switch
                    {
                        OutputProperties.MessagePropertyName => new AnsiMessageToken(propertyToken),
                        OutputProperties.TimestampPropertyName => new AnsiTimestampToken(propertyToken),
                        OutputProperties.LevelPropertyName => new AnsiLevelToken(propertyToken),
                        OutputProperties.TraceIdPropertyName => new AnsiTraceIdToken(propertyToken),
                        OutputProperties.SpanIdPropertyName => new AnsiSpanIdToken(propertyToken),
                        OutputProperties.NewLinePropertyName => new AnsiNewLineToken(propertyToken),
                        OutputProperties.ExceptionPropertyName => new AnsiExceptionToken(propertyToken),
                        OutputProperties.PropertiesPropertyName => new AnsiPropertiesToken(propertyToken, messageTemplate),
                        var _ => new AnsiPropertyToken(propertyToken)
                    };

                    break;
                }
            }
        }
    }
}