# String Enum

String Enum is a simple way to define a set of string values that can be used as a type, also encapsulating custom logic.

## Usage

```cs
public record Subscription(string Value, double Discount) : StringEnum<Subscription>(Value)
{
	public static readonly Subscription Free = new("free", 0.0);
	public static readonly Subscription Premium = new("premium", 0.25);
	public static readonly Subscription Vip = new("vip", 0.5);

	public double ApplyDiscount(double Price) => Price * (1.0 - Discount);
}

Subscription.Free.ApplyDiscount(100.0m); // 100.0
Subscription.Premium.ApplyDiscount(100.0m); // 75.0
Subscription.Vip.ApplyDiscount(100.0m); // 50.0
```