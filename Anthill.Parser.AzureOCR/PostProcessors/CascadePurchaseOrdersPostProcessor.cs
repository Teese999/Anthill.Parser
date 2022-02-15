using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Anthill.Parser.Contracts;
using Anthill.Parser.Models;

namespace Anthill.Parser.AzureOCR.PostProcessors
{
    public class CascadePurchaseOrdersPostProcessor : IPostProcessor
    {
        private Settings _settings;
        public CascadePurchaseOrdersPostProcessor(Settings settings)
        {
            _settings = settings;
        }

        public ParsedDocument ProcessDocuments(ParsedDocument parsedDocument)
        {
            if (parsedDocument.NeedHandChek == false)
            {
                var poFieldsSplitted = parsedDocument.Fields["PurchaseOrder"].Split(" ").ToList();
                var orderNumber = poFieldsSplitted.Where(x => x.Any(char.IsDigit)).First();
                if (orderNumber.Contains("#")) { orderNumber = orderNumber.Replace("#", ""); }
                var accNumber = string.Concat(parsedDocument.Fields["Account"].Where(char.IsDigit));
                parsedDocument.Fields["PurchaseOrder"] = orderNumber;
                parsedDocument.Fields["Account"] = accNumber;
            }
            else
            {
                if (_settings.CopyHendchakeFilesToFolder)
                {
                    File.Copy(parsedDocument.Path, Path.Combine(_settings.HandchackFolderName, Path.GetFileName(parsedDocument.Path)));
                }
            }

            return parsedDocument;
        }
    }
}
