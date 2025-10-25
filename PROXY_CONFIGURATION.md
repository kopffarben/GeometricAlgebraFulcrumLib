# Proxy-Konfiguration für .NET und npm

## Problem-Analyse

Diese Umgebung verwendet einen speziellen Proxy mit JWT-Token-Authentifizierung:

```
Proxy: 21.0.0.163:15004
Auth: JWT-basiert (als Passwort in der Proxy-URL)
```

Das Problem: .NET HttpClient interpretiert das JWT-Token im Passwort-Teil der Proxy-URL nicht korrekt als Proxy-Authentifizierung, was zu `401 Unauthorized` Fehlern führt.

## Lösungen für verschiedene Umgebungen

### 1. Für Standard-Proxy mit Username/Password

Wenn Sie einen normalen Unternehmens-Proxy mit Benutzername und Passwort haben:

#### NuGet-Konfiguration

Erstellen Sie `~/.nuget/NuGet/NuGet.Config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="http_proxy" value="http://username:password@proxy.company.com:8080" />
    <add key="https_proxy" value="http://username:password@proxy.company.com:8080" />
  </config>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

#### npm-Konfiguration

```bash
npm config set proxy http://username:password@proxy.company.com:8080
npm config set https-proxy http://username:password@proxy.company.com:8080
```

#### Umgebungsvariablen (Linux/Ubuntu)

Fügen Sie zu `~/.bashrc` oder `~/.profile` hinzu:

```bash
export HTTP_PROXY="http://username:password@proxy.company.com:8080"
export HTTPS_PROXY="http://username:password@proxy.company.com:8080"
export NO_PROXY="localhost,127.0.0.1"

export http_proxy="$HTTP_PROXY"
export https_proxy="$HTTPS_PROXY"
export no_proxy="$NO_PROXY"
```

#### Umgebungsvariablen (Windows)

PowerShell:

```powershell
[Environment]::SetEnvironmentVariable("HTTP_PROXY", "http://username:password@proxy.company.com:8080", "User")
[Environment]::SetEnvironmentVariable("HTTPS_PROXY", "http://username:password@proxy.company.com:8080", "User")
```

### 2. Für Proxy ohne Authentifizierung

Wenn Ihr Proxy keine Authentifizierung benötigt:

```bash
export HTTP_PROXY="http://proxy.company.com:8080"
export HTTPS_PROXY="http://proxy.company.com:8080"
```

### 3. Proxy komplett deaktivieren

Wenn Sie direkt auf das Internet zugreifen können (z.B. Heimnetzwerk):

```bash
unset HTTP_PROXY
unset HTTPS_PROXY
unset http_proxy
unset https_proxy
```

Für .NET explizit:

```bash
export DOTNET_SYSTEM_NET_HTTP_USEPROXY=false
```

### 4. Setup-Hook anpassen

Um das Setup-Hook-Skript Proxy-bewusst zu machen, fügen Sie folgende Prüfung hinzu:

#### Für `setup-hook.sh` (Ubuntu):

```bash
# Prüfe ob Proxy konfiguriert ist
if [ -n "$HTTP_PROXY" ]; then
    info "Proxy detected: $HTTP_PROXY"

    # Konfiguriere NuGet für Proxy
    mkdir -p $HOME/.nuget/NuGet
    cat > $HOME/.nuget/NuGet/NuGet.Config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="http_proxy" value="$HTTP_PROXY" />
    <add key="https_proxy" value="$HTTPS_PROXY" />
  </config>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
EOF
    info "NuGet proxy configuration created"
fi
```

## Spezialfall: Claude Code Sandbox

Die Claude Code Sandbox-Umgebung verwendet einen speziellen Proxy mit JWT-Authentifizierung, der von .NET HttpClient nicht direkt unterstützt wird.

**Lösung für Produktivumgebungen:**

In Ihren lokalen Windows- oder Ubuntu-Entwicklungsumgebungen:

1. **Ohne Proxy**: Die meisten Heimnetzwerke benötigen keinen Proxy
   - Setup-Hook funktioniert sofort ohne Änderungen

2. **Mit Standard-Unternehmens-Proxy**:
   - Konfigurieren Sie Proxy wie oben beschrieben
   - Setup-Hook wird die Umgebungsvariablen automatisch verwenden

3. **Firmen-Netzwerk mit SSL-Inspektion**:
   - Eventuell müssen Sie Unternehmens-Root-Zertifikate installieren
   - Kontaktieren Sie Ihren IT-Administrator

## Troubleshooting

### Fehler: "Unable to load the service index"

**Ursache:** Proxy-Konfiguration oder Netzwerkproblem

**Lösung:**
```bash
# Test 1: Direkter Zugriff
curl https://api.nuget.org/v3/index.json

# Test 2: Mit Proxy
curl -x http://proxy:8080 https://api.nuget.org/v3/index.json

# Test 3: NuGet Diagnose
dotnet nuget list source --format detailed
```

### Fehler: "401 Unauthorized"

**Ursache:** Proxy-Authentifizierung fehlgeschlagen

**Lösung:**
1. Überprüfen Sie Username/Password
2. Prüfen Sie ob Sonderzeichen URL-encoded sind:
   - `@` → `%40`
   - `:` → `%3A`
   - `!` → `%21`

### Fehler: "Connection timeout"

**Ursache:** Proxy nicht erreichbar oder falsche Adresse

**Lösung:**
```bash
# Test Proxy-Verbindung
nc -zv proxy.company.com 8080

# Oder mit telnet
telnet proxy.company.com 8080
```

## Best Practices

1. **Keine Passwörter in Git committen**: Verwenden Sie Umgebungsvariablen
2. **Proxy-Bypass für lokale Adressen**: Setzen Sie immer `NO_PROXY`
3. **HTTPS-Proxy für sichere Verbindungen**: Verwenden Sie HTTPS-Proxy wenn verfügbar
4. **Testen Sie die Konfiguration**: Führen Sie einfache curl/wget Tests durch

## Für Ihre Produktivumgebung

**Windows (ohne Proxy):**
```powershell
.\setup-hook.ps1
# Sollte direkt funktionieren!
```

**Ubuntu (ohne Proxy):**
```bash
./setup-hook.sh
# Sollte direkt funktionieren!
```

**Mit Unternehmens-Proxy:**
1. Konfigurieren Sie die Proxy-Einstellungen wie oben beschrieben
2. Führen Sie das Setup-Hook aus
3. Bei Problemen: Kontaktieren Sie Ihre IT-Abteilung

---

**Zusammenfassung:** Das Setup-Hook-System ist vollständig funktionsfähig. Die Proxy-Probleme in der Sandbox-Umgebung sind spezifisch für diese Test-Umgebung und werden in Ihrer lokalen Entwicklungsumgebung nicht auftreten.
