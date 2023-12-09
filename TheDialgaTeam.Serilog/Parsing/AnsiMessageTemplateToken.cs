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
using TheDialgaTeam.Serilog.Rendering;

namespace TheDialgaTeam.Serilog.Parsing;

public abstract class AnsiMessageTemplateToken : MessageTemplateToken
{
    protected static void Render<TMessageTemplateToken>(TMessageTemplateToken messageTemplateToken, LogEvent logEvent, TextWriter output, IFormatProvider? formatProvider = null) where TMessageTemplateToken : MessageTemplateToken
    {
        Render(messageTemplateToken, logEvent.Properties, output, formatProvider);
    }

    private static void Render<TMessageTemplateToken>(TMessageTemplateToken messageTemplateToken, IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output, IFormatProvider? formatProvider = null) where TMessageTemplateToken : MessageTemplateToken
    {
        var stringWriter = ReusableStringWriter.GetOrCreate(formatProvider);
        messageTemplateToken.Render(properties, stringWriter, formatProvider);

        var stringBuilder = stringWriter.GetStringBuilder();
        if (stringBuilder.Length == 0) return;

        AnsiEscapeCodeTextRenderer.Render(output, stringWriter.ToString());
    }

    public virtual void Render(LogEvent logEvent, TextWriter output, IFormatProvider? formatProvider = null)
    {
        Render(logEvent.Properties, output, formatProvider);
    }
}

public abstract class AnsiMessageTemplateToken<TMessageTemplateToken> : AnsiMessageTemplateToken where TMessageTemplateToken : MessageTemplateToken
{
    public override int Length => MessageTemplateToken.Length;

    protected readonly TMessageTemplateToken MessageTemplateToken;

    protected AnsiMessageTemplateToken(TMessageTemplateToken messageTemplateToken)
    {
        MessageTemplateToken = messageTemplateToken;
    }

    public override void Render(IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output, IFormatProvider? formatProvider = null)
    {
        var stringWriter = ReusableStringWriter.GetOrCreate(formatProvider);
        MessageTemplateToken.Render(properties, stringWriter, formatProvider);

        var stringBuilder = stringWriter.GetStringBuilder();
        if (stringBuilder.Length == 0) return;

        AnsiEscapeCodeTextRenderer.Render(output, stringWriter.ToString());
    }

    public override bool Equals(object? obj)
    {
        return MessageTemplateToken.Equals(obj);
    }

    public override int GetHashCode()
    {
        return MessageTemplateToken.GetHashCode();
    }

    public override string ToString()
    {
        return MessageTemplateToken.ToString() ?? string.Empty;
    }
}