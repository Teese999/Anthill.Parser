using System;
using Anthill.Parser.AzureOCR;
using Anthill.Parser.Console.Outputs;
using Anthill.Parser.Console.PostProcessors;
using Anthill.Parser.Contracts;
using Anthill.Parser.FileConverter;
using Unity;
using Unity.Lifetime;

namespace Anthill.Parser.Console
{
    public class ContainerConfiguration
    {

        public static void RegisterTypes<TLifetime>(IUnityContainer container)
      where TLifetime : ITypeLifetimeManager, new()
        {
            container.RegisterType<IHtmlToPdfConverter, HtmlToPdfConverter>(new TLifetime());
            container.RegisterType<IPdfParser, PdfParser>(new TLifetime());
            container.RegisterType<ITxtToHtmlRenamer, TxtToHtmlRenamer>(new TLifetime());
            container.RegisterType<IPostProcessor, CascadePurchaseOrdersPostProcessor>(new TLifetime());
            container.RegisterType<IOutput, ConsoleOutput>(new TLifetime());
        }

    }
}
