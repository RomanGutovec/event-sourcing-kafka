using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Converters;

public class EventJsonConverter : JsonConverter<BaseEvent>
{
    public override bool CanConvert(Type typeToConvert)
        => typeof(BaseEvent).IsAssignableFrom(typeToConvert);

    public override BaseEvent Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        if (!jsonDocument.RootElement.TryGetProperty("Type", out var typeProperty))
        {
            throw new JsonException();
        }

        var typeDiscriminator = typeProperty.GetString();
        var rawJson = jsonDocument.RootElement.GetRawText();

        return typeDiscriminator switch
        {
            nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(rawJson),
            nameof(MessageUpdatedEvent) => JsonSerializer.Deserialize<MessageUpdatedEvent>(rawJson, options),
            nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(rawJson, options),
            nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(rawJson, options),
            nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(rawJson, options),
            nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(rawJson, options),
            nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(rawJson, options),
            _ => throw new JsonException($"{typeDiscriminator} is not supported yet.")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        BaseEvent value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}