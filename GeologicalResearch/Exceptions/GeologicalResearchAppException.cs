using System;
using System.Net;

namespace GeologicalResearch.Exceptions;
//Базовый класс кастомного исключения
public abstract class GeologicalResearchAppException : Exception
{
    public HttpStatusCode HttpStatusCode { get; }
    public string UserMessage { get; }
        protected GeologicalResearchAppException(string messageEx, string userMessage, HttpStatusCode httpStatusCode) 
            : base(messageEx)
        {
            HttpStatusCode = httpStatusCode;
            UserMessage = userMessage;
        }
}

