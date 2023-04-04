using ProtoBuf;
using System.Linq;

namespace FluentResults.ProtoBuf;

/// <summary>
///     Represents a serializable success reason for a result.
/// </summary>
[ProtoContract]
public class ProtoSuccess : ProtoReason
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtoSuccess" /> class.
    /// </summary>
    public ProtoSuccess()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtoSuccess" /> class with a message.
    /// </summary>
    /// <param name="message">The description of the success message.</param>
    public ProtoSuccess(string message)
        : this()
    {
        this.Message = message;
    }

    /// <summary>
    ///     Converts a <see cref="Success" /> object to a <see cref="ProtoSuccess" /> object.
    /// </summary>
    /// <param name="success">The <see cref="Success" /> object to convert.</param>
    public static explicit operator ProtoSuccess(Success success)
    {
        if (success == null)
            return null;

        return new ProtoSuccess { Message = success.Message, Metadata = success.Metadata.ToDictionary(x => x.Key, x => x.Value.ToString()) };
    }

    /// <summary>
    ///     Converts a <see cref="ProtoSuccess" /> object to a <see cref="Success" /> object.
    /// </summary>
    /// <param name="protoSuccess">The <see cref="ProtoSuccess" /> object to convert.</param>
    public static explicit operator Success(ProtoSuccess protoSuccess)
    {
        if (protoSuccess == null) return null;

        var success = new Success(protoSuccess.Message);
        if (protoSuccess.Metadata != null)
            foreach (var kvp in protoSuccess.Metadata)
                success.WithMetadata(kvp.Key, kvp.Value);

        return success;
    }
}