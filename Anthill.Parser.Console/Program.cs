using System;
using Microsoft.Extensions.Configuration;
using Anthill.Parser.AzureOCR;
using System.IO;
using Unity;
using Serilog;
using Unity.Lifetime;
using System.Linq;
using System.Collections.Generic;
using Anthill.Parser.Contracts;
using System.Threading.Tasks;
using Anthill.Parser.Models;

namespace Anthill.Parser.Console
{
    class Program
    {
        private static IUnityContainer _container = new UnityContainer();
        private static Settings _settings;
        private static ILogger _log;
        
        static async Task Main(string[] args)
        {
            Configure();           

            if (args.Any())
            {
                string path = args[0];
                if (Path.GetExtension(path) == "")
                {
                    FolderPrepare(path);
                }
                else
                {
                    FileConvertToPdf(path);
                }

                var parsedDocuments = await _container.Resolve<PdfParser>().StarParseAllPdfs();

                if (_settings.DeleteTempFiles)
                {
                    File.Delete(_settings.TempDirectoryFullPath);
                }
                if (Directory.GetFiles("ConvertException").Length > 0)
                {
                    _log.Warning($"Check folder {Path.GetFullPath("ConvertException")} !!!");
                }
            }
            else
            {
                Log.Information("No args, Application closed");
            }
        }

        private static void Configure()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();


            _settings = builder.GetSection("Settings").Get<Settings>();
            _container.RegisterInstance(_settings);

            _log = Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _container.RegisterInstance(_log);

            var log = _container.Resolve<ILogger>();

            log.Information("Application Started");

            ContainerConfiguration.RegisterTypes<HierarchicalLifetimeManager>(_container);


            if (!Directory.Exists(_settings.TempDirectoryFullPath))
            {
                Directory.CreateDirectory(_settings.TempDirectoryFullPath);
            }
            if (!Directory.Exists("ConvertException"))
            {
                Directory.CreateDirectory("ConvertException");
            }
            if (_settings.CopyHendchakeFilesToFolder)
            {
                if (!Directory.Exists(_settings.HandchackFolderName))
                {
                    Directory.CreateDirectory(_settings.HandchackFolderName);
                }           
            }
        }
        private static void FolderPrepare(string path)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToList();

            foreach (var file in files)
            {
                FileConvertToPdf(file);
                
            }
        }
        private static void FileConvertToPdf(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".txt":
                    path = _container.Resolve<ITxtToHtmlRenamer>().RemaneTxtToHtml(path);
                    _container.Resolve<IHtmlToPdfConverter>().ConvertHtmlToPdf(path);
                    break;
                case ".html":
                    _container.Resolve<IHtmlToPdfConverter>().ConvertHtmlToPdf(path);
                    break;
                case ".pdf":
                    File.Copy(path, Path.Combine(_settings.TempDirectoryFullPath, Path.GetFileName(path)));
                    _log.Information($"File {Path.GetFileName(path)} was copied to {_settings.TempDirectoryFullPath}");
                    if (_settings.DeleteSourceFile)
                    {
                        File.Delete(path);
                        _log.Information($"File {path} was deleted");
                    }
                    break;
                default:
                    break;
            }
        }
    }

}
