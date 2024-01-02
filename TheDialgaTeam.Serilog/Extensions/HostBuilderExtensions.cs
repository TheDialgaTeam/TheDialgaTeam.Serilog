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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using TheDialgaTeam.Serilog.Configuration;

namespace TheDialgaTeam.Serilog.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder, Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> configureLogger)
    {
        return hostBuilder.ConfigureServices(static collection =>
        {
            collection.AddOptions<LogLevelOptions>().BindConfiguration("TheDialgaTeam.Serilog:LogLevel");
            collection.TryAddSingleton<SerilogLoggerSettings>();
        }).UseSerilog((context, provider, configuration) =>
        {
            configuration.ReadFrom.Settings(provider.GetRequiredService<SerilogLoggerSettings>());
            configureLogger(context, provider, configuration);
        });
    }
    
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder, string logLevelConfigSection, Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> configureLogger)
    {
        return hostBuilder.ConfigureServices(collection =>
        {
            collection.AddOptions<LogLevelOptions>().BindConfiguration(logLevelConfigSection);
            collection.TryAddSingleton<SerilogLoggerSettings>();
        }).UseSerilog((context, provider, configuration) =>
        {
            configuration.ReadFrom.Settings(provider.GetRequiredService<SerilogLoggerSettings>());
            configureLogger(context, provider, configuration);
        });
    }
}