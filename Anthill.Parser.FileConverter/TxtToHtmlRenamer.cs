using System;
using System.IO;
using Anthill.Parser.AzureOCR;
using Anthill.Parser.Contracts;
using Serilog;
using Unity;
using shortid;
using shortid.Configuration;
using Anthill.Parser.Models;

namespace Anthill.Parser.FileConverter
{
    public class TxtToHtmlRenamer : ITxtToHtmlRenamer
    {
        private ILogger _log;
        private Settings _settings;
        public TxtToHtmlRenamer(IUnityContainer container)
        {
            _log = container.Resolve<ILogger>();
            _settings = container.Resolve<Settings>();
        }

        public string RemaneTxtToHtml(string path)
        {
            var oldFileName = Path.GetFileName(path);
            var newfilename = Path.GetFileName(Path.ChangeExtension(path, ".html"));
            if (File.Exists(Path.Combine(_settings.TempDirectoryFullPath, newfilename)))
            {
                newfilename = ShortId.Generate(new GenerationOptions() {UseNumbers = true, UseSpecialCharacters = false }) + "_" + newfilename;
            }
            File.Copy(path, Path.Combine(_settings.TempDirectoryFullPath, newfilename));
            _log.Information($"File {oldFileName} was copied to {_settings.TempDirectoryFullPath} and renamed to {newfilename}");
            if (_settings.DeleteSourceFile)
            {
                File.Delete(path);
                _log.Information($"File deleted {path}");
            }
            return Path.Combine(_settings.TempDirectoryFullPath, newfilename);
        }
    }
}
