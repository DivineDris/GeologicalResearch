using System;
using System.Net;

namespace GeologicalResearch.Exceptions;

public class ValidationException : GeologicalResearchAppException
{
        public ValidationException(string messageEx, string userMessage) 
        : base(messageEx, userMessage, HttpStatusCode.BadRequest) { }
}
