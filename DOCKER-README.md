# Docker Quick Reference

## 🚀 Quick Start (Using Existing PostgreSQL)

Your existing PostgreSQL container (`erezept-postgres`) is already running.
Just start the API:

```bash
# Update ABDATA connection in docker-compose.yml first!
# Then run:
docker-compose up --build
```

Access at: **http://localhost:8080**

---

## 🐳 What Gets Created

**Using `docker-compose.yml` (default):**
- ✅ `erezept-validator-api` container (NEW)
- ✅ Uses existing `erezept-postgres` container (EXISTING)
- Port: 8080

**Using `docker-compose-standalone.yml` (alternative):**
- ✅ `erezept-validator-api-standalone` container (NEW)
- ✅ `erezept-postgres-standalone` container (NEW)
- Ports: 8081 (API), 5433 (PostgreSQL)

---

## 📋 Common Commands

```bash
# Start API (uses existing DB)
docker-compose up -d

# Stop API (keeps DB running)
docker-compose down

# View logs
docker-compose logs -f api

# Rebuild after code changes
docker-compose up --build -d

# Stop everything
docker stop erezept-validator-api erezept-postgres
```

---

## 🧪 Testing

**Swagger UI:**
http://localhost:8080

**Health Check:**
```bash
curl http://localhost:8080/health
```

**Validate Bundle:**
```bash
curl -X POST http://localhost:8080/api/validation/e-rezept \
  -H "Content-Type: application/fhir+xml" \
  --data-binary @docs/eRezept-Beispiele/PZN-Verordnung_Nr_1/PZN_Nr1_eAbgabedaten.xml
```

---

## ⚙️ Configuration

**ABDATA Connection:**
Edit `docker-compose.yml` line 14:
```yaml
ConnectionStrings__AbdataDatabase: "Data Source=YOUR_SERVER;Initial Catalog=ABDATA1225A;..."
```

**For local SQL Server:**
```yaml
ConnectionStrings__AbdataDatabase: "Data Source=host.docker.internal;Initial Catalog=ABDATA1225A;..."
```

---

## 🔧 Troubleshooting

**PostgreSQL not accessible:**
```bash
# Check if running
docker ps | grep erezept-postgres

# Restart if needed
docker restart erezept-postgres
```

**Port 8080 already in use:**
```yaml
# Change in docker-compose.yml
ports:
  - "9000:8080"  # Use port 9000 instead
```

**View API logs:**
```bash
docker logs -f erezept-validator-api
```

**Connect to PostgreSQL:**
```bash
docker exec -it erezept-postgres psql -U erezept_user -d erezept_validator
```

---

## 📚 Full Documentation

See **DOCKER-SETUP.md** for complete documentation including:
- Production deployment
- Security configuration
- Health monitoring
- Advanced troubleshooting
