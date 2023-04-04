using System;

namespace FluentResults.ProtoBuf;

public class InvalidProtoReasonException : Exception
{
    public InvalidProtoReasonException()
    {
    }

    public InvalidProtoReasonException(string message) : base(message)
    {
    }
}