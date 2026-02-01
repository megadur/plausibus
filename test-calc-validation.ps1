# Test CALC-004 to CALC-007 Validation Rules
# PowerShell script to test calculation validators with sample FHIR bundles

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing CALC-004 to CALC-007 Validation" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "https://localhost:7001"
$apiEndpoint = "$baseUrl/api/validation/e-rezept"

# Test cases
$testCases = @(
    @{
        Name = "Standard PZN Prescription (CALC-004 test)"
        File = "docs/eRezept-Beispiele/PZN-Verordnung_Nr_1/PZN_Nr1_eAbgabedaten.xml"
        ExpectedRules = @("CALC-004", "Factor calculation", "Price calculation")
        Description = "Tests basic price calculation: Price = (Factor / 1000) × Base_Price"
    },
    @{
        Name = "Artificial Insemination (CALC-003 test)"
        File = "docs/eRezept-Beispiele/PZN-Verordnung_Künstliche_Befruchtung/PZN-Verordnung_Künstliche_Befruchtung_V1/PZN_KB_V1_eAbgabedaten.xml"
        ExpectedRules = @("CALC-003", "Artificial insemination", "Factor 1000", "Price 0.00")
        Description = "Tests artificial insemination special code: Factor=1000, Price=0.00, PriceId=90"
    },
    @{
        Name = "Compounding Prescription (CALC-005 test)"
        File = "docs/eRezept-Beispiele/Rezeptur-Verordnung_Nr_1/Rez_Nr1_eAbgabedaten.xml"
        ExpectedRules = @("CALC-005", "VAT exclusion", "Compounding")
        Description = "Tests VAT exclusion for compounding: Price should exclude VAT"
    }
)

# Function to test validation
function Test-Validation {
    param(
        [string]$FilePath,
        [string]$TestName,
        [string]$Description
    )

    if (-not (Test-Path $FilePath)) {
        Write-Host "❌ File not found: $FilePath" -ForegroundColor Red
        return
    }

    Write-Host "📋 Test: $TestName" -ForegroundColor Yellow
    Write-Host "   Description: $Description" -ForegroundColor Gray
    Write-Host "   File: $FilePath" -ForegroundColor Gray

    try {
        $content = Get-Content $FilePath -Raw

        $response = Invoke-WebRequest -Uri $apiEndpoint `
            -Method POST `
            -Body $content `
            -ContentType "application/fhir+xml" `
            -SkipCertificateCheck `
            -ErrorAction Stop

        $result = $response.Content | ConvertFrom-Json

        Write-Host "   ✅ HTTP $($response.StatusCode) - Validation completed" -ForegroundColor Green
        Write-Host ""

        # Display validation summary
        $totalValidators = $result.Length
        $totalErrors = ($result | ForEach-Object { $_.errorCount } | Measure-Object -Sum).Sum
        $totalWarnings = ($result | ForEach-Object { $_.warningCount } | Measure-Object -Sum).Sum

        Write-Host "   📊 Summary:" -ForegroundColor Cyan
        Write-Host "      Validators run: $totalValidators" -ForegroundColor White
        Write-Host "      Total errors: $totalErrors" -ForegroundColor $(if ($totalErrors -gt 0) { "Red" } else { "Green" })
        Write-Host "      Total warnings: $totalWarnings" -ForegroundColor Yellow
        Write-Host ""

        # Display results per validator
        foreach ($validator in $result) {
            $icon = if ($validator.errorCount -eq 0 -and $validator.warningCount -eq 0) { "✅" }
                   elseif ($validator.errorCount -gt 0) { "❌" }
                   else { "⚠️" }

            Write-Host "      $icon $($validator.validatorName):" -ForegroundColor White

            if ($validator.errors -and $validator.errors.Count -gt 0) {
                foreach ($error in $validator.errors) {
                    Write-Host "         🔴 [$($error.code)] $($error.message)" -ForegroundColor Red
                }
            }

            if ($validator.warnings -and $validator.warnings.Count -gt 0) {
                foreach ($warning in $validator.warnings) {
                    Write-Host "         🟡 [$($warning.code)] $($warning.message)" -ForegroundColor Yellow
                }
            }

            if ($validator.errorCount -eq 0 -and $validator.warningCount -eq 0) {
                Write-Host "         All checks passed" -ForegroundColor Green
            }
        }

        Write-Host ""
        Write-Host "   " + ("─" * 80) -ForegroundColor DarkGray
        Write-Host ""

    } catch {
        Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $errorBody = $reader.ReadToEnd()
            Write-Host "   Response: $errorBody" -ForegroundColor Red
        }
        Write-Host ""
    }
}

# Check if API is running
Write-Host "🔍 Checking if API is running at $baseUrl..." -ForegroundColor Cyan
try {
    $healthCheck = Invoke-WebRequest -Uri "$baseUrl/health" -SkipCertificateCheck -ErrorAction Stop
    Write-Host "✅ API is running" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "❌ API is not running. Please start the API first:" -ForegroundColor Red
    Write-Host "   cd ErezeptValidator" -ForegroundColor Yellow
    Write-Host "   dotnet run" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

# Run tests
foreach ($test in $testCases) {
    Test-Validation -FilePath $test.File -TestName $test.Name -Description $test.Description
    Start-Sleep -Seconds 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Complete" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
