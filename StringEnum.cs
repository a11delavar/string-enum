using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace A11d.StringEnum;

public abstract record StringEnum<TSelf>(string Value) : IComparable where TSelf : StringEnum<TSelf>
{
	public static IEnumerable<TSelf> GetMembers()
	{
		var type = typeof(TSelf);
		foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
		{
			var value = typeof(TSelf).IsAssignableFrom(field.FieldType) is false ? null : field.GetValue(default);
			if (value is TSelf @enum)
			{
				yield return @enum;
			}
		}
	}

	public static bool TryParse(string? value, IFormatProvider? _, out TSelf @enum)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
		@enum = GetMembers().FirstOrDefault(e => e.Value == value)!;
		return @enum is not null;
	}

	public static TSelf Parse(string value, IFormatProvider? formatProvider = null)
	{
		var valid = TryParse(value, formatProvider, out var @enum);
		if (valid is false)
		{
			throw new ArgumentException($"The value '{value}' is not a valid {typeof(TSelf).Name}");
		}
		return @enum;
	}

	public static bool operator ==(StringEnum<TSelf>? @enum, string? value) => @enum?.Value == value;
	public static bool operator !=(StringEnum<TSelf>? @enum, string? value) => @enum?.Value != value;

	public sealed override string ToString() => Value;

	public int CompareTo(object? obj) => CompareTo(obj is TSelf other ? other : null);
	public int CompareTo(TSelf? other) => other is null ? 1 : string.Compare(Value, other.Value, StringComparison.Ordinal);

	public class JsonConverter : JsonConverter<TSelf>
	{
		public sealed override TSelf Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Parse(reader.GetString() ?? "", null);

		public sealed override void Write(Utf8JsonWriter writer, TSelf value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value);
	}
}