﻿using System;
using System.CommandLine;
using System.CommandLine.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotVVM.Tool
{
    public static class Program
    {
        public const string VerboseAlias = "--verbose";

        public static ILoggerFactory Logging = new NullLoggerFactory();

        public static int Main(string[] args)
        {
            var rootCmd = new RootCommand("DotVVM Command-line Interface");
            Compiler.AddCompiler(rootCmd);
            SeleniumGenerator.AddSeleniumGenerator(rootCmd);
            rootCmd.AddGlobalOption(new Option<bool>(
                aliases: new [] {"-v", VerboseAlias},
                description: "Show more verbose output"));
            var builder = new CommandLineBuilder(rootCmd)
                .UseDefaults()
                .UseMiddleware(c =>
                {
                    var logLevel = c.ParseResult.ValueForOption<bool>(VerboseAlias)
                        ? LogLevel.Debug
                        : LogLevel.Information;
                    Logging = LoggerFactory.Create(b => b.AddConsole().SetMinimumLevel(logLevel));
                })
                .Build();

            using(Logging)
            {
                return rootCmd.Invoke(args);
            }
        }
    }
}