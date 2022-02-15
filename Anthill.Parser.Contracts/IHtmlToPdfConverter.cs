using System;
namespace Anthill.Parser.Contracts
{
    public interface IHtmlToPdfConverter
    {
        public void ConvertHtmlToPdf(string path);
    }
}
