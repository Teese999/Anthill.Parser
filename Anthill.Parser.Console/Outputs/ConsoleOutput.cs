using System;
using System.Collections.Generic;
using Anthill.Parser.Contracts;
using Anthill.Parser.Models;

namespace Anthill.Parser.Console.Outputs
{
    public class ConsoleOutput : IOutput
    {

        public void DisplayFields(ParsedDocument parsedDocument)
        {
            System.Console.WriteLine();
            System.Console.WriteLine(parsedDocument.Path);
            foreach (var item in parsedDocument.Fields)
            {
                System.Console.Write($"{item.Key}: {item.Value}");
                System.Console.Write("\t");
            }
        }

        public void SaveToFile(List<ParsedDocument> parsedDocuments, string Path)
        {
            throw new NotImplementedException();
        }
    }
}
