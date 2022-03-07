using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anthill.Parser.Contracts;
using Anthill.Parser.Models;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.FormRecognizer.Models;
using Unity;

namespace Anthill.Parser.AzureOCR
{
    public class PdfParser3_0 : IPdfParser
    {
        private AzureKeyCredential _credential;
        private DocumentAnalysisClient _client;

        private IUnityContainer _container;
        private Settings _settings;

        private List<ParsedDocument> _documents = new();

        public PdfParser3_0(IUnityContainer container)
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

            AnalyzeDocumentOperation operation;
            using (FileStream fs = new(path, FileMode.Open))
            {
                operation = await _client.StartAnalyzeDocumentAsync(_settings.ModelId, fs);
            }

            AnalyzeResult result = await operation.WaitForCompletionAsync();

            return CommonService.AzureOCRCreateParsedDocument(_settings, result, path);


        }
    }
}
