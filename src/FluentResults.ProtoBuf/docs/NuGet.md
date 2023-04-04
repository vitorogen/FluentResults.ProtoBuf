# FluentResults.ProtoBuf

FluentResults.ProtoBuf is a library that extends the popular [FluentResults](https://github.com/altmann/FluentResults) library and provides compatibility with the [protobuf-net](https://github.com/protobuf-net/protobuf-net) library for serialization and transport between client/server applications. The library enables you to serialize and deserialize Result objects using Protocol Buffers while maintaining their properties and structure.

## Features / Limitations

- Convert FluentResults Result objects to ProtoResult objects suitable for serialization with protobuf-net.
- Convert ProtoResult objects back to FluentResults Result objects.
- Serialize and deserialize Result objects with Errors, Successes, and metadata while preserving their structure.
- Compatible with FluentResults and protobuf-net.
- The Metadata dictionary only supports string values.