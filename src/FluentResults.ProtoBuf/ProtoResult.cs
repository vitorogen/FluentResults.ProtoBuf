using ProtoBuf;
using System;
using System.Collections.Generic;

namespace FluentResults.ProtoBuf;

/// <summary>
/// Represents an interface for ProtoResult and ProtoResult&lt;TValue&gt;.
/// </summary>
public interface IProtoResult
{
    /// <summary>
    /// Gets or sets the list of reasons for the result.
    /// </summary>
    List<ProtoReason> Reasons { get; set; }
}

[ProtoContract]
public class ProtoResult<TValue> : IProtoResult
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtoResult" /> class.
    /// </summary>
    public ProtoResult()
    {
        this.Reasons = new List<ProtoReason>();
    }

    /// <summary>
    ///     Gets or sets the list of reasons for the result.
    /// </summary>
    [ProtoMember(1)]
    public List<ProtoReason> Reasons { get; set; }

    [ProtoMember(2)]
    public TValue Value { get; set; }

    /// <summary>
    /// Converts a Result&lt;TValue&gt; object to a ProtoResult&lt;TValue&gt; object.
    /// </summary>
    /// <param name="result">The Result&lt;TValue&gt; object to convert.</param>
    /// <returns>The converted ProtoResult&lt;TValue&gt; object.</returns>
    public static implicit operator ProtoResult<TValue>(Result<TValue> result)
    {
        var protoResult = new ProtoResult<TValue>
        {
            Value = result.IsSuccess ? result.Value : default
        };

        foreach (var reason in result.Reasons)
        {
            switch (reason)
            {
                case Error error:
                    protoResult.Reasons.Add((ProtoError)error);
                    break;
                case Success success:
                    protoResult.Reasons.Add((ProtoSuccess)success);
                    break;
                default:
                    throw new InvalidProtoReasonException("Reason is not of type Error or Success");
            }
        }

        return protoResult;
    }


    /// <summary>
    /// Converts a TValue object to a ProtoResult&lt;TValue&gt; object.
    /// </summary>
    /// <param name="value">The TValue object to convert.</param>
    /// <returns>The converted ProtoResult&lt;TValue&gt; object.</returns>
    public static implicit operator ProtoResult<TValue>(TValue value)
    {
        var protoResult = new ProtoResult<TValue>
        {
            Value = value
        };
        return protoResult;
    }

    public Result<TValue> ToResult()
    {
        var result = new Result<TValue>();

        foreach (var protoReason in this.Reasons)
        {
            switch (protoReason)
            {
                case ProtoError protoError:
                    result.WithError((Error)protoError);
                    break;
                case ProtoSuccess protoSuccess:
                    result.WithSuccess((Success)protoSuccess);
                    break;
                default:
                    throw new InvalidProtoReasonException("Invalid ProtoReason type encountered.");
            }
        }

        if (result.IsFailed)
        {
            return result;
        }

        result = result.WithValue(this.Value);
        return result;
    }
}


/// <summary>
///     Represents a serializable result that can contain a list of reasons, either errors or successes.
/// </summary>
[ProtoContract]
public class ProtoResult : IProtoResult
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtoResult" /> class.
    /// </summary>
    public ProtoResult()
    {
        this.Reasons = new List<ProtoReason>();
    }

    /// <summary>
    ///     Gets or sets the list of reasons for the result.
    /// </summary>
    [ProtoMember(1)]
    public List<ProtoReason> Reasons { get; set; }

    /// <summary>
    ///     Converts a <see cref="Result" /> object to a <see cref="ProtoResult" /> object.
    /// </summary>
    /// <param name="result">The <see cref="Result" /> object to convert.</param>
    public static implicit operator ProtoResult(Result result)
    {
        var protoResult = new ProtoResult();
        foreach (var reason in result.Reasons)
            switch (reason)
            {
                case Error error:
                    protoResult.Reasons.Add((ProtoError)error);
                    break;
                case Success success:
                    protoResult.Reasons.Add((ProtoSuccess)success);
                    break;
                default:
                    throw new InvalidProtoReasonException("Reason is not of type Error or Success");
            }

        return protoResult;
    }

    /// <summary>
    ///     Converts the <see cref="ProtoResult" /> object to a <see cref="Result" /> object.
    /// </summary>
    /// <returns>The converted <see cref="Result" /> object.</returns>
    public Result ToResult()
    {
        var result = new Result();

        foreach (var protoReason in this.Reasons)
            switch (protoReason)
            {
                case ProtoError protoError:
                    result.WithError((Error)protoError);
                    break;
                case ProtoSuccess protoSuccess:
                    result.WithSuccess((Success)protoSuccess);
                    break;
                default:
                    throw new InvalidProtoReasonException("Invalid ProtoReason type encountered.");
            }

        return result;
    }
}