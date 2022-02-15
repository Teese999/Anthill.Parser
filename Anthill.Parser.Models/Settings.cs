using System;
using System.IO;

namespace Anthill.Parser.Models
{
    public class Settings
    {
        public string ApiKey { get; set; }
        public string ApiEndpoint { get; set; }
        public string TempDirectoryName { get; set; }
        public string ModelId { get; set; }
        public double ValidAccuracy { get; set; }
        public string[] ModelFields { get; set; }
        public bool DeleteSourceFile { get; set; }
        public string TempDirectoryFullPath { get {return Path.Combine(Directory.GetCurrentDirectory(), TempDirectoryName);  }}
        public bool CopyHendchakeFilesToFolder { get; set; }
        public string HandchackFolderName { get; set; }
        public bool DeleteTempFiles { get; set; }
    }
}
