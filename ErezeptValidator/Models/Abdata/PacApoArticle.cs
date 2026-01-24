namespace ErezeptValidator.Models.Abdata;

/// <summary>
/// Represents an article from ABDATA PAC_APO table (Article base information)
/// Reference: ABDATA Artikelstamm Technical Documentation, Table PAC_APO (File 1005)
/// </summary>
public class PacApoArticle
{
    /// <summary>
    /// Pharmazentralnummer (PZN) - 8-digit article identifier
    /// Field ID: 01
    /// </summary>
    public string Pzn { get; set; } = string.Empty;

    /// <summary>
    /// Product name
    /// Field ID: 60
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Full product name (unabbreviated)
    /// Field ID: 67
    /// </summary>
    public string? LangnameUngekuerzt { get; set; }

    /// <summary>
    /// BTM indicator (Betäubungsmittel - Controlled substances)
    /// Field ID: 08
    /// Values: 0=None, 1=No, 2=BTM, 3=Exempt preparation, 4=T-Rezept
    /// </summary>
    public byte Btm { get; set; }

    /// <summary>
    /// Cannabis indicator (Cannabis as medicine)
    /// Field ID: G8
    /// Values: 0=None, 1=No, 2=Cannabis MedCanG §2 Nr.1, 3=Cannabis MedCanG §2 Nr.2
    /// </summary>
    public byte Cannabis { get; set; }

    /// <summary>
    /// Pharmacy purchase price (AEK) in cents, excl. VAT
    /// Field ID: 02
    /// </summary>
    public long ApoEk { get; set; }

    /// <summary>
    /// Pharmacy sales price (AVK) in cents, incl. VAT
    /// Field ID: 04
    /// </summary>
    public long ApoVk { get; set; }

    /// <summary>
    /// Pharmaceutical manufacturer price (ApU) in cents, excl. VAT
    /// Field ID: 18
    /// </summary>
    public long ApU { get; set; }

    /// <summary>
    /// Pharmaceutical manufacturer price per § 78 (3a) Satz 1 AMG (Reimbursement price)
    /// Field ID: C0
    /// </summary>
    public long? ApU_78_3a_1_Amg { get; set; }

    /// <summary>
    /// Fixed price (Festbetrag) in cents
    /// Field ID: 97
    /// </summary>
    public long? Festbetrag { get; set; }

    /// <summary>
    /// Market status (Verkehrsfähigkeitsstatus)
    /// Field ID: 52
    /// Values: 01=Available, 02=Out of trade, 03=Authorization revoked, etc.
    /// </summary>
    public string Verkehrsstatus { get; set; } = string.Empty;

    /// <summary>
    /// Prescription requirement (Verschreibungspflicht)
    /// Field ID: 54
    /// Values: 1=OTC (pharmacy only), 2=Prescription, 3=BTM prescription, etc.
    /// </summary>
    public byte Rezeptpflicht { get; set; }

    /// <summary>
    /// VAT rate indicator
    /// Field ID: 37
    /// Values: 1=19%, 2=7%, 3=0%
    /// </summary>
    public byte MwSt { get; set; }

    /// <summary>
    /// Lifestyle medication indicator
    /// Field ID: 81
    /// Values: 0=None, 1=No, 2=Yes
    /// </summary>
    public byte Lifestyle { get; set; }

    /// <summary>
    /// Transfusion law (T-Rezept) indicator
    /// Field ID: 50
    /// Values: 0=None, 1=No, 2=Yes
    /// </summary>
    public byte Tfg { get; set; }

    /// <summary>
    /// Pharmacy-only indicator
    /// Field ID: 03
    /// Values: 0=None, 1=No, 2=Yes
    /// </summary>
    public byte Apopflicht { get; set; }

    // Computed properties for easier access

    public bool IsBtm => Btm == 2;
    public bool IsBtmExempt => Btm == 3;
    public bool IsTRezept => Btm == 4 || Tfg == 2;
    public bool IsCannabis => Cannabis is 2 or 3;
    public bool IsLifestyleMedication => Lifestyle == 2;
    public bool IsAvailableOnMarket => Verkehrsstatus == "01";
    public bool RequiresPrescription => Rezeptpflicht >= 2;
    public bool RequiresBtmPrescription => Rezeptpflicht == 3;

    /// <summary>
    /// Get pharmacy sales price in euros (converts from cents)
    /// </summary>
    public decimal ApoVkEuros => ApoVk / 100.0m;

    /// <summary>
    /// Get pharmacy purchase price in euros (converts from cents)
    /// </summary>
    public decimal ApoEkEuros => ApoEk / 100.0m;

    /// <summary>
    /// Get fixed price in euros (if available)
    /// </summary>
    public decimal? FestbetragEuros => Festbetrag.HasValue ? Festbetrag.Value / 100.0m : null;

    /// <summary>
    /// Get VAT rate percentage
    /// </summary>
    public decimal VatRatePercentage => MwSt switch
    {
        1 => 19.0m,
        2 => 7.0m,
        3 => 0.0m,
        _ => 19.0m // Default
    };
}
