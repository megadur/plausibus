# PowerShell script to test Code Reference API endpoints
# Usage: .\test-api.ps1

Write-Host "E-Rezept Validator - Code Reference API Test Script" -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "https://localhost:7001/api/v1/codes"

# Test 1: Get statistics
Write-Host "Test 1: GET /api/v1/codes/stats" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/stats" -Method Get -SkipCertificateCheck
    Write-Host "✓ Success!" -ForegroundColor Green
    Write-Host "  Total codes: $($response.totalCodes)" -ForegroundColor White
    Write-Host "  SOK codes: $($response.specialCodes.total) (SOK1: $($response.specialCodes.sok1), SOK2: $($response.specialCodes.sok2))" -ForegroundColor White
    Write-Host "  Factor codes: $($response.factorCodes)" -ForegroundColor White
    Write-Host "  Price codes: $($response.priceCodes)" -ForegroundColor White
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Get all factor codes
Write-Host "Test 2: GET /api/v1/codes/factors" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/factors" -Method Get -SkipCertificateCheck
    Write-Host "✓ Success! Found $($response.count) factor codes:" -ForegroundColor Green
    foreach ($code in $response.codes) {
        Write-Host "  - Code $($code.code): $($code.content) ($($code.description))" -ForegroundColor White
    }
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Get all price codes
Write-Host "Test 3: GET /api/v1/codes/prices" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/prices" -Method Get -SkipCertificateCheck
    Write-Host "✓ Success! Found $($response.count) price codes:" -ForegroundColor Green
    foreach ($code in $response.codes) {
        Write-Host "  - Code $($code.code): $($code.content) ($($code.taxStatus))" -ForegroundColor White
    }
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Get specific SOK code (valid)
Write-Host "Test 4: GET /api/v1/codes/sok/02566958 (valid SOK code)" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/sok/02566958" -Method Get -SkipCertificateCheck
    Write-Host "✓ Success!" -ForegroundColor Green
    Write-Host "  Code: $($response.code)" -ForegroundColor White
    Write-Host "  Type: $($response.codeType)" -ForegroundColor White
    Write-Host "  Description: $($response.description)" -ForegroundColor White
    Write-Host "  E-Rezept compatible: $($response.eRezeptCompatible)" -ForegroundColor White
    Write-Host "  VAT rate: $($response.vatRate) ($($response.vatPercentage)%)" -ForegroundColor White
    Write-Host "  Currently valid: $($response.validity.isCurrentlyValid)" -ForegroundColor White
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 5: Get specific SOK code (invalid)
Write-Host "Test 5: GET /api/v1/codes/sok/99999999 (invalid SOK code)" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/sok/99999999" -Method Get -SkipCertificateCheck
    Write-Host "✗ Should have returned 404, but got success" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode -eq 404) {
        Write-Host "✓ Correctly returned 404 Not Found" -ForegroundColor Green
    } else {
        Write-Host "✗ Unexpected error: $($_.Exception.Message)" -ForegroundColor Red
    }
}
Write-Host ""

Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "Testing complete!" -ForegroundColor Cyan
Write-Host ""
Write-Host "To test manually, open: https://localhost:7001" -ForegroundColor Yellow
