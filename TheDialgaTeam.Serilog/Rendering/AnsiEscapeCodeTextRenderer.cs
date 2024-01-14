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

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TheDialgaTeam.Serilog.Formatting;
using TheDialgaTeam.Serilog.Native;

namespace TheDialgaTeam.Serilog.Rendering;

internal static partial class AnsiEscapeCodeTextRenderer
{
    private const string AnsiEscapeRegex = "((?:\\u001b\\[[0-9:;<=>?]*[\\s!\"#$%&'()*+,\\-./]*[@A-Z[\\]^_`a-z{|}~])|(?:\\u001b[@A-Z\\[\\]^_]))";

    private static readonly Dictionary<TextWriter, bool> IsAnsiEscapeCodeSupported = new();

    private static readonly ConsoleColor DefaultForegroundColor;
    private static readonly ConsoleColor DefaultBackgroundColor;

    static AnsiEscapeCodeTextRenderer()
    {
        DefaultForegroundColor = Console.ForegroundColor;
        DefaultBackgroundColor = Console.BackgroundColor;
    }

    public static void Render(TextWriter output, string text)
    {
        if (!IsAnsiEscapeCodeSupported.TryGetValue(output, out var isSupported))
        {
            if (output == Console.Out && !Console.IsOutputRedirected)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (WindowsConsoleNative.GetConsoleMode(WindowsConsoleNative.StandardOutputHandleId, out var mode))
                    {
                        isSupported = (mode & WindowsConsoleNative.EnableVirtualTerminalProcessingMode) == WindowsConsoleNative.EnableVirtualTerminalProcessingMode;
                    }
                }
                else
                {
                    isSupported = true;
                }
            }
            else if (output == Console.Error && !Console.IsErrorRedirected)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (WindowsConsoleNative.GetConsoleMode(WindowsConsoleNative.StandardErrorHandleId, out var mode))
                    {
                        isSupported = (mode & WindowsConsoleNative.EnableVirtualTerminalProcessingMode) == WindowsConsoleNative.EnableVirtualTerminalProcessingMode;
                    }
                }
                else
                {
                    isSupported = true;
                }
            }
            else
            {
                isSupported = false;
            }

            IsAnsiEscapeCodeSupported.Add(output, isSupported);
        }

        if (isSupported)
        {
            output.Write(text);
        }
        else
        {
            var ansiTokens = AnsiEscapeCodeRegex().Matches(text);

            if (ansiTokens.Count == 0)
            {
                // There is no ansi tokens so we can safely write the whole thing
                output.Write(text);
                return;
            }

            var currentText = text.AsSpan();
            var currentIndex = 0;

            foreach (Match ansiToken in ansiTokens)
            {
                if (currentIndex < ansiToken.Index)
                {
                    output.Write(currentText[currentIndex..ansiToken.Index]);
                }

                if ((output == Console.Out && !Console.IsOutputRedirected) || (output == Console.Error && !Console.IsErrorRedirected))
                {
                    switch (ansiToken.Value)
                    {
                        case AnsiEscapeCodeConstants.BlackForegroundColor:
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;

                        case AnsiEscapeCodeConstants.DarkRedForegroundColor:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            break;

                        case AnsiEscapeCodeConstants.DarkGreenForegroundColor:
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            break;

                        case AnsiEscapeCodeConstants.DarkYellowForegroundColor:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;

                        case AnsiEscapeCodeConstants.DarkBlueForegroundColor:
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            break;

                        case AnsiEscapeCodeConstants.DarkMagentaForegroundColor:
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            break;

                        case AnsiEscapeCodeConstants.DarkCyanForegroundColor:
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            break;

                        case AnsiEscapeCodeConstants.DarkGrayForegroundColor:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            break;

                        case AnsiEscapeCodeConstants.GrayForegroundColor:
                        case "\u001b[30;1m":
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;

                        case AnsiEscapeCodeConstants.RedForegroundColor:
                        case "\u001b[31;1m":
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;

                        case AnsiEscapeCodeConstants.GreenForegroundColor:
                        case "\u001b[32;1m":
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;

                        case AnsiEscapeCodeConstants.YellowForegroundColor:
                        case "\u001b[33;1m":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;

                        case AnsiEscapeCodeConstants.BlueForegroundColor:
                        case "\u001b[34;1m":
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;

                        case AnsiEscapeCodeConstants.MagentaForegroundColor:
                        case "\u001b[35;1m":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;

                        case AnsiEscapeCodeConstants.CyanForegroundColor:
                        case "\u001b[36;1m":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;

                        case AnsiEscapeCodeConstants.WhiteForegroundColor:
                        case "\u001b[37;1m":
                            Console.ForegroundColor = ConsoleColor.White;
                            break;

                        case AnsiEscapeCodeConstants.BlackBackgroundColor:
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;

                        case AnsiEscapeCodeConstants.DarkRedBackgroundColor:
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            break;

                        case AnsiEscapeCodeConstants.DarkGreenBackgroundColor:
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            break;

                        case AnsiEscapeCodeConstants.DarkYellowBackgroundColor:
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                            break;

                        case AnsiEscapeCodeConstants.DarkBlueBackgroundColor:
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            break;

                        case AnsiEscapeCodeConstants.DarkMagentaBackgroundColor:
                            Console.BackgroundColor = ConsoleColor.DarkMagenta;
                            break;

                        case AnsiEscapeCodeConstants.DarkCyanBackgroundColor:
                            Console.BackgroundColor = ConsoleColor.DarkCyan;
                            break;

                        case AnsiEscapeCodeConstants.DarkGrayBackgroundColor:
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            break;

                        case AnsiEscapeCodeConstants.GrayBackgroundColor:
                        case "\u001b[40;1m":
                            Console.BackgroundColor = ConsoleColor.Gray;
                            break;

                        case AnsiEscapeCodeConstants.RedBackgroundColor:
                        case "\u001b[41;1m":
                            Console.BackgroundColor = ConsoleColor.Red;
                            break;

                        case AnsiEscapeCodeConstants.GreenBackgroundColor:
                        case "\u001b[42;1m":
                            Console.BackgroundColor = ConsoleColor.Green;
                            break;

                        case AnsiEscapeCodeConstants.YellowBackgroundColor:
                        case "\u001b[43;1m":
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            break;

                        case AnsiEscapeCodeConstants.BlueBackgroundColor:
                        case "\u001b[44;1m":
                            Console.BackgroundColor = ConsoleColor.Blue;
                            break;

                        case AnsiEscapeCodeConstants.MagentaBackgroundColor:
                        case "\u001b[45;1m":
                            Console.BackgroundColor = ConsoleColor.Magenta;
                            break;

                        case AnsiEscapeCodeConstants.CyanBackgroundColor:
                        case "\u001b[46;1m":
                            Console.BackgroundColor = ConsoleColor.Cyan;
                            break;

                        case AnsiEscapeCodeConstants.WhiteBackgroundColor:
                        case "\u001b[47;1m":
                            Console.BackgroundColor = ConsoleColor.White;
                            break;

                        case AnsiEscapeCodeConstants.Reset:
                            Console.ForegroundColor = DefaultForegroundColor;
                            Console.BackgroundColor = DefaultBackgroundColor;
                            break;
                    }
                }

                currentIndex = ansiToken.Index + ansiToken.Length;
            }

            if (currentIndex < currentText.Length)
            {
                output.Write(currentText[currentIndex..]);
            }
        }
    }

    [GeneratedRegex(AnsiEscapeRegex)]
    private static partial Regex AnsiEscapeCodeRegex();
}