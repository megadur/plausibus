# Architectural Review and Evaluation

**Project:** Plausibus: eRezept-Validator
**Date:** 2026-01-24
**Evaluator:** Jules

---

## 1. Executive Summary

The project is a .NET 10 (likely .NET 8/9 compatible) REST API designed to validate electronic prescriptions (E-Rezept) against German healthcare regulations (TA1, BtMG, etc.).

**Current Status:**
- **Foundation Layer:** ✅ Implemented. Data access to ABDATA (SQL Server) and TA1 Reference Data (PostgreSQL) is established.
- **Data Access:** ✅ Implemented. `PznRepository` efficiently retrieves product data using Dapper and caches it.
- **Validation Logic:** ⚠️ Partial. Basic PZN format and checksum validation exists. The comprehensive "Validation Engine" described in the specs is **missing**.
- **API Layer:** ⚠️ MVP. Currently exposes a `PznTestController` for testing data access, but lacks the formal validation endpoints.

---

## 2. Architecture Overview

The application follows a **Layered Architecture** pattern:

```mermaid
graph TD
    Client[Client / External System] --> API[API Layer (Controllers)]
    API --> Services[Service Layer (Validation Engine)]
    Services --> Repo[Data Access Layer (Repositories)]
    Repo --> Cache[In-Memory Cache]
    Repo --> DB1[(ABDATA - SQL Server)]
    Repo --> DB2[(TA1 Ref - PostgreSQL)]
```

### 2.1 Key Components

*   **Presentation Layer (`ErezeptValidator/Controllers`)**:
    *   Currently contains `PznTestController`.
    *   **Recommendation:** Rename/Refactor to `ValidationController` for the actual product.
*   **Data Access Layer (`ErezeptValidator/Data`)**:
    *   `PznRepository`: Uses **Dapper** for high-performance read operations from the legacy/production ABDATA database. This is a strong architectural choice for read-heavy workloads.
    *   `Ta1DbContext`: Uses **Entity Framework Core** with PostgreSQL for managing reference data (TA1 rules, likely user/config data). Valid choice for structured, relational data that might need migration management.
*   **Models (`ErezeptValidator/Models`)**:
    *   `PacApoArticle`: Accurately maps to the ABDATA `PAC_APO` table definition.
*   **Infrastructure**:
    *   **Caching:** In-memory caching (`IMemoryCache`) with 24h TTL is implemented for PZN lookups.
    *   **Docker:** Containerization support is present.
    *   **CI/CD:** GitHub Actions for Build, Test, and Deploy.

---

## 3. Detailed Evaluation

### 3.1 Strengths (Pros)

1.  **Performance-Oriented Data Access:** The use of **Dapper** for fetching Article data is excellent. ABDATA databases can be large, and the overhead of EF Core change tracking is unnecessary for read-only reference data.
2.  **Caching Strategy:** Caching PZNs for 24 hours is appropriate given the daily update cycle of ABDATA. This will significantly reduce database load.
3.  **Clean Separation of Concerns:** The distinction between the external reference database (ABDATA) and the internal application database (TA1 Ref) is well-handled.
4.  **Documentation:** The project includes high-quality documentation (`ABDATA-Database-Integration-Plan.md`, `TA1-Validation-Rules-Technical-Specification.md`) which clearly defines the requirements.
5.  **Modern Stack:** Use of .NET 6+ features (implied by syntax), generic host builder, and OpenAPI (Swagger).

### 3.2 Weaknesses (Cons) & Risks

1.  **Missing Validation Engine:** The core business logic defined in `TA1-Validation-Rules-Technical-Specification.md` (e.g., `ValidationEngine`, `BtmValidator`, `PriceValidator`) is not yet implemented. The project is currently more of a "PZN Lookup Service" than a "Validator".
2.  **Test Controller in Production:** `PznTestController` exposes internal details and debugging endpoints. It should not be part of the final release.
3.  **Hardcoded SQL:** While Dapper is fast, the SQL queries are hardcoded in strings. As the queries get more complex (joins with `VOV_APO`, `VPV_APO`), this might become hard to maintain.
4.  **Database Dependency Injection:** `PznRepository` instantiates `SqlConnection` directly. It would be better to inject a `IDbConnectionFactory` to facilitate unit testing (mocking the database).
5.  **Magic Strings:** Connection string names ("AbdataDatabase", "Ta1ReferenceDatabase") are hardcoded in multiple places.

---

## 4. Gap Analysis (vs. Technical Specification)

The `TA1-Validation-Rules-Technical-Specification.md` outlines a sophisticated validation suite. Here is the implementation status:

| Component | Spec Reference | Status |
|-----------|----------------|--------|
| **PZN Format Validation** | FMT-001, FMT-002 | ✅ Implemented |
| **PZN Data Lookup** | Data Access | ✅ Implemented |
| **BTM/Cannabis Flags** | BTM-001, CAN-001 | ✅ Data available, Logic missing |
| **Validation Engine** | Section 13.1 | ❌ **Missing** |
| **Timestamp Validation** | FMT-003, GEN-001 | ❌ Missing |
| **Price Calculation** | CALC-001 to CALC-007 | ❌ Missing |
| **Compounding Rules** | REZ-xxx | ❌ Missing |
| **Error Response Format** | Section 12.2 | ❌ Missing (Uses simple JSON) |

---

## 5. Recommendations & Roadmap

### Phase 1: Core Refactoring (Immediate)
1.  **Remove `PznTestController`** or move it to a test project.
2.  **Implement `ValidationController`** that accepts an `ErezeptBundle` (FHIR JSON).
3.  **Refactor `PznRepository`** to use an `IDbConnectionFactory` for better testability.

### Phase 2: Implement Validation Engine (Next 2 Weeks)
1.  Create the `ValidationEngine` service that orchestrates the validation flow.
2.  Implement the **Pipeline Pattern** or **Chain of Responsibility** for the validators (Format -> General -> Specific).
3.  Implement `BtmValidator` and `CannabisValidator` using the data already available in `PacApoArticle`.

### Phase 3: Advanced Features
1.  Implement `PriceValidator` (checking AEK/AVK vs. billed prices).
2.  Implement `CompoundingValidator` (complex logic for recipes).

### Phase 4: Infrastructure
1.  Set up the PostgreSQL database for TA1 reference data.
2.  Consider moving hardcoded SQL to a separate provider or stored procedures if complexity increases.

---

## 6. Conclusion

The project has a solid foundation with a highly performant data access layer. The architectural decisions made so far (Dapper for reads, Caching) are sound. The major remaining task is to implement the **Business Logic Layer (Validation Engine)** according to the excellent technical specifications provided. The project is well-positioned to succeed once the validation logic is filled in.
