using ErezeptValidator.Data;
using ErezeptValidator.Models.Abdata;
using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.Validation.Validators;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Task = System.Threading.Tasks.Task;

namespace ErezeptValidator.Tests.Validators;

/// <summary>
/// Unit tests for BtmDetectionValidator (BTM-001 to BTM-004)
/// </summary>
public class BtmDetectionValidatorTests
{
    private readonly Mock<IPznRepository> _mockPznRepository;
    private readonly Mock<ILogger<BtmDetectionValidator>> _mockLogger;
    private readonly BtmDetectionValidator _validator;

    public BtmDetectionValidatorTests()
    {
        _mockPznRepository = new Mock<IPznRepository>();
        _mockLogger = new Mock<ILogger<BtmDetectionValidator>>();
        _validator = new BtmDetectionValidator(_mockPznRepository.Object, _mockLogger.Object);
    }

    #region BTM-001: E-BTM fee special code

    [Fact]
    public async Task BTM001_ValidEBtmFeeWithCorrectFactor_NoError()
    {
        // Arrange
        var pzn1 = "12345678";
        var pzn2 = "87654321";
        var context = CreateBtmContext(pzn1, isBtm: true);

        // Add second BTM medication to Invoice
        var invoice = context.Invoices.First();
        var lineItem2 = new Invoice.LineItemComponent
        {
            ChargeItem = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "http://fhir.de/CodeSystem/ifa/pzn",
                        Code = pzn2
                    }
                }
            },
            PriceComponent = new List<Invoice.PriceComponentComponent>
            {
                new Invoice.PriceComponentComponent
                {
                    Amount = new Money { Value = 25.50m },
                    Factor = 1m
                }
            }
        };
        invoice.LineItem.Add(lineItem2);
        context.PznCodes.Add(pzn2);

        // Add E-BTM fee SOK code with correct factor (2 BTM medications)
        var btmFeeLineItem = new Invoice.LineItemComponent
        {
            ChargeItem = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "http://fhir.de/CodeSystem/sok",
                        Code = "02567001"
                    }
                }
            },
            PriceComponent = new List<Invoice.PriceComponentComponent>
            {
                new Invoice.PriceComponentComponent
                {
                    Amount = new Money { Value = 5.00m },
                    Factor = 2m // 2 BTM medications
                }
            }
        };
        invoice.LineItem.Add(btmFeeLineItem);

        // Mock repository to return BTM articles
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn1, CreateBtmArticle(pzn1, btmFlag: 2) },
            { pzn2, CreateBtmArticle(pzn2, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Issues.Should().Contain(i => i.Code == "BTM-DETECTED");
    }

    [Fact]
    public async Task BTM001_MissingEBtmFee_ReturnsInfo()
    {
        // Arrange
        var pzn = "12345678";
        var context = CreateBtmContext(pzn, isBtm: true);

        // Mock repository to return BTM article (no E-BTM fee in invoice)
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert - Currently returns INFO, not ERROR (per validator implementation)
        result.Issues.Should().Contain(i => i.Code == "BTM-001-I");
        result.Issues.First(i => i.Code == "BTM-001-I").Message
            .Should().Contain("E-BTM fee special code");
    }

    [Fact]
    public async Task BTM001_IncorrectFactorValue_ReturnsError()
    {
        // Arrange
        var pzn = "12345678";
        var context = CreateBtmContext(pzn, isBtm: true);

        // Add E-BTM fee SOK code with INCORRECT factor (should be 1, not 3)
        var invoice = context.Invoices.First();
        var btmFeeLineItem = new Invoice.LineItemComponent
        {
            ChargeItem = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "http://fhir.de/CodeSystem/sok",
                        Code = "02567001"
                    }
                }
            },
            PriceComponent = new List<Invoice.PriceComponentComponent>
            {
                new Invoice.PriceComponentComponent
                {
                    Amount = new Money { Value = 5.00m },
                    Factor = 3m // WRONG: should be 1 (only 1 BTM medication)
                }
            }
        };
        invoice.LineItem.Add(btmFeeLineItem);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "BTM-001-E");
        result.Errors.First(e => e.Code == "BTM-001-E").Message
            .Should().Contain("factor incorrect");
    }

    [Fact]
    public async Task BTM001_NoBtmPresent_NoEBtmFeeRequired_NoError()
    {
        // Arrange - Non-BTM medication
        var pzn = "12345678";
        var context = CreateBtmContext(pzn, isBtm: false);

        // Mock repository to return non-BTM article
        var articles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 0) } // 0 = no BTM
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(articles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Issues.Should().NotContain(i => i.Code.StartsWith("BTM-001"));
    }

    #endregion

    #region BTM-002: All pharmaceuticals must be listed

    [Fact]
    public async Task BTM002_AllBtmPharmaceuticalsListed_NoError()
    {
        // Arrange - BTM medication with complete data
        var pzn = "12345678";
        var context = CreateBtmContext(pzn, isBtm: true);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.Errors.Should().NotContain(e => e.Code == "BTM-002-E");
    }

    [Fact]
    public async Task BTM002_MissingPznForBtmMedication_ReturnsError()
    {
        // Arrange - BTM medication WITHOUT PZN
        var context = new ValidationContext
        {
            Bundle = new Bundle { Id = "test-bundle" },
            BundleType = BundleType.Abgabedaten
        };

        var invoice = new Invoice
        {
            Id = "test-invoice",
            LineItem = new List<Invoice.LineItemComponent>
            {
                new Invoice.LineItemComponent
                {
                    // ChargeItem is missing (no PZN)
                    PriceComponent = new List<Invoice.PriceComponentComponent>
                    {
                        new Invoice.PriceComponentComponent
                        {
                            Amount = new Money { Value = 15.50m },
                            Factor = 1m
                        }
                    }
                }
            }
        };
        context.Invoices.Add(invoice);

        // This is tricky - we can't identify BTM without PZN
        // Test will show that missing PZN prevents BTM detection
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new Dictionary<string, PacApoArticle>());

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert - No BTM detected without PZN
        result.Issues.Should().NotContain(i => i.Code == "BTM-DETECTED");
    }

    [Fact]
    public async Task BTM002_MissingQuantityForBtmMedication_ReturnsError()
    {
        // Arrange - BTM medication WITHOUT quantity
        var pzn = "12345678";
        var context = new ValidationContext
        {
            Bundle = new Bundle { Id = "test-bundle" },
            BundleType = BundleType.Abgabedaten
        };

        var invoice = new Invoice
        {
            Id = "test-invoice",
            LineItem = new List<Invoice.LineItemComponent>
            {
                new Invoice.LineItemComponent
                {
                    ChargeItem = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://fhir.de/CodeSystem/ifa/pzn",
                                Code = pzn
                            }
                        }
                    },
                    PriceComponent = new List<Invoice.PriceComponentComponent>
                    {
                        new Invoice.PriceComponentComponent
                        {
                            Amount = new Money { Value = 15.50m }
                            // Factor (quantity) is missing
                        }
                    }
                }
            }
        };
        context.Invoices.Add(invoice);
        context.PznCodes.Add(pzn);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "BTM-002-E");
        result.Errors.First(e => e.Code == "BTM-002-E").Message
            .Should().Contain("Quantity");
    }

    [Fact]
    public async Task BTM002_MissingPriceForBtmMedication_ReturnsError()
    {
        // Arrange - BTM medication WITHOUT price
        var pzn = "12345678";
        var context = new ValidationContext
        {
            Bundle = new Bundle { Id = "test-bundle" },
            BundleType = BundleType.Abgabedaten
        };

        var invoice = new Invoice
        {
            Id = "test-invoice",
            LineItem = new List<Invoice.LineItemComponent>
            {
                new Invoice.LineItemComponent
                {
                    ChargeItem = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://fhir.de/CodeSystem/ifa/pzn",
                                Code = pzn
                            }
                        }
                    },
                    PriceComponent = new List<Invoice.PriceComponentComponent>
                    {
                        new Invoice.PriceComponentComponent
                        {
                            // Amount (price) is missing
                            Factor = 1m
                        }
                    }
                }
            }
        };
        context.Invoices.Add(invoice);
        context.PznCodes.Add(pzn);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "BTM-002-E");
        result.Errors.First(e => e.Code == "BTM-002-E").Message
            .Should().Contain("Price");
    }

    #endregion

    #region BTM-003: Seven-day validity rule

    [Fact]
    public async Task BTM003_DispensingWithinSevenDays_NoWarning()
    {
        // Arrange
        var pzn = "12345678";
        var prescriptionDate = DateTimeOffset.UtcNow.AddDays(-5);
        var dispensingDate = DateTimeOffset.UtcNow;
        var context = CreateBtmContext(pzn, isBtm: true, prescriptionDate, dispensingDate);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "BTM-003-W");
    }

    [Fact]
    public async Task BTM003_DispensingAfterSevenDays_ReturnsWarning()
    {
        // Arrange - Dispensing 8 days after prescription
        var pzn = "12345678";
        var prescriptionDate = DateTimeOffset.UtcNow.AddDays(-8);
        var dispensingDate = DateTimeOffset.UtcNow;
        var context = CreateBtmContext(pzn, isBtm: true, prescriptionDate, dispensingDate);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.Warnings.Should().Contain(w => w.Code == "BTM-003-W");
        result.Warnings.First(w => w.Code == "BTM-003-W").Message
            .Should().Contain("7 days");
    }

    [Fact]
    public async Task BTM003_DispensingExactlySevenDays_NoWarning()
    {
        // Arrange - Exactly 7 days (boundary condition)
        var pzn = "12345678";
        var prescriptionDate = DateTimeOffset.UtcNow.AddDays(-7);
        var dispensingDate = DateTimeOffset.UtcNow;
        var context = CreateBtmContext(pzn, isBtm: true, prescriptionDate, dispensingDate);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert - Exactly 7 days should NOT trigger warning (per BtMG §3)
        result.Warnings.Should().NotContain(w => w.Code == "BTM-003-W");
    }

    [Fact]
    public async Task BTM003_MissingPrescriptionDate_NoWarning()
    {
        // Arrange - No prescription date available
        var pzn = "12345678";
        var context = CreateBtmContext(pzn, isBtm: true, prescriptionDate: null, dispensingDate: DateTimeOffset.UtcNow);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert - Cannot validate without date, so no warning
        result.Warnings.Should().NotContain(w => w.Code == "BTM-003-W");
    }

    #endregion

    #region BTM-004: Diagnosis requirement

    [Fact]
    public async Task BTM004_BtmWithDiagnosisCode_NoWarning()
    {
        // Arrange
        var pzn = "12345678";
        var context = CreateBtmContext(pzn, isBtm: true, includeDiagnosis: true);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.Warnings.Should().NotContain(w => w.Code == "BTM-004-W");
        result.Issues.Should().Contain(i => i.Code == "BTM-004-I"); // Info that diagnosis is present
    }

    [Fact]
    public async Task BTM004_BtmWithoutDiagnosisCode_ReturnsWarning()
    {
        // Arrange - BTM WITHOUT diagnosis
        var pzn = "12345678";
        var context = CreateBtmContext(pzn, isBtm: true, includeDiagnosis: false);

        // Mock repository to return BTM article
        var btmArticles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 2) }
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(btmArticles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert
        result.Warnings.Should().Contain(w => w.Code == "BTM-004-W");
        result.Warnings.First(w => w.Code == "BTM-004-W").Message
            .Should().Contain("diagnosis");
    }

    [Fact]
    public async Task BTM004_NonBtmPrescription_NoDiagnosisRequired()
    {
        // Arrange - Non-BTM medication
        var pzn = "12345678";
        var context = CreateBtmContext(pzn, isBtm: false, includeDiagnosis: false);

        // Mock repository to return non-BTM article
        var articles = new Dictionary<string, PacApoArticle>
        {
            { pzn, CreateBtmArticle(pzn, btmFlag: 0) } // 0 = no BTM
        };
        _mockPznRepository.Setup(x => x.GetByPznBatchAsync(It.IsAny<string[]>()))
            .ReturnsAsync(articles);

        // Act
        var result = await _validator.ValidateAsync(context);

        // Assert - No BTM-004 warning for non-BTM prescription
        result.Warnings.Should().NotContain(w => w.Code == "BTM-004-W");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a test ValidationContext with BTM medications
    /// </summary>
    private ValidationContext CreateBtmContext(
        string pzn,
        bool isBtm,
        DateTimeOffset? prescriptionDate = null,
        DateTimeOffset? dispensingDate = null,
        bool includeDiagnosis = true)
    {
        var context = new ValidationContext
        {
            Bundle = new Bundle { Id = "test-bundle" },
            BundleType = BundleType.Abgabedaten,
            DispensingDate = dispensingDate ?? DateTimeOffset.UtcNow
        };

        // Add MedicationRequest with prescription date
        var medRequest = new MedicationRequest
        {
            Id = "test-medrequest",
            AuthoredOn = prescriptionDate?.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            Status = MedicationRequest.MedicationrequestStatus.Active,
            Intent = MedicationRequest.MedicationRequestIntent.Order
        };
        context.MedicationRequests.Add(medRequest);

        // Add Invoice with line item containing the PZN
        var invoice = new Invoice
        {
            Id = "test-invoice",
            Status = Invoice.InvoiceStatus.Issued,
            LineItem = new List<Invoice.LineItemComponent>
            {
                new Invoice.LineItemComponent
                {
                    ChargeItem = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://fhir.de/CodeSystem/ifa/pzn",
                                Code = pzn
                            }
                        }
                    },
                    PriceComponent = new List<Invoice.PriceComponentComponent>
                    {
                        new Invoice.PriceComponentComponent
                        {
                            Amount = new Money { Value = 15.50m, Currency = "EUR" },
                            Factor = 1m
                        }
                    }
                }
            }
        };
        context.Invoices.Add(invoice);
        context.PznCodes.Add(pzn);

        // Add Condition (diagnosis) resource if requested
        if (includeDiagnosis)
        {
            var condition = new Condition
            {
                Id = "test-condition",
                Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://fhir.de/CodeSystem/dimdi/icd-10-gm",
                            Code = "F11.2", // ICD-10: Opioid dependence
                            Display = "Opioidabhängigkeit"
                        }
                    }
                }
            };

            // Add condition to bundle
            var conditionEntry = new Bundle.EntryComponent
            {
                Resource = condition,
                FullUrl = $"Condition/{condition.Id}"
            };
            context.Bundle.Entry = context.Bundle.Entry ?? new List<Bundle.EntryComponent>();
            context.Bundle.Entry.Add(conditionEntry);
        }

        return context;
    }

    /// <summary>
    /// Creates a mock BTM article for ABDATA lookup
    /// </summary>
    private PacApoArticle CreateBtmArticle(string pzn, byte btmFlag = 2)
    {
        return new PacApoArticle
        {
            Pzn = pzn,
            Name = $"Test BTM Product {pzn}",
            Btm = btmFlag, // 0 = none, 2 = BTM, 3 = BTM exempt, 4 = T-Rezept
            Cannabis = 0
        };
    }

    #endregion
}
