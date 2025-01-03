namespace A11d.StringEnum;

using System.Text.Json;
using System.Text.Json.Serialization;

public class StringEnumJsonConverter<TSelf> : JsonConverter<TSelf> where TSelf : StringEnum<TSelf>
{
	public sealed override TSelf? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> StringEnum<TSelf>.Parse(reader.GetString() ?? "", null);

	public sealed override void Write(Utf8JsonWriter writer, TSelf value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.Value);
}