#!/bin/bash
# Setup hook for GeometricAlgebraFulcrumLib - Ubuntu/Linux
# This script ensures all required development tools are installed

set -e  # Exit on error

echo "=========================================="
echo "GA-FuL Setup Hook (Ubuntu/Linux)"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored messages
info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running on Linux
if [[ "$(uname -s)" != "Linux" ]]; then
    error "This script is for Linux/Ubuntu. Please use setup-hook.ps1 for Windows."
    exit 1
fi

info "Detected OS: $(uname -s) $(uname -r)"

# 1. Check and install .NET SDK 8.0
info "Checking for .NET SDK 8.0..."

if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    info "Found dotnet version: $DOTNET_VERSION"

    # Check if SDK 8.0 is installed
    if dotnet --list-sdks | grep -q "^8\.0"; then
        info ".NET SDK 8.0 is already installed"
    else
        warn ".NET SDK 8.0 not found, installing..."

        # Install .NET SDK 8.0 on Ubuntu
        wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
        chmod +x dotnet-install.sh
        ./dotnet-install.sh --channel 8.0
        rm dotnet-install.sh

        # Add to PATH if not already there
        if [[ ":$PATH:" != *":$HOME/.dotnet:"* ]]; then
            echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
            echo 'export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools' >> ~/.bashrc
            export DOTNET_ROOT=$HOME/.dotnet
            export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools
        fi

        info ".NET SDK 8.0 installed successfully"
    fi
else
    warn ".NET SDK not found, installing .NET SDK 8.0..."

    # Install .NET SDK 8.0 on Ubuntu
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
    chmod +x dotnet-install.sh
    ./dotnet-install.sh --channel 8.0
    rm dotnet-install.sh

    # Add to PATH
    echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
    echo 'export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools' >> ~/.bashrc
    export DOTNET_ROOT=$HOME/.dotnet
    export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools

    info ".NET SDK 8.0 installed successfully"
fi

# 2. Install csharp-ls (C# Language Server)
info "Checking for csharp-ls dotnet tool..."

if dotnet tool list -g | grep -q "csharp-ls"; then
    info "csharp-ls is already installed"
else
    info "Installing csharp-ls dotnet tool..."
    dotnet tool install --global csharp-ls
    info "csharp-ls installed successfully"
fi

# 3. Check for Node.js (required for mcp-lsp-bridge)
info "Checking for Node.js..."

if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    info "Found Node.js version: $NODE_VERSION"
else
    warn "Node.js not found. Installing Node.js LTS..."

    # Install Node.js using NodeSource
    curl -fsSL https://deb.nodesource.com/setup_lts.x | sudo -E bash -
    sudo apt-get install -y nodejs

    info "Node.js installed successfully"
fi

# 4. Install mcp-lsp-bridge
info "Checking for mcp-lsp-bridge..."

# Check if npm package is globally installed
if npm list -g mcp-lsp-bridge &> /dev/null; then
    info "mcp-lsp-bridge is already installed globally"
else
    info "Installing mcp-lsp-bridge..."
    npm install -g mcp-lsp-bridge
    info "mcp-lsp-bridge installed successfully"
fi

# 5. Verify installations
echo ""
echo "=========================================="
echo "Installation Verification"
echo "=========================================="

info "Installed versions:"
echo "  - .NET SDK: $(dotnet --version)"
echo "  - csharp-ls: $(dotnet tool list -g | grep csharp-ls | awk '{print $2}')"
echo "  - Node.js: $(node --version)"
echo "  - npm: $(npm --version)"

if npm list -g mcp-lsp-bridge &> /dev/null; then
    echo "  - mcp-lsp-bridge: $(npm list -g mcp-lsp-bridge --depth=0 | grep mcp-lsp-bridge | awk '{print $2}')"
fi

echo ""
info "Setup completed successfully!"
echo "=========================================="

exit 0
