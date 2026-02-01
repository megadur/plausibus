# Value Objects

This directory contains Domain-Driven Design (DDD) value objects for the E-Rezept Validator.

## What are Value Objects?

Value objects are immutable objects that represent a descriptive aspect of the domain with **no conceptual identity**. They are defined by their value, not by an ID.

## Benefits

✅ **Type Safety** - Compiler prevents mixing up similar types
✅ **Self-Validation** - Invalid states are impossible
✅ **Encapsulation** - Business logic lives with the data
✅ **Testability** - Easy to test in isolation
✅ **Readability** - Code documents itself

## Available Value Objects

### Money
Represents monetary amounts (prices) stored as cents (integers) for precision.

```csharp
// Creation
var price = Money.Euro(5.50m);          // 5.50 EUR
var zero = Money.Zero;                  // 0.00 EUR

// Operations
var total = price * 3;                  // 16.50 EUR
var sum = price + Money.Euro(2.00m);    // 7.50 EUR

// Comparison
if (price > Money.Euro(1.00m)) { }

// Display
Console.WriteLine(price);               // "5.50 EUR"
```

**Why integers internally?**
- No floating-point precision errors
- Guaranteed exactness for financial calculations
- Standard practice in payment systems

### PromilleFactor
Represents Promilleanteil (per mille) factors with 6 decimal precision.

```csharp
// Creation
var factor = PromilleFactor.FromDecimal(1000m);
var ratio = PromilleFactor.FromRatio(
    dispensed: Quantity.Of(7),
    package: Quantity.Of(28)
); // 250.0 = (7/28) * 1000

// Special values
var one = PromilleFactor.One;                          // 1.0
var aiMarker = PromilleFactor.ArtificialInsemination;  // 1000.0

// Comparison with tolerance
if (actualFactor.EqualsWithinTolerance(expectedFactor)) { }

// Display
Console.WriteLine(factor);  // "1000" or "250.0"
```

**Why micro-units internally?**
- 6 decimal precision (1000.000000)
- Integer storage for exactness
- Tolerant comparison (handles 1.0 = 1.000000)

### Quantity
Represents quantities with optional units.

```csharp
// Creation
var tablets = Quantity.Of(7, "tablets");
var grams = Quantity.Of(2.5m, "g");
var unitless = Quantity.Of(10);

// Operations
var ratio = tablets.DivideBy(Quantity.Of(28));  // 0.25

// Checks
if (quantity.IsZero) { }

// Display
Console.WriteLine(tablets);  // "7 tablets"
```

### Pzn
Represents PZN (Pharmazentralnummer) with Modulo 11 validation.

```csharp
// Creation (with validation)
var pzn = Pzn.Create("00123456");  // ✅ Valid checksum

// Creation (throws if invalid)
try
{
    var invalid = Pzn.Create("00123457");  // ❌ Invalid checksum
}
catch (ArgumentException ex)
{
    // Handle validation error
}

// Safe creation
if (Pzn.TryCreate(userInput, out var validPzn))
{
    // Use validPzn - guaranteed valid
}

// Checks
var hasValidChecksum = pzn.HasValidChecksum;

// Display
Console.WriteLine(pzn);  // "00123456"
```

**Validation rules:**
- Must be 8 digits
- Modulo 11 checksum validation
- Auto-normalized (left-padded with zeros)

### SokCode
Represents SOK (Sonderkennzeichen) special codes.

```csharp
// Creation
var sok = SokCode.Create("1.3.1");

// Special codes
var aiCode = SokCode.ArtificialInsemination;  // "09999643"

// Business logic
if (sok.HasNoQuantityReference)
{
    // Must use factor 1.0 (CALC-002)
}

if (sok.IsArtificialInsemination)
{
    // Special validation (CALC-003)
}

// Display
Console.WriteLine(sok);  // "1.3.1"
```

**Special codes without quantity reference:**
- 1.1.1, 1.1.2, 1.2.1, 1.2.2
- 1.3.1, 1.3.2
- 1.6.5
- 1.10.2, 1.10.3

### PriceIdentifier
Represents price identifiers (Preiskennzeichen) with validation.

