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

using System.Globalization;
using Serilog.Events;

namespace TheDialgaTeam.Serilog.Events;

internal sealed class LiteralScalarValue(object? value) : ScalarValue(value)
{
    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        switch (Value)
        {
            case null:
                output.Write("null");
                return;

            case string s:
                output.Write(s);
                return;
        }

        if (formatProvider?.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter customFormatter)
        {
            output.Write(customFormatter.Format(format, Value, formatProvider));
            return;
        }

        if (Value is IFormattable f)
        {
            output.Write(f.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
        }
        else
        {
            output.Write(Value.ToString());
        }
    }
}