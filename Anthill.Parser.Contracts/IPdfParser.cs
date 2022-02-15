using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anthill.Parser.Models;

namespace Anthill.Parser.Contracts
{
    public interface IPdfParser
    {
        public Task<List<ParsedDocument>> StarParseAllPdfs();
    }
}
