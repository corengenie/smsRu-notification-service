﻿using System.Runtime.Serialization;

namespace SmsRuNotificationService.Exceptions;

/// <summary>
/// Message validation exception
/// </summary>
[Serializable]
public class InvalidMessageException : Exception
{
    public InvalidMessageException() { }
    public InvalidMessageException(string message) : base(message) { }
    public InvalidMessageException(string message, Exception innerException) : base(message, innerException) { }
    protected InvalidMessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}