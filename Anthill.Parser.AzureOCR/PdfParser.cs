using System;
using System.IO;
using System.Linq;
using Anthill.Parser.Contracts;
using Unity;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Threading.Tasks;
using System.Collections.Generic;
using Anthill.Parser.Models;
namespace Anthill.Parser.AzureOCR
{

    public class PdfParser : IPdfParser
    {
        private AzureKeyCredential _credential;
        private DocumentAnalysisClient _client;

        private IUnityContainer _container;
        private Settings _settings;

        private List<ParsedDocument> _documents = new();
        public PdfParser(IUnityContainer container)
        {
            _container = container;
            _settings = container.Resolve<Settings>();
            _credential = new AzureKeyCredential(_settings.ApiKey);
            _client = new DocumentAnalysisClient(new Uri(_settings.ApiEndpoint), _credential);
        }

        public async Task<List<ParsedDocument>> StarParseAllPdfs()
        {
            foreach (var file in Directory.GetFiles(_settings.TempDirectoryFullPath).Where(x => Path.GetExtension(x) == ".pdf").ToArray())
            {
                var parsedDocument = await ParsePdf(file);
                parsedDocument = _container.Resolve<IPostProcessor>().ProcessDocuments(parsedDocument);
                _container.Resolve<IOutput>().DisplayFields(parsedDocument);
                _documents.Add(parsedDocument);

            }
            return _documents;
        }

        private async Task<ParsedDocument> ParsePdf(string path)
        {
            AnalyzeDocumentOperation operation = null;
            using (FileStream fs = new(path, FileMode.Open))
            {
                operation = await _client.StartAnalyzeDocumentAsync(_settings.ModelId, fs);
            }

            await operation.WaitForCompletionAsync();

            AnalyzeResult result = operation.Value;

            return ParsedDocument.Create(_settings, result, path);

        }
    }
}
