# Erforderliche Domains für GeometricAlgebraFulcrumLib Entwicklungsumgebung

Diese Liste enthält alle Domains, die für die vollständige Funktionalität der Entwicklungsumgebung benötigt werden.

## .NET SDK & Runtime

### Microsoft Download & Installation
- **dot.net** - .NET Download-Portal
- **download.visualstudio.microsoft.com** - Visual Studio Downloads
- **dotnet.microsoft.com** - .NET Dokumentation und Downloads
- **builds.dotnet.microsoft.com** - .NET Build-Artefakte
- **aka.ms** - Microsoft URL-Shortener (für Downloads)

## NuGet Package Manager

### NuGet Core Services
- **api.nuget.org** - NuGet API v3 Hauptendpunkt
- **www.nuget.org** - NuGet Gallery Website
- **nuget.org** - Basis-Domain

### NuGet CDN & Storage
- **globalcdn.nuget.org** - Globales CDN
- **azuresearch-usnc.nuget.org** - NuGet Search Service (US North Central)
- **azuresearch-ussc.nuget.org** - NuGet Search Service (US South Central)

### Azure Storage (für NuGet Pakete)
- **api.nuget.org/v3-flatcontainer/** - Paket-Storage
- **api.nuget.org/v3/registration5-semver1/** - Paket-Metadaten
- **api.nuget.org/v3/registration5-semver2/** - Paket-Metadaten (SemVer 2.0)
- **api.nuget.org/v3/registration5-gz-semver1/** - GZIP-komprimierte Metadaten
- **api.nuget.org/v3/registration5-gz-semver2/** - GZIP-komprimierte Metadaten (SemVer 2.0)

### NuGet Security & Vulnerabilities
- **api.nuget.org/v3/vulnerabilities/** - Vulnerability-Datenbank
- **api.nuget.org/v3-index/repository-signatures/** - Repository-Signaturen

## Node.js & npm

### Node.js Download
- **nodejs.org** - Node.js Website und Downloads
- **deb.nodesource.com** - NodeSource Debian/Ubuntu Repository

### npm Registry
- **registry.npmjs.org** - npm Package Registry
- **registry.npmjs.com** - npm Package Registry (alternative)
- **npmjs.org** - npm Website
- **npmjs.com** - npm Website (alternative)

### npm CDN
- **cdn.jsdelivr.net** - jsDelivr CDN (für npm Pakete)
- **unpkg.com** - unpkg CDN (für npm Pakete)

## Spezifische NuGet Pakete (aus dem Projekt)

### AngouriMath & Dependencies
- Alle oben genannten NuGet-Domains

### Dew.Lab.Studio
- **nuget.org** (proprietäre Pakete)

### EPPlus
- **nuget.org**

### BenchmarkDotNet
- **nuget.org**

### NUnit & Testing
- **nuget.org**

### Microsoft Packages
- **nuget.org** (Microsoft.* Pakete)

## MCP Server Dependencies

### Context7 (npm)
- **registry.npmjs.org** - @upstash/context7-mcp
- **npmjs.org**

### Sequential Thinking (npm)
- **registry.npmjs.org** - @modelcontextprotocol/server-sequential-thinking
- **npmjs.org**

### mcp-lsp-bridge (npm)
- **registry.npmjs.org** - mcp-lsp-bridge
- **npmjs.org**

### Serena (Python/uvx)
- **github.com** - git+https://github.com/oraios/serena
- **raw.githubusercontent.com** - Raw GitHub Content
- **pypi.org** - Python Package Index
- **files.pythonhosted.org** - Python Package Files

## Git & Version Control

### GitHub
- **github.com** - GitHub Hauptseite
- **api.github.com** - GitHub API
- **raw.githubusercontent.com** - Raw File Access
- **codeload.github.com** - Repository Download

## Documentation & Resources

### Microsoft Docs
- **learn.microsoft.com** - Microsoft Learn Dokumentation
- **docs.microsoft.com** - Microsoft Dokumentation (alt)

### .NET Documentation
- **dotnet.microsoft.com** - .NET Dokumentation

## SSL/TLS Certificates

### Certificate Authorities
- **ocsp.digicert.com** - DigiCert OCSP
- **crl.microsoft.com** - Microsoft Certificate Revocation List
- **www.microsoft.com/pki/** - Microsoft PKI

## Zusammenfassung: Kritische Domains

### Absolut notwendig für NuGet:
```
api.nuget.org
www.nuget.org
nuget.org
globalcdn.nuget.org
azuresearch-usnc.nuget.org
azuresearch-ussc.nuget.org
```

### Absolut notwendig für .NET:
```
dot.net
dotnet.microsoft.com
download.visualstudio.microsoft.com
builds.dotnet.microsoft.com
aka.ms
```

### Absolut notwendig für npm:
```
registry.npmjs.org
npmjs.org
npmjs.com
```

### Absolut notwendig für Git:
```
github.com
api.github.com
raw.githubusercontent.com
codeload.github.com
```

### Absolut notwendig für Python (Serena):
```
github.com
raw.githubusercontent.com
pypi.org
files.pythonhosted.org
```

## Konfiguration für Proxy-Bypass

Wenn Sie diese Domains direkt (ohne Proxy) erreichen möchten, fügen Sie sie zur `NO_PROXY` Variable hinzu:

```bash
export NO_PROXY="localhost,127.0.0.1,\
api.nuget.org,\
nuget.org,\
*.nuget.org,\
registry.npmjs.org,\
*.npmjs.org,\
github.com,\
*.github.com,\
*.githubusercontent.com,\
dotnet.microsoft.com,\
*.microsoft.com"
```

## Domain-Whitelist für Firewall/Proxy

Falls Sie eine Firewall oder einen Proxy-Server konfigurieren müssen, hier die komplette Liste:

### NuGet (443/HTTPS)
- *.nuget.org
- nuget.org

### Microsoft (443/HTTPS)
- *.microsoft.com
- *.visualstudio.com
- aka.ms
- dot.net
- dotnet.microsoft.com

### npm (443/HTTPS)
- registry.npmjs.org
- *.npmjs.org
- *.npmjs.com

### GitHub (443/HTTPS)
- github.com
- *.github.com
- *.githubusercontent.com

### Python PyPI (443/HTTPS)
- pypi.org
- *.pypi.org
- files.pythonhosted.org

### CDN (443/HTTPS)
- cdn.jsdelivr.net
- unpkg.com

## Port-Anforderungen

### Ausgehende Verbindungen:
- **Port 443 (HTTPS)**: Alle oben genannten Domains
- **Port 80 (HTTP)**: Nur für HTTP → HTTPS Redirects
- **Port 22 (SSH)**: Optional für Git über SSH

## DNS-Anforderungen

Stellen Sie sicher, dass DNS-Auflösung für folgende Domains funktioniert:
- Alle oben genannten Domains
- Wildcard-Subdomains (z.B. *.nuget.org)

## Bandbreitenanforderungen

### Typische Download-Größen:
- **.NET SDK 8.0**: ~200 MB
- **NuGet Packages (gesamt)**: ~500 MB - 2 GB (je nach Projekt)
- **npm Packages**: ~100 MB - 500 MB
- **Python Packages**: ~50 MB - 200 MB

### Geschätzte Gesamtbandbreite für initiale Einrichtung:
- **Minimum**: 1 GB
- **Empfohlen**: 2-3 GB (mit Caching)

## Troubleshooting

### Domain-Erreichbarkeit testen:

```bash
# NuGet API
curl -I https://api.nuget.org/v3/index.json

# npm Registry
curl -I https://registry.npmjs.org/

# .NET Downloads
curl -I https://dotnet.microsoft.com/

# GitHub
curl -I https://github.com/

# Alle kritischen Domains testen
for domain in api.nuget.org registry.npmjs.org github.com dotnet.microsoft.com; do
    echo "Testing $domain..."
    curl -s -o /dev/null -w "%{http_code}\n" https://$domain/
done
```

### DNS-Auflösung testen:

```bash
# Test DNS resolution
nslookup api.nuget.org
nslookup registry.npmjs.org
nslookup github.com
```

## Sicherheitshinweise

1. **HTTPS verwenden**: Alle Domains sollten über HTTPS erreichbar sein
2. **Zertifikat-Validierung**: Aktivieren Sie SSL/TLS-Zertifikat-Validierung
3. **Proxy-Authentifizierung**: Verwenden Sie sichere Authentifizierungsmethoden
4. **Keine Man-in-the-Middle**: Vermeiden Sie SSL-Inspektion für Entwickler-Tools

## Für Claude Code Sandbox

**Aktuelle NO_PROXY Konfiguration:**
```
localhost,127.0.0.1,169.254.169.254,metadata.google.internal,
*.svc.cluster.local,*.local,*.googleapis.com,*.google.com
```

**Empfohlene Erweiterung für .NET/NuGet:**
```
localhost,127.0.0.1,169.254.169.254,metadata.google.internal,
*.svc.cluster.local,*.local,*.googleapis.com,*.google.com,
api.nuget.org,*.nuget.org,nuget.org,
registry.npmjs.org,*.npmjs.org,npmjs.com,
github.com,*.github.com,*.githubusercontent.com,
dotnet.microsoft.com,*.microsoft.com
```

---

**Hinweis**: Diese Liste basiert auf den aktuellen Anforderungen des GeometricAlgebraFulcrumLib-Projekts. Zusätzliche Domains können erforderlich sein, wenn neue Abhängigkeiten hinzugefügt werden.
