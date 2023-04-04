# FluentResults.ProtoBuf

FluentResults.ProtoBuf is a library that extends the popular [FluentResults](https://github.com/altmann/FluentResults) library and provides compatibility with the [protobuf-net](https://github.com/protobuf-net/protobuf-net) library for serialization and transport between client/server applications. The library enables you to serialize and deserialize Result objects using Protocol Buffers while maintaining their properties and structure.

## Features / Limitations

- Convert FluentResults Result objects to ProtoResult objects suitable for serialization with protobuf-net.
- Convert ProtoResult objects back to FluentResults Result objects.
- Serialize and deserialize Result objects with Errors, Successes, and metadata while preserving their structure.
- Compatible with FluentResults and protobuf-net.
- The Metadata dictionary only supports string values.

## Installation

Install FluentResults.ProtoBuf via NuGet:

```powershell
Install-Package FluentResults.ProtoBuf
```

## Usage

### 1. Implement the gRPC service

Implement the gRPC service by returning a Result value implicitly converted to a ProtoResult. Use the extension methods provided by the FluentResults library, such as ToResult() or WithError(), to build your Result value.

```csharp
using FluentResults;
using FluentResults.ProtoBuf;

public class MyService : IMyService
{
    public async Task<ProtoResult> MyMethodAsync(MyRequest request)
    {
        // Perform your business logic here
        // ...

        // Create a Result object
        var result = Results.Ok();

        // Optionally, add errors or successes
        if (someCondition)
        {
            result = result.WithError("An error occurred.");
        }
        else
        {
            result = result.WithSuccess("Operation completed successfully.");
        }

        // Return the Result value implicitly converted to a ProtoResult
        return result;
    }
}
```

### 2. Consume the gRPC service

```csharp
using FluentResults;
using FluentResults.ProtoBuf;

public async Task ConsumeServiceAsync()
{
    // Get a reference to your gRPC service client
    var myServiceClient = ...;

    // Call the gRPC service method
    ProtoResult protoResult = await myServiceClient.MyMethodAsync(new MyRequest());

    // Convert the ProtoResult back to a Result object
    Result result = protoResult.ToResult();
	
	// Handle the result
	...
}
```

## License

FluentResults.ProtoBuf is released under the [MIT License](https://github.com/vitorogen/FluentResults.ProtoBuf/blob/develop/LICENSE).