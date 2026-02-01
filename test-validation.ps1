# PowerShell script to test Prescription Validation API
# Usage: .\test-validation.ps1

Write-Host "E-Rezept Validator - Validation API Test Script" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

$validationUrl = "https://localhost:7001/api/v1/prescriptions/validate"

# Helper function to run a test
function Test-Validation {
    param (
        [string]$Name,
        [string]$FilePath,
        [string]$ExpectedType
    )

    Write-Host "Test: $Name" -ForegroundColor Yellow
    Write-Host "  File: $FilePath" -ForegroundColor Gray
    
    if (-not (Test-Path $FilePath)) {
        Write-Host "✗ File not found!" -ForegroundColor Red
        return
    }

    $xmlContent = Get-Content -Path $FilePath -Raw
    
    try {
        $response = Invoke-RestMethod -Uri $validationUrl -Method Post -Body $xmlContent -ContentType "application/xml" -SkipCertificateCheck
        
        # Check validation status
        if ($response.isValid) {
            Write-Host "✓ Valid ($($response.bundleType))" -ForegroundColor Green
        } else {
            Write-Host "⚠ Valid with Issues ($($response.bundleType))" -ForegroundColor Yellow
        }
        
        # Verify bundle type
        if ($response.bundleType -eq $ExpectedType) {
             Write-Host "  Bundle Type: Match ($($response.bundleType))" -ForegroundColor Green
        } else {
             Write-Host "  Bundle Type: Mismatch! Expected $ExpectedType, got $($response.bundleType)" -ForegroundColor Red
        }

        # Show stats
        Write-Host "  Errors: $($response.errorCount)" -ForegroundColor White
        Write-Host "  Warnings: $($response.warningCount)" -ForegroundColor White
        
        # Show first few errors if any
        if ($response.errors) {
            foreach ($err in $response.errors | Select-Object -First 3) {
                 Write-Host "    [E] $($err.ruleId): $($err.message)" -ForegroundColor Red
            }
        }
        
        # Show first few warnings if any
         if ($response.warnings) {
            foreach ($warn in $response.warnings | Select-Object -First 3) {
                 Write-Host "    [W] $($warn.ruleId): $($warn.message)" -ForegroundColor DarkYellow
            }
        }
        
    } catch {
        Write-Host "✗ Request Failed: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
             $reader = New-Object System.IO.StreamReader $_.Exception.Response.GetResponseStream()
             $errBody = $reader.ReadToEnd()
             Write-Host "  Response: $errBody" -ForegroundColor Red
        }
    }
    Write-Host ""
}

# 1. Test Prescription Bundle (VerordnungArzt)
Test-Validation -Name "Simple Prescription (PZN Nr. 1)" `
    -FilePath "docs\eRezept-Beispiele\PKV\PZN-Verordnung_Nr_1\PZN_Nr1_VerordnungArzt.xml" `
    -ExpectedType "Prescription"

# 2. Test Abgabedaten Bundle (eAbgabedaten)
Test-Validation -Name "Simple Abgabedaten (PZN Nr. 1)" `
    -FilePath "docs\eRezept-Beispiele\PKV\PZN-Verordnung_Nr_1\PZN_Nr1_eAbgabedaten.xml" `
    -ExpectedType "Abgabedaten"

# 3. Test Complex Recipe (Rezeptur)
Test-Validation -Name "Complex Recipe (Compounding)" `
    -FilePath "docs\eRezept-Beispiele\PKV\Rezeptur-Verordnung_Nr_1\Rez_Nr1_eAbgabedaten.xml" `
    -ExpectedType "Abgabedaten"

