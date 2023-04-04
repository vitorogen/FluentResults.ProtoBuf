using ProtoBuf;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FluentResults.ProtoBuf;

/// <summary>
///     Represents a serializable reason for a result, either an error or a success.
/// </summary>
[ProtoInclude(100, typeof(ProtoError))]
[ProtoInclude(101, typeof(ProtoSuccess))]
[ProtoContract]
public class ProtoReason
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtoReason" /> class.
    /// </summary>
    public ProtoReason()
    {
        this.Metadata = new Dictionary<string, string>();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtoReason" /> class with a message.
    /// </summary>
    /// <param name="message">The message for the reason.</param>
    public ProtoReason(string message)
        : this()
    {
        this.Message = message;
    }

    /// <summary>
    ///     Gets or sets the message of the reason.
    /// </summary>
    [ProtoMember(1)]
    public string Message { get; set; }

    /// <summary>
    ///     Gets or sets the metadata associated with the reason.
    /// </summary>
    [DataMember]
    [ProtoMember(2)]
    public Dictionary<string, string> Metadata { get; set; }
}