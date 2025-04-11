using System;
using System.Net;

namespace GeologicalResearch.Exceptions;
public class NotFoundException : GeologicalResearchAppException
{
    public NotFoundException(string messageEx, string userMessage) 
        : base(messageEx, userMessage, HttpStatusCode.NotFound) { }
}
