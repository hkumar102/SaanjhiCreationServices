using System;
using System.Runtime.Serialization;

namespace Shared.ErrorHandling;

/// <summary>
/// Exception for business rule violations. Should result in HTTP 402 responses.
/// </summary>
[Serializable]
public class BusinessRuleException : Exception
{
    public BusinessRuleException() { }
    public BusinessRuleException(string message) : base(message) { }
    public BusinessRuleException(string message, Exception inner) : base(message, inner) { }
    protected BusinessRuleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
