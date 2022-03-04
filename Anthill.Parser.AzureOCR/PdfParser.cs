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
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;

namespace Anthill.Parser.AzureOCR
{

    public class PdfParser : IPdfParser
    {
        private AzureKeyCredential _credential;
        private FormRecognizerClient _client;

        private IUnityContainer _container;
        private Settings _settings;

        private List<ParsedDocument> _documents = new();
        public PdfParser(IUnityContainer container)
        {
            _container = container;
            _settings = container.Resolve<Settings>();
            _credential = new AzureKeyCredential(_settings.ApiKey);
            _client = new FormRecognizerClient(new Uri(_settings.ApiEndpoint), _credential);

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

            var options = new RecognizeCustomFormsOptions() { IncludeFieldElements = true };
            RecognizeCustomFormsOperation operation = null;
            using (FileStream fs = new(path, FileMode.Open))
            {
                operation = await _client.StartRecognizeCustomFormsAsync(_settings.ModelId, fs, options);
            }

            Response<RecognizedFormCollection> result = await operation.WaitForCompletionAsync();

            return CommonService.AzureOCRCreateParsedDocument(_settings, result.Value, path);


        }
    }
}
