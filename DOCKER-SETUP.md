# E-Rezept Validator - Docker Setup Guide

This guide explains how to run the E-Rezept Validator in Docker Desktop.

---

## Prerequisites

1. **Docker Desktop** installed and running
2. **ABDATA SQL Server** accessible from your machine
3. **Git** (to clone/update the repository)

---

## Quick Start

### 1. Update ABDATA Connection String

Edit `docker-compose.yml` and update the ABDATA connection string:

```yaml
ConnectionStrings__AbdataDatabase: "Data Source=YOUR_SQL_SERVER;Initial Catalog=ABDATA1225A;User ID=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true"
```

**Note:** Use `host.docker.internal` if ABDATA is on your local machine:
```yaml
ConnectionStrings__AbdataDatabase: "Data Source=host.docker.internal;Initial Catalog=ABDATA1225A;..."
```

### 2. Build and Start Containers

Open terminal in the project root directory:

```bash
# Build and start all services
docker-compose up --build

# Or run in detached mode (background)
docker-compose up -d --build
```

### 3. Verify Services are Running

**Check container status:**
```bash
docker-compose ps
```

Expected output:
```
NAME                      STATUS    PORTS
erezept-postgres          Up        0.0.0.0:5432->5432/tcp
erezept-validator-api     Up        0.0.0.0:8080->8080/tcp
```

**Check API health:**
```bash
curl http://localhost:8080/health
```

Expected response:
```json
{
  "status": "healthy",
  "service": "E-Rezept Validator",
  "version": "1.0.0",
  "timestamp": "2026-02-01T14:30:00Z"
}
```

### 4. Access the API

- **Swagger UI:** http://localhost:8080
- **Health Check:** http://localhost:8080/health
- **Validation Endpoint:** POST http://localhost:8080/api/validation/e-rezept

---

## Testing the API

### Using Swagger UI (Browser)

1. Open http://localhost:8080 in your browser
2. Navigate to the validation endpoint
3. Click "Try it out"
4. Upload a FHIR XML bundle
5. Click "Execute"

### Using curl (Command Line)

```bash
# Test with XML bundle
curl -X POST http://localhost:8080/api/validation/e-rezept \
  -H "Content-Type: application/fhir+xml" \
  --data-binary @docs/eRezept-Beispiele/PZN-Verordnung_Nr_1/PZN_Nr1_eAbgabedaten.xml

# Test with JSON bundle
curl -X POST http://localhost:8080/api/validation/e-rezept \
  -H "Content-Type: application/fhir+json" \
  -d @your-bundle.json
```

### Using PowerShell

```powershell
# Update test script to use port 8080 instead of 7001
$baseUrl = "http://localhost:8080"
$apiEndpoint = "$baseUrl/api/validation/e-rezept"

$xml = Get-Content "docs/eRezept-Beispiele/PZN-Verordnung_Nr_1/PZN_Nr1_eAbgabedaten.xml" -Raw
$response = Invoke-WebRequest `
    -Uri $apiEndpoint `
    -Method POST `
    -Body $xml `
    -ContentType "application/fhir+xml"

$response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
```

---

## Container Management

### View Logs

```bash
# View all logs
docker-compose logs

# Follow logs in real-time
docker-compose logs -f

# View API logs only
docker-compose logs -f api

# View PostgreSQL logs only
docker-compose logs -f postgres
```

### Stop Services

```bash
# Stop containers (keeps data)
docker-compose stop

# Stop and remove containers (keeps data volumes)
docker-compose down

# Stop and remove everything including data
docker-compose down -v
```

### Restart Services

```bash
# Restart all services
docker-compose restart

# Restart API only
docker-compose restart api
```

### Rebuild After Code Changes

```bash
# Rebuild and restart
docker-compose up --build -d

# Force rebuild from scratch
docker-compose build --no-cache
docker-compose up -d
```

---

## Troubleshooting

### API Won't Start

**1. Check logs:**
```bash
docker-compose logs api
```

**2. Common issues:**
- **PostgreSQL not ready:** Wait 10-20 seconds for DB initialization
- **ABDATA connection failed:** Verify connection string and SQL Server accessibility
- **Port 8080 already in use:** Change port in docker-compose.yml

### Database Connection Issues

**Check PostgreSQL:**
```bash
# Connect to PostgreSQL container
docker exec -it erezept-postgres psql -U erezept_user -d erezept_validator

# List tables
\dt ta1_reference.*

# Check SOK codes
SELECT COUNT(*) FROM ta1_reference.special_codes;
```

**Check ABDATA connection:**
```bash
# View API logs for ABDATA errors
docker-compose logs api | grep -i abdata
```

### Reset Database

```bash
# Remove PostgreSQL volume and recreate
docker-compose down -v
docker-compose up -d
```

---

## Configuration Options

### Environment Variables

You can override settings via environment variables in `docker-compose.yml`:

```yaml
environment:
  # Logging
  Logging__LogLevel__Default: Information
  Logging__LogLevel__Microsoft: Warning

  # Cache
  AbdataSettings__CacheExpirationHours: 24

  # Connection timeouts
  ConnectionStrings__Ta1ReferenceDatabase: "...;Timeout=60"
```

### Custom Port

To change the API port (e.g., to 9000):

```yaml
services:
  api:
    ports:
      - "9000:8080"  # Host:Container
```

Then access at http://localhost:9000

---

## Production Deployment

### Security Considerations

**Before production:**

1. **Remove development endpoints:**
   - Comment out or remove `PznTestController`
   - Disable Swagger in production

2. **Use secrets management:**
   ```yaml
   secrets:
     abdata_connection:
       file: ./secrets/abdata_connection.txt

   services:
     api:
       secrets:
         - abdata_connection
   ```

3. **Enable HTTPS:**
   ```yaml
   environment:
     ASPNETCORE_URLS: https://+:443;http://+:80
   volumes:
     - ./certificates:/https:ro
   ```

4. **Set production environment:**
   ```yaml
   environment:
     ASPNETCORE_ENVIRONMENT: Production
   ```

### Health Monitoring

```bash
# Check health status
docker-compose ps

# Automated health check
while true; do
  curl -f http://localhost:8080/health || echo "API unhealthy"
  sleep 60
done
```

---

## Docker Desktop Integration

### View in Docker Desktop

1. Open Docker Desktop
2. Click "Containers" in the left sidebar
3. Find `erezept-validator-api` and `erezept-postgres`
4. Click on containers to view:
   - Logs
   - Stats (CPU, Memory)
   - Files
   - Exec (terminal)

### Resource Limits

Right-click container → Settings:
- CPU: 2-4 cores recommended
- Memory: 2-4 GB recommended
- Swap: 1 GB

---

## Additional Resources

- **Validation Rules:** See `CALC-004-to-CALC-007-IMPLEMENTATION.md`
- **API Documentation:** http://localhost:8080 (Swagger UI)
- **Test Scripts:** `test-calc-validation.ps1`
- **Example Bundles:** `docs/eRezept-Beispiele/`

---

## Support

For issues or questions:
1. Check logs: `docker-compose logs -f`
2. Verify configuration: `docker-compose config`
3. Review health checks: `docker-compose ps`

**Common Commands Reference:**
```bash
docker-compose up -d          # Start in background
docker-compose down           # Stop and remove containers
docker-compose logs -f api    # Follow API logs
docker-compose restart api    # Restart API only
docker-compose build --no-cache  # Rebuild from scratch
```

---

**Ready to test!** 🚀

Run `docker-compose up --build` and access http://localhost:8080
