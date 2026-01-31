namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Represents a single line item in a prescription (medication or service)
/// </summary>
public class PrescriptionLineItem
{
    /// <summary>
    /// Line number in the prescription (1-based)
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// 8-digit PZN (Pharmazentralnummer) code, if applicable
    /// Mutually exclusive with SOK
    /// </summary>
    public string? Pzn { get; set; }

    /// <summary>
    /// 8-digit SOK (Sonderkennzeichen) special code, if applicable
    /// Mutually exclusive with PZN
    /// </summary>
    public string? Sok { get; set; }

    /// <summary>
    /// Quantity of items dispensed
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gross price in EUR
    /// </summary>
    public decimal GrossPrice { get; set; }

    /// <summary>
    /// VAT rate code: 0=0%, 1=7%, 2=19%
    /// </summary>
    public short VatRate { get; set; }

    /// <summary>
    /// Factor code (2 digits), if applicable
    /// </summary>
    public string? FactorCode { get; set; }

    /// <summary>
    /// Factor value, if factor code is present
    /// </summary>
    public decimal? FactorValue { get; set; }

    /// <summary>
    /// Price code (2 digits), if applicable
    /// </summary>
    public string? PriceCode { get; set; }

    /// <summary>
    /// Price value, if price code is present
    /// </summary>
    public decimal? PriceValue { get; set; }

    /// <summary>
    /// Product description
    /// </summary>
    public string? Description { get; set; }
}
