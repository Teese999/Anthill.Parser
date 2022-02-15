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

        public ParsedDocument ProcessDocuments(ParsedDocument pardesDocument)
        {
            if (pardesDocument.NeedHandChek == false)
            {
                var poFieldsSplitted = pardesDocument.Fields["PurchaseOrder"].Split(" ").ToList();
                var orderNumber = poFieldsSplitted.Where(x => x.Any(char.IsDigit)).First();
                if (orderNumber.Contains("#")) { orderNumber = orderNumber.Replace("#", ""); }
                var accNumber = string.Concat(pardesDocument.Fields["Account"].Where(char.IsDigit));
                pardesDocument.Fields["PurchaseOrder"] = orderNumber;
                pardesDocument.Fields["Account"] = accNumber;
            }
            else
            {
                if (_settings.CopyHendchakeFilesToFolder)
                {
                    File.Copy(pardesDocument.Path, Path.Combine(_settings.HandchackFolderName, Path.GetFileName(pardesDocument.Path)));
                }
            }

            return pardesDocument;
        }
    }
}
