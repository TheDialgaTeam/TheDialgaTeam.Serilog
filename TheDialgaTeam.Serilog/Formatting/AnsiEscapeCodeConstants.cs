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

namespace TheDialgaTeam.Serilog.Formatting;

public static class AnsiEscapeCodeConstants
{
    public const string BlackForegroundColor = "\u001b[30m";
    public const string BlackBackgroundColor = "\u001b[40m";

    public const string DarkRedForegroundColor = "\u001b[31m";
    public const string DarkRedBackgroundColor = "\u001b[41m";

    public const string DarkGreenForegroundColor = "\u001b[32m";
    public const string DarkGreenBackgroundColor = "\u001b[42m";

    public const string DarkYellowForegroundColor = "\u001b[33m";
    public const string DarkYellowBackgroundColor = "\u001b[43m";

    public const string DarkBlueForegroundColor = "\u001b[34m";
    public const string DarkBlueBackgroundColor = "\u001b[44m";

    public const string DarkMagentaForegroundColor = "\u001b[35m";
    public const string DarkMagentaBackgroundColor = "\u001b[45m";

    public const string DarkCyanForegroundColor = "\u001b[36m";
    public const string DarkCyanBackgroundColor = "\u001b[46m";

    public const string DarkGrayForegroundColor = "\u001b[37m";
    public const string DarkGrayBackgroundColor = "\u001b[47m";

    public const string GrayForegroundColor = "\u001b[90m";
    public const string GrayBackgroundColor = "\u001b[100m";

    public const string RedForegroundColor = "\u001b[91m";
    public const string RedBackgroundColor = "\u001b[101m";

    public const string GreenForegroundColor = "\u001b[92m";
    public const string GreenBackgroundColor = "\u001b[102m";

    public const string YellowForegroundColor = "\u001b[93m";
    public const string YellowBackgroundColor = "\u001b[103m";

    public const string BlueForegroundColor = "\u001b[94m";
    public const string BlueBackgroundColor = "\u001b[104m";

    public const string MagentaForegroundColor = "\u001b[95m";
    public const string MagentaBackgroundColor = "\u001b[105m";

    public const string CyanForegroundColor = "\u001b[96m";
    public const string CyanBackgroundColor = "\u001b[106m";

    public const string WhiteForegroundColor = "\u001b[97m";
    public const string WhiteBackgroundColor = "\u001b[107m";

    public const string Reset = "\u001b[0m";

    public const string Bold = "\u001b[1m";
    public const string Underline = "\u001b[4m";
    public const string Reverse = "\u001b[7m";
    public const string NoUnderline = "\u001b[24m";
}