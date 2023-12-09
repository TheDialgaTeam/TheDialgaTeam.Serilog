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

using System.Text.Json;
using Serilog.Events;
using Serilog.Parsing;
using TheDialgaTeam.Serilog.Rendering;

namespace TheDialgaTeam.Serilog.Parsing;

internal sealed class AnsiPropertyToken : AnsiMessageTemplateToken<PropertyToken>
{
    public AnsiPropertyToken(PropertyToken propertyToken) : base(propertyToken)
    {
    }

    public static void Render(PropertyToken propertyToken, LogEvent logEvent, TextWriter output, IFormatProvider? formatProvider = null, bool isLiteral = false, bool isJson = false)
    {
        if (!logEvent.Properties.TryGetValue(propertyToken.PropertyName, out var propertyValue)) return;

        if (isLiteral && propertyValue is ScalarValue { Value: string value })
        {
            AnsiEscapeCodeTextRenderer.Render(output, value);
        }
        else if (isJson && propertyToken.Format is null)
        {
            RenderJson(output, propertyValue);
        }
        else
        {
            var stringWriter = ReusableStringWriter.GetOrCreate(formatProvider);
            propertyToken.Render(logEvent.Properties, stringWriter, formatProvider);

            var stringBuilder = stringWriter.GetStringBuilder();
            if (stringBuilder.Length == 0) return;

            AnsiEscapeCodeTextRenderer.Render(output, stringWriter.ToString());
        }
    }

    private static void RenderJson(TextWriter output, LogEventPropertyValue propertyValue)
    {
        switch (propertyValue)
        {
            case ScalarValue scalarValue:
            {
                output.Write(JsonSerializer.Serialize(scalarValue));
                break;
            }

            case SequenceValue sequenceValue:
            {
                output.Write("[");

                char? delim = null;

                foreach (var element in sequenceValue.Elements)
                {
                    if (delim is not null)
                    {
                        output.Write(",");
                    }

                    delim = ',';
                    RenderJson(output, element);
                }

                output.Write("]");
                break;
            }

            case StructureValue structureValue:
            {
                output.Write("{");

                char? delim = null;

                foreach (var property in structureValue.Properties)
                {
                    if (delim is not null)
                    {
                        output.Write(",");
                    }

                    delim = ',';

                    output.Write($"\"{property.Name}\"");
                    output.Write(":");

                    RenderJson(output, property.Value);
                }

                output.Write("}");
                break;
            }

            case DictionaryValue dictionaryValue:
            {
                output.Write("{");

                char? delim = null;

                foreach (var element in dictionaryValue.Elements)
                {
                    if (delim is not null)
                    {
                        output.Write(",");
                    }

                    delim = ',';

                    if (element.Key.Value is string key)
                    {
                        output.Write($"\"{key}\"");
                    }
                    else
                    {
                        output.Write($"\"{element.Key.Value?.ToString() ?? "null"}\"");
                    }

                    output.Write(":");
                    RenderJson(output, element.Value);
                }

                output.Write("}");
                break;
            }
        }
    }
}