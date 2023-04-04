using FluentAssertions;
using ProtoBuf;

namespace FluentResults.ProtoBuf.UnitTests;

public sealed class ProtoResultTests
{
    [Fact]
    public void GivenAResultWithSuccess_WhenConvertingToProtoResult_ThenProtoResultShouldContainSuccess()
    {
        // Arrange
        var result = Result.Ok().WithSuccess("Success message");

        // Act
        ProtoResult protoResult = result;

        // Assert
        protoResult.Reasons.Should().HaveCount(1);
        protoResult.Reasons.Single().Should().BeOfType<ProtoSuccess>();
        protoResult.Reasons.Single().Message.Should().Be("Success message");
    }

    [Fact]
    public void GivenAResultWithError_WhenConvertingToProtoResult_ThenProtoResultShouldContainError()
    {
        // Arrange
        var result = Result.Fail("Error message");

        // Act
        ProtoResult protoResult = result;

        // Assert
        protoResult.Reasons.Should().HaveCount(1);
        protoResult.Reasons.Single().Should().BeOfType<ProtoError>();
        protoResult.Reasons.Single().Message.Should().Be("Error message");
    }

    [Fact]
    public void GivenAProtoResultWithSuccess_WhenConvertingToResult_ThenResultShouldContainSuccess()
    {
        // Arrange
        var protoResult = new ProtoResult();
        protoResult.Reasons.Add(new ProtoSuccess("Success message"));

        // Act
        var result = protoResult.ToResult();

        // Assert
        result.Reasons.Should().HaveCount(1);
        result.Reasons.Single().Should().BeOfType<Success>();
        result.Reasons.Single().Message.Should().Be("Success message");
    }

    [Fact]
    public void GivenAProtoResultWithError_WhenConvertingToResult_ThenResultShouldContainError()
    {
        // Arrange
        var protoResult = new ProtoResult();
        protoResult.Reasons.Add(new ProtoError("Error message"));

        // Act
        var result = protoResult.ToResult();

        // Assert
        result.Reasons.Should().HaveCount(1);
        result.Reasons.Single().Should().BeOfType<Error>();
        result.Reasons.Single().Message.Should().Be("Error message");
    }

    [Fact]
    public void GivenAResultWithErrorAndNestedError_WhenConvertingToProtoResult_ThenProtoResultShouldContainErrorWithNestedError()
    {
        // Arrange
        var nestedInnerError = new Error("Nested inner error message");
        var innerError = new Error("Inner error message").CausedBy(nestedInnerError);
        var result = new Result().WithError(new Error("Outer error message").CausedBy(innerError));

        // Act
        ProtoResult protoResult = result;


        // Assert
        var expectedProtoResult = new ProtoResult
        {
            Reasons = new List<ProtoReason>
            {
                new ProtoError("Outer error message")
                {
                    Reasons = new List<ProtoError>
                    {
                        new("Inner error message") { Reasons = new List<ProtoError> { new("Nested inner error message") } }
                    }
                }
            }
        };
        protoResult.Should().BeEquivalentTo(expectedProtoResult);
    }

    [Fact]
    public void GivenAProtoResultWithErrorAndNestedError_WhenConvertingToResult_ThenResultShouldContainErrorWithNestedError()
    {
        // Arrange
        var protoResult = new ProtoResult();
        var outerError = new ProtoError("Outer error message");
        var innerError = new ProtoError("Inner error message");
        innerError.Reasons.Add(new ProtoError("Nested inner error message"));
        outerError.Reasons.Add(innerError);
        protoResult.Reasons.Add(outerError);

        // Act
        var result = protoResult.ToResult();


        // Assert
        var expectedResult = new Result().WithError(
            new Error("Outer error message").CausedBy(
                new Error("Inner error message").CausedBy(
                    new Error("Nested inner error message"))));
        result.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public void GivenAProtoResultWithErrorAndNestedError_WhenSerializingAndDeserializing_ThenDeserializedProtoResultShouldBeEquivalentToOriginal()
    {
        // Arrange
        var protoResult = new ProtoResult();
        var outerError = new ProtoError("Outer error message");
        var innerError = new ProtoError("Inner error message");
        innerError.Reasons.Add(new ProtoError("Nested inner error message"));
        outerError.Reasons.Add(innerError);
        protoResult.Reasons.Add(outerError);

        // Act
        ProtoResult deserializedProtoResult;
        using (var memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, protoResult);
            memoryStream.Position = 0;
            deserializedProtoResult = Serializer.Deserialize<ProtoResult>(memoryStream);
        }

        // Assert
        deserializedProtoResult.Should().BeEquivalentTo(protoResult);
    }

    [Fact]
    public void GivenAProtoResultWithSuccessAndMetadata_WhenSerializingAndDeserializing_ThenDeserializedProtoResultShouldBeEquivalentToOriginal()
    {
        // Arrange
        var protoResult = new ProtoResult();
        var success = new ProtoSuccess("Success message");
        success.Metadata.Add("key", "value");
        protoResult.Reasons.Add(success);

        // Act
        ProtoResult deserializedProtoResult;
        using (var memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, protoResult);
            memoryStream.Position = 0;
            deserializedProtoResult = Serializer.Deserialize<ProtoResult>(memoryStream);
        }

