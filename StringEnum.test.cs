using System.Text.Json;
using Xunit;

namespace A11d.StringEnum;

public record SampleStringEnum(string Value) : StringEnum<SampleStringEnum>(Value)
{
	public static readonly SampleStringEnum Option1 = new("option1");
	public static readonly SampleStringEnum Option2 = new("option2");
}

public record ComplexSampleStringEnum(string Value, int OtherValue) : StringEnum<ComplexSampleStringEnum>(Value)
{
	public static ComplexSampleStringEnum Option1 => new("option1", 1);
	public static ComplexSampleStringEnum Option2 => new("option2", 2);

	// Should be ignored by GetMembers
	public static ComplexSampleStringEnum GetOption() => new("option3", 2);

	public string Mutable { get; set; } = string.Empty;
}

public class StringEnumTest
{
	[Theory]
	[InlineData("option1")]
	[InlineData("option2")]
	public void TestConstructor(string value)
	{
		SampleStringEnum @enum = new(value);
		Assert.Equal(value, @enum.Value);
	}

	[Fact]
	public void TestGetMembers()
	{
		var all = SampleStringEnum.GetMembers().ToArray();

		Assert.Equal(2, all.Length);
		Assert.Equal("option1", all.First().Value);
		Assert.Equal("option2", all.Last().Value);
	}

	[Fact]
	public void TestComplexGetMembers()
	{
		var all = ComplexSampleStringEnum.GetMembers().ToArray();

		Assert.Equal(2, all.Length);
		Assert.Equal("option1", all.First().Value);
		Assert.Equal("option2", all.Last().Value);
	}

	[Theory]
	[InlineData("option1", true)]
	[InlineData("option2", true)]
	[InlineData("invalid-option", false)]
	public void TestParse(string value, bool valid)
	{
		Assert.Equal(valid, SampleStringEnum.TryParse(value, null, out var parsedEnum));
		if (valid)
		{
			Assert.Equal(value, parsedEnum!.Value);
			Assert.Equal(value, SampleStringEnum.Parse(value, null)!.Value);
		}
		else
		{
			Assert.Throws<ArgumentException>(() =>
			{
				var @enum = SampleStringEnum.Parse(value, null);
				_ = @enum!.Value;
			});
		}
	}

	[Fact]
	public void TestParseComplex()
	{
		Assert.True(ComplexSampleStringEnum.TryParse("option1", null, out var parsedEnum));
		Assert.Equal("option1", parsedEnum!.Value);
		Assert.Equal(1, parsedEnum.OtherValue);
	}

	[Fact]
	public void TestParseNotReturningSameReference()
	{
		ComplexSampleStringEnum.TryParse("option1", null, out var parsedEnum1);
		ComplexSampleStringEnum.TryParse("option1", null, out var parsedEnum2);

		parsedEnum1!.Mutable = "test1";
		parsedEnum2!.Mutable = "test2";

		Assert.NotEqual(parsedEnum1, parsedEnum2);
		Assert.NotEqual(parsedEnum1.Mutable, parsedEnum2.Mutable);
		Assert.Equal("test1", parsedEnum1.Mutable);
		Assert.Equal("test2", parsedEnum2.Mutable);
	}

	[Fact]
	public void TestToString()
	{
		Assert.Equal("option1", SampleStringEnum.Option1.ToString());
		Assert.Equal("option2", SampleStringEnum.Option2.ToString());
	}

	[Fact]
	public void TestCompareTo()
	{
		Assert.Equal(0, SampleStringEnum.Option1.CompareTo(SampleStringEnum.Option1));
		Assert.Equal(-1, SampleStringEnum.Option1.CompareTo(SampleStringEnum.Option2));
		Assert.Equal(1, SampleStringEnum.Option2.CompareTo(SampleStringEnum.Option1));
	}

	[Fact]
	public void TestEquality()
	{
		Assert.Equal(SampleStringEnum.Option1, SampleStringEnum.Option1);
		Assert.True(SampleStringEnum.Option1.Equals(SampleStringEnum.Option1));
		Assert.True(SampleStringEnum.Option1 == "option1");

		Assert.NotEqual(SampleStringEnum.Option1, SampleStringEnum.Option2);
		Assert.True(SampleStringEnum.Option1 != "option2");
		Assert.True(SampleStringEnum.Option1 != "option2");
	}

	[Fact]
	public void TestJsonConverter()
	{
		JsonSerializerOptions options = new()
		{
			Converters = { new SampleStringEnum.JsonConverter() }
		};

		Assert.Equal("\"option1\"", JsonSerializer.Serialize(SampleStringEnum.Option1, options));
		Assert.Equal(SampleStringEnum.Option2, JsonSerializer.Deserialize<SampleStringEnum>("\"option2\"", options));

		Assert.Equal("""{"Value":"option1"}""", JsonSerializer.Serialize(SampleStringEnum.Option1));
		Assert.Equal(SampleStringEnum.Option1, JsonSerializer.Deserialize<SampleStringEnum>("""{"Value":"option1"}"""));
	}

	private record EnumConvertedByFactory(string Value, int Number) : StringEnum<EnumConvertedByFactory>(Value)
	{
		public static readonly EnumConvertedByFactory Option1 = new("option1", 1);
		public static readonly EnumConvertedByFactory Option2 = new("option2", 2);
	}

	[Fact]
	public void TestJsonConverterFactory()
	{
		JsonSerializerOptions options = new()
		{
			Converters = { new StringEnumConverterFactory() }
		};

		Assert.Equal("\"option1\"", JsonSerializer.Serialize(EnumConvertedByFactory.Option1, options));
		Assert.Equal(EnumConvertedByFactory.Option1, JsonSerializer.Deserialize<EnumConvertedByFactory>("\"option1\"", options));

		Assert.Equal("""{"Number":1,"Value":"option1"}""", JsonSerializer.Serialize(EnumConvertedByFactory.Option1));
		Assert.Equal(EnumConvertedByFactory.Option1, JsonSerializer.Deserialize<EnumConvertedByFactory>("""{"Number":1,"Value":"option1"}"""));
	}

	[StringEnumConverterFactory.Skip]
	private record EnumIgnoredByFactory(string Value, int Number) : StringEnum<EnumConvertedByFactory>(Value)
	{
		public static readonly EnumIgnoredByFactory Option1 = new("option1", 1);
		public static readonly EnumIgnoredByFactory Option2 = new("option2", 2);
	}

	[Fact]
	public void TestJsonConverterFactorySkip()
	{
		JsonSerializerOptions options = new()
		{
			Converters = { new StringEnumConverterFactory() }
		};

		Assert.Equal("""{"Number":1,"Value":"option1"}""", JsonSerializer.Serialize(EnumIgnoredByFactory.Option1, options));
		Assert.Equal(EnumIgnoredByFactory.Option1, JsonSerializer.Deserialize<EnumIgnoredByFactory>("""{"Number":1,"Value":"option1"}"""));
	}
}