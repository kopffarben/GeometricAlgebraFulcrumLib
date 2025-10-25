# Setup hook for GeometricAlgebraFulcrumLib - Windows
# This script ensures all required development tools are installed

$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "GA-FuL Setup Hook (Windows)" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Function to print colored messages
function Write-Info {
    param($Message)
    Write-Host "[INFO] $Message" -ForegroundColor Green
}

function Write-Warn {
    param($Message)
    Write-Host "[WARN] $Message" -ForegroundColor Yellow
}

function Write-Error-Custom {
    param($Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Check if running on Windows
if ($PSVersionTable.Platform -eq "Unix") {
    Write-Error-Custom "This script is for Windows. Please use setup-hook.sh for Linux/Ubuntu."
    exit 1
}

Write-Info "Detected OS: Windows"

# 1. Check and install .NET SDK 8.0
Write-Info "Checking for .NET SDK 8.0..."

$dotnetExists = Get-Command dotnet -ErrorAction SilentlyContinue

if ($dotnetExists) {
    $dotnetVersion = dotnet --version
    Write-Info "Found dotnet version: $dotnetVersion"

    # Check if SDK 8.0 is installed
    $sdk80 = dotnet --list-sdks | Select-String "^8\.0"

    if ($sdk80) {
        Write-Info ".NET SDK 8.0 is already installed"
    } else {
        Write-Warn ".NET SDK 8.0 not found, installing..."

        # Download and install .NET SDK 8.0
        $installerUrl = "https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-8.0-windows-x64-installer"
        $installerPath = "$env:TEMP\dotnet-sdk-8.0-installer.exe"

        Write-Info "Downloading .NET SDK 8.0 installer..."
        Invoke-WebRequest -Uri $installerUrl -OutFile $installerPath

        Write-Info "Running installer (this may require administrator privileges)..."
        Start-Process -FilePath $installerPath -Args "/quiet /norestart" -Wait

        Remove-Item $installerPath
        Write-Info ".NET SDK 8.0 installed successfully"
    }
} else {
    Write-Warn ".NET SDK not found, installing .NET SDK 8.0..."

    # Download and install .NET SDK 8.0
    $installerUrl = "https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-8.0-windows-x64-installer"
    $installerPath = "$env:TEMP\dotnet-sdk-8.0-installer.exe"

    Write-Info "Downloading .NET SDK 8.0 installer..."
    Invoke-WebRequest -Uri $installerUrl -OutFile $installerPath

    Write-Info "Running installer (this may require administrator privileges)..."
    Start-Process -FilePath $installerPath -Args "/quiet /norestart" -Wait

    Remove-Item $installerPath

    # Refresh environment variables
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")

    Write-Info ".NET SDK 8.0 installed successfully"
}

# 2. Install csharp-ls (C# Language Server)
Write-Info "Checking for csharp-ls dotnet tool..."

$csharpLsInstalled = dotnet tool list -g | Select-String "csharp-ls"

if ($csharpLsInstalled) {
    Write-Info "csharp-ls is already installed"
} else {
    Write-Info "Installing csharp-ls dotnet tool..."
    dotnet tool install --global csharp-ls
    Write-Info "csharp-ls installed successfully"
}

# 3. Check for Node.js (required for mcp-lsp-bridge)
Write-Info "Checking for Node.js..."

$nodeExists = Get-Command node -ErrorAction SilentlyContinue

if ($nodeExists) {
    $nodeVersion = node --version
    Write-Info "Found Node.js version: $nodeVersion"
} else {
    Write-Warn "Node.js not found. Please install Node.js LTS from https://nodejs.org/"
    Write-Warn "After installing Node.js, please run this script again."
    exit 1
}

# 4. Install mcp-lsp-bridge
Write-Info "Checking for mcp-lsp-bridge..."

# Check if npm package is globally installed
$npmList = npm list -g mcp-lsp-bridge 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Info "mcp-lsp-bridge is already installed globally"
} else {
    Write-Info "Installing mcp-lsp-bridge..."
    npm install -g mcp-lsp-bridge

    if ($LASTEXITCODE -eq 0) {
        Write-Info "mcp-lsp-bridge installed successfully"
    } else {
        Write-Error-Custom "Failed to install mcp-lsp-bridge"
        exit 1
    }
}

# 5. Verify installations
Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Installation Verification" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

Write-Info "Installed versions:"
$dotnetVersionFinal = dotnet --version
Write-Host "  - .NET SDK: $dotnetVersionFinal"

$csharpLsVersion = (dotnet tool list -g | Select-String "csharp-ls").ToString().Split()[1]
Write-Host "  - csharp-ls: $csharpLsVersion"

$nodeVersionFinal = node --version
Write-Host "  - Node.js: $nodeVersionFinal"

$npmVersionFinal = npm --version
Write-Host "  - npm: $npmVersionFinal"

$mcpLspBridgeVersion = npm list -g mcp-lsp-bridge --depth=0 2>&1 | Select-String "mcp-lsp-bridge@"
if ($mcpLspBridgeVersion) {
    Write-Host "  - mcp-lsp-bridge: $($mcpLspBridgeVersion.ToString().Split('@')[1])"
}

Write-Host ""
Write-Info "Setup completed successfully!"
Write-Host "==========================================" -ForegroundColor Cyan

exit 0
