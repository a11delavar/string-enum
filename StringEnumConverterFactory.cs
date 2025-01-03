namespace A11d.StringEnum;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class StringEnumConverterFactory : JsonConverterFactory
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class SkipAttribute : Attribute { }

	public override bool CanConvert(Type typeToConvert) =>
		typeToConvert.BaseType?.IsGenericType is true
		&& typeToConvert.BaseType.GetGenericTypeDefinition() == typeof(StringEnum<>)
		&& typeToConvert.GetCustomAttributes(typeof(SkipAttribute), false).Length == 0;

	public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		var converterType = typeof(StringEnumJsonConverter<>).MakeGenericType(typeToConvert);
		return (JsonConverter?)Activator.CreateInstance(converterType);
	}
}