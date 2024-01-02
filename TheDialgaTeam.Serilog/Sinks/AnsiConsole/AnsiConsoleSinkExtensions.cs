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
using Serilog;
using Serilog.Configuration;
using TheDialgaTeam.Serilog.Formatting;
using TheDialgaTeam.Serilog.Native;

namespace TheDialgaTeam.Serilog.Sinks.AnsiConsole;

public static class AnsiConsoleSinkExtensions
{
    public static LoggerConfiguration AnsiConsoleSink(this LoggerSinkConfiguration sinkConfiguration, IFormatProvider? formatProvider = null)
    {
        SetAnsiConsole();
        return sinkConfiguration.Async(configuration => configuration.Sink(new AnsiConsoleSink(new LogLevelMessageTemplateOptions(), formatProvider)));
    }
    
    public static LoggerConfiguration AnsiConsoleSink(this LoggerSinkConfiguration sinkConfiguration, LogLevelMessageTemplateOptions options, IFormatProvider? formatProvider = null)
    {
        SetAnsiConsole();
        return sinkConfiguration.Async(configuration => configuration.Sink(new AnsiConsoleSink(options, formatProvider)));
    }
    
    public static LoggerConfiguration AnsiConsoleSink(this LoggerSinkConfiguration sinkConfiguration, Action<LogLevelMessageTemplateOptionsBuilder> options, IFormatProvider? formatProvider = null)
    {
        SetAnsiConsole();

        var optionsBuilder = new LogLevelMessageTemplateOptionsBuilder();
        options(optionsBuilder);
        
        return sinkConfiguration.Async(configuration => configuration.Sink(new AnsiConsoleSink(optionsBuilder.Build(), formatProvider)));
    }

    private static void SetAnsiConsole()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        var stdout = WindowsConsoleNative.GetStdHandle(WindowsConsoleNative.StandardOutputHandleId);

        if (stdout != WindowsConsoleNative.InvalidHandleValue && WindowsConsoleNative.GetConsoleMode(stdout, out var stdoutMode))
        {
            WindowsConsoleNative.SetConsoleMode(stdout, stdoutMode | WindowsConsoleNative.EnableVirtualTerminalProcessingMode);
        }

        var stdError = WindowsConsoleNative.GetStdHandle(WindowsConsoleNative.StandardErrorHandleId);

        if (stdError != WindowsConsoleNative.InvalidHandleValue && WindowsConsoleNative.GetConsoleMode(stdError, out var stdErrorMode))
        {
            WindowsConsoleNative.SetConsoleMode(stdError, stdErrorMode | WindowsConsoleNative.EnableVirtualTerminalProcessingMode);
        }
    }
}