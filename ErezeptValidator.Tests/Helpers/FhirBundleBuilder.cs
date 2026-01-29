using Hl7.Fhir.Model;

namespace ErezeptValidator.Tests.Helpers;

/// <summary>
/// Fluent builder for creating FHIR Bundles for testing
/// </summary>
public class FhirBundleBuilder
{
    private readonly Bundle _bundle;
    private readonly List<Bundle.EntryComponent> _entries = new();

    public FhirBundleBuilder()
    {
        _bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString(),
            Type = Bundle.BundleType.Document,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    public FhirBundleBuilder WithId(string id)
    {
        _bundle.Id = id;
        return this;
    }

    public FhirBundleBuilder WithMedicationRequest(string pzn, string medicationRequestId = "test-med-request")
    {
        var medRequest = new MedicationRequest
        {
            Id = medicationRequestId,
            Status = MedicationRequest.MedicationrequestStatus.Active,
            Intent = MedicationRequest.MedicationRequestIntent.Order,
            Medication = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "http://fhir.de/CodeSystem/ifa/pzn",
                        Code = pzn
                    }
                }
            }
        };

        _entries.Add(new Bundle.EntryComponent
        {
            Resource = medRequest
        });

        return this;
    }

    public FhirBundleBuilder WithPatient(string patientId, string familyName, string givenName, string insuranceNumber)
    {
        var patient = new Patient
        {
            Id = patientId,
            Name = new List<HumanName>
            {
                new HumanName
                {
                    Use = HumanName.NameUse.Official,
                    Family = familyName,
                    Given = new[] { givenName }
                }
            },
            Identifier = new List<Identifier>
            {
                new Identifier
                {
                    Type = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding { Code = "GKV" }
                        }
                    },
                    Value = insuranceNumber
                }
            }
        };

        _entries.Add(new Bundle.EntryComponent
        {
            Resource = patient
        });

        return this;
    }

    public FhirBundleBuilder WithPractitioner(string practitionerId, string lanr, string familyName)
    {
        var practitioner = new Practitioner
        {
            Id = practitionerId,
            Identifier = new List<Identifier>
            {
                new Identifier
                {
                    Type = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding { Code = "LANR" }
                        }
                    },
                    Value = lanr
                }
            },
            Name = new List<HumanName>
            {
                new HumanName
                {
                    Use = HumanName.NameUse.Official,
                    Family = familyName
                }
            }
        };

        _entries.Add(new Bundle.EntryComponent
        {
            Resource = practitioner
        });

        return this;
    }

    public Bundle Build()
    {
        _bundle.Entry = _entries;
        return _bundle;
    }

    /// <summary>
    /// Creates a minimal valid E-Rezept bundle for testing
    /// </summary>
    public static Bundle CreateMinimalBundle(string pzn)
    {
        return new FhirBundleBuilder()
            .WithId("test-bundle-" + Guid.NewGuid().ToString())
            .WithMedicationRequest(pzn)
            .Build();
    }
}