        // Assert
        deserializedProtoResult.Should().BeEquivalentTo(protoResult);
    }

    [Fact]
    public void ToResult_WithEmptyProtoResultWithValue_ShouldReturnEmptyResultWithValue()
    {
        // Arrange
        var protoResult = new ProtoResult<int> { Value = 42 };

        // Act
        var result = protoResult.ToResult();

        // Assert
        result.Reasons.Should().BeEmpty();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ToResult_WithProtoResultWithValueHavingSuccessReason_ShouldReturnResultWithValueAndSuccess()
    {
        // Arrange
        var protoResult = new ProtoResult<int> { Value = 42 };
        protoResult.Reasons.Add(new ProtoSuccess("Success!"));

        // Act
        var result = protoResult.ToResult();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ToResult_WithProtoResultWithValueHavingErrorReason_ShouldReturnResultWithValueAndError()
    {
        // Arrange
        var protoResult = new ProtoResult<int> { Value = 42 };
        protoResult.Reasons.Add(new ProtoError("Error!"));

        // Act
        var result = protoResult.ToResult();
        var getValueAction = () => result.Value;

        // Assert
        result.IsFailed.Should().BeTrue();
        getValueAction.Should().Throw<InvalidOperationException>();
    }


    [Fact]
    public void Given_ProtoResultWithValue_When_SerializedAndDeserialized_Then_ShouldPreserveValue()
    {
        // Arrange
        var originalProtoResult = new ProtoResult<int> { Value = 42 };

        // Act
        var deserializedProtoResult = SerializeAndDeserialize(originalProtoResult);

        // Assert
        deserializedProtoResult.Value.Should().Be(42);
    }

    [Fact]
    public void Given_ProtoResultWithValueAndSuccessReason_When_SerializedAndDeserialized_Then_ShouldPreserveValueAndReason()
    {
        // Arrange
        var originalProtoResult = new ProtoResult<int> { Value = 42 };
        originalProtoResult.Reasons.Add(new ProtoSuccess("Success!"));

        // Act
        var deserializedProtoResult = SerializeAndDeserialize(originalProtoResult);

        // Assert
        deserializedProtoResult.Value.Should().Be(42);
        deserializedProtoResult.Reasons.Should().HaveCount(1);
        deserializedProtoResult.Reasons[0].Should().BeOfType<ProtoSuccess>();
        ((ProtoSuccess)deserializedProtoResult.Reasons[0]).Message.Should().Be("Success!");
    }

    [Fact]
    public void Given_ProtoResultWithValueAndErrorReason_When_SerializedAndDeserialized_Then_ShouldPreserveValueAndReason()
    {
        // Arrange
        var originalProtoResult = new ProtoResult<int> { Value = 42 };
        originalProtoResult.Reasons.Add(new ProtoError("Error!"));

        // Act
        var deserializedProtoResult = SerializeAndDeserialize(originalProtoResult);

        // Assert
        deserializedProtoResult.Value.Should().Be(42);
        deserializedProtoResult.Reasons.Should().HaveCount(1);
        deserializedProtoResult.Reasons[0].Should().BeOfType<ProtoError>();
        ((ProtoError)deserializedProtoResult.Reasons[0]).Message.Should().Be("Error!");
    }

    private static ProtoResult<TValue> SerializeAndDeserialize<TValue>(ProtoResult<TValue> originalProtoResult)
    {
        using var memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, originalProtoResult);
        memoryStream.Position = 0;
        return Serializer.Deserialize<ProtoResult<TValue>>(memoryStream);
    }


    [Fact]
    public void Given_ResultWithValue_When_ImplicitlyConvertedToProtoResultWithValue_Then_ShouldPreserveValue()
    {
        // Arrange
        var result = Result.Ok(42);

        // Act
        ProtoResult<int> protoResult = result;

        // Assert
        protoResult.Value.Should().Be(42);
    }

    [Fact]
    public void Given_ResultWithValueAndReason_When_ImplicitlyConvertedToProtoResultWithValue_Then_ShouldPreserveValueAndReason()
    {
        // Arrange
        var result = Result.Ok(42).WithSuccess("Success!");

        // Act
        ProtoResult<int> protoResult = result;

        // Assert
        protoResult.Value.Should().Be(42);
        protoResult.Reasons.Should().HaveCount(1);
        protoResult.Reasons[0].Should().BeOfType<ProtoSuccess>();
        ((ProtoSuccess)protoResult.Reasons[0]).Message.Should().Be("Success!");
    }

    [Fact]
    public void Given_Value_When_ImplicitlyConvertedToProtoResultWithValue_Then_ShouldSetProtoResultWithValue()
    {
        // Arrange
        var value = 42;

        // Act
        ProtoResult<int> protoResult = value;

        // Assert
        protoResult.Value.Should().Be(42);
    }
}