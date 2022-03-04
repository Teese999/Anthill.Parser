using System;
using System.Linq;
using Anthill.Parser.Models;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.FormRecognizer.Models;

namespace Anthill.Parser.AzureOCR
{
    public class CommonService
    {
        public static ParsedDocument AzureOCRCreateParsedDocument(Settings settings, RecognizedFormCollection result, string path)
        {
            var document = new ParsedDocument();
            document.Path = path;
            foreach (var fieldName in settings.ModelFields) { document.Fields.Add(fieldName, null); }
            RecognizedForm analyzedDocument = null;
            foreach (var doc in result)
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
            foreach (var field in analyzedDocument.Fields)
            {
                document.AvgAccuracy += field.Value.Confidence;
            }
            document.AvgAccuracy /= analyzedDocument.Fields.Count;
            if (document.AvgAccuracy < settings.ValidAccuracy)
            {
                document.NeedHandChek = true;
            }
            foreach (var fieldName in document.Fields.Keys.ToList())
            {
                var cc = fieldName;
                document.Fields[fieldName] = analyzedDocument.Fields.Where(x => x.Key == fieldName).First().Value.ValueData.Text;
            }
            return document;
        }
    }
}
