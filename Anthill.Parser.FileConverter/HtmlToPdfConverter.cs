using System;
using System.IO;
using Anthill.Parser.Contracts;
using PdfSharpCore.Pdf;
using Syncfusion.HtmlConverter;
using Unity;
using Anthill.Parser.AzureOCR;
using Serilog;
using shortid.Configuration;
using shortid;
using Anthill.Parser.Models;

namespace Anthill.Parser.FileConverter
{
    public class HtmlToPdfConverter : IHtmlToPdfConverter
    {
        private IUnityContainer _container;
        private ILogger _log;
        private Settings _settings;

        private static Syncfusion.HtmlConverter.HtmlToPdfConverter _htmlConverter = new Syncfusion.HtmlConverter.HtmlToPdfConverter(HtmlRenderingEngine.Blink);
        private static BlinkConverterSettings _blinkConverterSettings = new BlinkConverterSettings();

        public HtmlToPdfConverter(IUnityContainer container)
        {
            _container = container;
            _log = _container.Resolve<ILogger>();
            _settings = _container.Resolve<Settings>();

            _blinkConverterSettings.BlinkPath =Directory.GetCurrentDirectory();
            _htmlConverter.ConverterSettings = _blinkConverterSettings;
        }

        public void ConvertHtmlToPdf(string path)
        {
            string html = File.ReadAllText(path);
            var oldFileName = Path.GetFileName(path);
            var newfilename = Path.GetFileName(Path.ChangeExtension(path, ".pdf"));
            if (File.Exists(Path.Combine(_settings.TempDirectoryFullPath, newfilename)))
            {
                newfilename = ShortId.Generate(new GenerationOptions() { UseNumbers = true, UseSpecialCharacters = false }) + "_" + newfilename;
            }

            Syncfusion.Pdf.PdfDocument document = null;
            try
            {
                document = _htmlConverter.Convert(Path.GetFullPath(path));
            }
            catch (Syncfusion.Pdf.PdfException ex)
            {
                _log.Error($"{ex} \n {path}");
                if (!Directory.Exists("ConvertException"))
                {
                    Directory.CreateDirectory("ConvertException");
                }
                File.Copy(path, Path.Combine("ConvertException", ShortId.Generate(new GenerationOptions() { UseNumbers = true, UseSpecialCharacters = false }) + "_" + oldFileName));
                return;
            }
            FileStream fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(),_settings.TempDirectoryName, newfilename), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _log.Information($"Created file {newfilename} in directory {_settings.TempDirectoryFullPath}");
            document.Save(fileStream);
            document.Close(true);
            
            if (_container.Resolve<Settings>().DeleteSourceFile)
            {
                File.Delete(path);
                _log.Information($"File deleted {path}");
            }
        }
    }
}
