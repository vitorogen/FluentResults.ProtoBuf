using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace FluentResults.ProtoBuf;

/// <summary>
///     Represents a serializable error reason for a result.
/// </summary>
[ProtoContract]
public class ProtoError : ProtoReason
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtoError" /> class.
    /// </summary>
    public ProtoError()
    {
        this.Reasons = new List<ProtoError>();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtoError" /> class with a message.
    /// </summary>
    /// <param name="message">The description of the error.</param>
    public ProtoError(string message)
        : this()
    {
        this.Message = message;
    }

    /// <summary>
    ///     Gets or sets the list of reasons for an error.
    /// </summary>
    [ProtoMember(1)]
    public List<ProtoError> Reasons { get; set; }

    /// <summary>
    ///     Converts an <see cref="Error" /> object to a <see cref="ProtoError" /> object.
    /// </summary>
    /// <param name="error">The <see cref="Error" /> object to convert.</param>
    public static explicit operator ProtoError(Error error)
    {
        if (error == null) return null;

        var protoError = new ProtoError
        {
            Message = error.Message,
            Metadata = error.Metadata.ToDictionary(x => x.Key, x => x.Value.ToString()),
            Reasons = error.Reasons.OfType<Error>().Select(e => (ProtoError)e).ToList()
        };

        return protoError;
    }

    /// <summary>
    ///     Converts a <see cref="ProtoError" /> object to an <see cref="Error" /> object.
    /// </summary>
    /// <param name="protoError">The <see cref="ProtoError" /> object to convert.</param>
    public static explicit operator Error(ProtoError protoError)
    {
        if (protoError == null) return null;

        var error = new Error(protoError.Message);
        if (protoError.Metadata != null)
            foreach (var kvp in protoError.Metadata)
                error.WithMetadata(kvp.Key, kvp.Value);

        error.CausedBy(protoError.Reasons.Select(e => (Error)e).ToList());
        return error;
    }
}