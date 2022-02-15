using System.Collections.Generic;

namespace Anthill.Parser.Models
{
    public class ParsedDocument
    {
        public Dictionary<string, string> Fields { get; set; } = new();
        public double AvgAccuracy { get; set; }
        public bool NeedHandChek { get; set; } = false;
        public string Path { get; set; }
    }
}
