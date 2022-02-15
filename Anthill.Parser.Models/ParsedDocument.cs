using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace Anthill.Parser.Models
{
    public class ParsedDocument
    {
        public Dictionary<string, string> Fields { get; set; } = new();
        public double AvgAccuracy { get; set; }
        public bool NeedHandChek { get; private set; } = false;
        public string Path { get; set; }

        public static ParsedDocument Create(Settings settings, AnalyzeResult result, string path)
        {
            var document = new ParsedDocument();
            document.Path = path;
            foreach (var fieldName in settings.ModelFields) { document.Fields.Add(fieldName, null);}
            AnalyzedDocument analyzedDocument = null;
            foreach (var doc in result.Documents)
            {
                var resultDocumentFields = doc.Fields.Keys.ToArray();

                int fieldsCount = settings.ModelFields.Length;
                int validFields = 0;

                foreach (var item in document.Fields.Keys)
                {
                    if (resultDocumentFields.Contains(item))
                    {
                        validFields++;
                    }
                }
                if (validFields == fieldsCount)
                {
                    analyzedDocument = doc;
                    break;
                }
            }
            if (analyzedDocument == null)
            {
                return null;
            }
            document.AvgAccuracy = analyzedDocument.Confidence;
            if (document.AvgAccuracy < settings.ValidAccuracy)
            {
                document.NeedHandChek = true;
            }
            foreach (var fieldName in document.Fields.Keys.ToList())
            {
                document.Fields[fieldName] = analyzedDocument.Fields.Where(x => x.Key == fieldName).First().Value.Content;
            }
            return document;
        }
    }
}
