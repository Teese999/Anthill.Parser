using System;
using System.Collections.Generic;
using Anthill.Parser.Models;

namespace Anthill.Parser.Contracts
{
    public interface IPostProcessor
    {
        public ParsedDocument ProcessDocuments(ParsedDocument pardesDocuments);
    }
}
