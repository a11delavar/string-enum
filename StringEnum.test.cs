using System.Text.Json;
using Xunit;

namespace A11d.StringEnum;

public record SampleStringEnum(string Value) : StringEnum<SampleStringEnum>(Value)
{
	public static readonly SampleStringEnum Option1 = new("option1");
	public static readonly SampleStringEnum Option2 = new("option2");
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

	[Theory]
	[InlineData("option1", true)]
	[InlineData("option2", true)]
	[InlineData("invalid-option", false)]
	public void TestParse(string value, bool valid)
	{
		Assert.Equal(valid, SampleStringEnum.TryParse(value, null, out var parsedEnum));
		if (valid)
		{
			Assert.Equal(value, parsedEnum.Value);
			Assert.Equal(value, SampleStringEnum.Parse(value, null).Value);
		}
		else
		{
			Assert.Throws<ArgumentException>(() =>
			{
				var @enum = SampleStringEnum.Parse(value, null);
				_ = @enum.Value;
			});
		}
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
		var options = new JsonSerializerOptions
		{
			Converters = { new SampleStringEnum.JsonConverter() }
		};

		var json = JsonSerializer.Serialize(SampleStringEnum.Option1, options);
		Assert.Equal("\"option1\"", json);

		var @enum = JsonSerializer.Deserialize<SampleStringEnum>("\"option2\"", options);
		Assert.Equal(SampleStringEnum.Option2, @enum);
	}
}