```csharp
// Creation (validates against TA3 Table 8.2.26)
var priceId = PriceIdentifier.Create("11");  // ✅ Valid

// Invalid codes throw
try
{
    var invalid = PriceIdentifier.Create("99");  // ❌ Not in valid list
}
catch (ArgumentException ex) { }

// Special codes
var aiPrice = PriceIdentifier.ArtificialInsemination;  // "90"

// Checks
if (priceId.IsArtificialInsemination) { }

// Display
Console.WriteLine(priceId);  // "11"
```

**Valid codes:**
- Standard: 11-17, 21
- Special: 90 (artificial insemination)

## Usage Examples

### Before (Primitive Obsession)

```csharp
// ❌ No type safety - easy to make mistakes
public void CalculateTotal(decimal price, decimal factor, decimal vatRate)
{
    // Which decimal is which?
}

// ❌ Can mix up parameters
CalculateTotal(0.19m, 5.50m, 1000m);  // Wrong order!

// ❌ Can use invalid values
var price = -5.50m;  // Negative price!
var pzn = "abc123";  // Invalid PZN!
```

### After (Value Objects)

```csharp
// ✅ Type safety - compiler enforces correctness
public void CalculateTotal(Money price, PromilleFactor factor, VatRate vatRate)
{
    // Clear what each parameter represents
}

// ✅ Compiler catches mistakes
CalculateTotal(VatRate.Standard, price, factor);  // ❌ Compile error!

// ✅ Invalid states are impossible
var price = Money.Euro(-5.50m);  // ❌ Throws ArgumentException
var pzn = Pzn.Create("abc123");   // ❌ Throws ArgumentException
```

### Real Validator Code

```csharp
// Extract and validate in one step
if (PromilleFactor.TryParse(factorValue, out var actualFactor) &&
    Quantity.TryParse(dispensedQty, out var dispensed) &&
    Quantity.TryParse(packageQty, out var package))
{
    // Calculate expected factor
    var expectedFactor = PromilleFactor.FromRatio(dispensed, package);

    // Compare with tolerance
    if (!actualFactor.EqualsWithinTolerance(expectedFactor))
    {
        // Error: Factor calculation incorrect
        // Expected: 250.0, Found: 500.0
        // Calculation: (7 tablets / 28 tablets) × 1000 = 250.0
    }
}
```

## Implementation Pattern

All value objects follow this pattern:

```csharp
public readonly struct ValueObjectName : IEquatable<ValueObjectName>
{
    // 1. Private immutable fields
    private readonly TValue _value;

    // 2. Private constructor (forces validation)
    private ValueObjectName(TValue value) => _value = value;

    // 3. Factory method with validation
    public static ValueObjectName Create(TValue value)
    {
        // Validate
        if (/* invalid */)
            throw new ArgumentException();

        return new ValueObjectName(value);
    }

    // 4. Try pattern (non-throwing)
    public static bool TryCreate(TValue value, out ValueObjectName result) { }

    // 5. Value equality
    public bool Equals(ValueObjectName other) => _value == other._value;

    // 6. Operators
    public static bool operator ==(ValueObjectName left, ValueObjectName right);

    // 7. ToString for display
    public override string ToString() => _value.ToString();
}
```

## When to Use

✅ **Use for:**
- Money, prices, amounts
- Identifiers (PZN, SOK codes)
- Measurements (quantities, factors)
- Codes with validation rules

❌ **Don't use for:**
- Entities with identity (User, Prescription)
- Simple local variables (loop counters)
- Frequently changing values

## References

- **Domain-Driven Design** by Eric Evans
- **Implementing Domain-Driven Design** by Vaughn Vernon
- **Value Objects Explained** - Martin Fowler

## Financial Calculations

Value objects use **integers internally** for financial data:

| Type | Storage | Precision | Example |
|------|---------|-----------|---------|
| Money | `long` (cents) | 2 decimals | 550 = €5.50 |
| PromilleFactor | `long` (micro-units) | 6 decimals | 1,000,000 = 1.000000 |
| Quantity | `decimal` | Variable | 7.5 tablets |

This eliminates floating-point errors and ensures exact calculations.
