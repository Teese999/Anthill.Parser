using System;
using System.Collections.Generic;
using Anthill.Parser.Models;

namespace Anthill.Parser.Contracts
{
    public interface IOutput
    {
        public void DisplayFields(ParsedDocument parsedDocument);
        public void SaveToFile(List<ParsedDocument> parsedDocuments, string Path);
    }
}
