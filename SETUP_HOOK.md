# Setup Hook Documentation

## Overview

This repository includes automated setup hooks that ensure all required development tools are installed before you start working on the GeometricAlgebraFulcrumLib project.

## What Gets Installed

The setup hook automatically checks for and installs (if missing):

1. **.NET SDK 8.0** - Required for building and running the C# codebase
2. **csharp-ls** - C# Language Server for enhanced IDE support
3. **Node.js** (LTS) - Required for MCP server bridge
4. **mcp-lsp-bridge** - MCP server that bridges Language Server Protocol

## Platform Support

### Ubuntu/Linux
The setup hook uses `setup-hook.sh` which:
- Downloads and installs .NET SDK 8.0 using Microsoft's official install script
- Adds .NET to PATH in `~/.bashrc` if needed
- Installs csharp-ls as a global dotnet tool
- Installs Node.js LTS from NodeSource repository
- Installs mcp-lsp-bridge as a global npm package

### Windows
The setup hook uses `setup-hook.ps1` which:
- Downloads and runs the official .NET SDK 8.0 installer
- Installs csharp-ls as a global dotnet tool
- Checks for Node.js (manual installation required if missing)
- Installs mcp-lsp-bridge as a global npm package

## How It Works

The hook is configured in `.claude/settings.local.json` to run automatically when a Claude Code session starts:

```json
{
  "hooks": {
    "SessionStart": [
      {
        "hooks": [
          {
            "type": "command",
            "command": "bash -c 'if [ -f ./setup-hook.sh ]; then bash ./setup-hook.sh; elif [ -f ./setup-hook.ps1 ]; then pwsh ./setup-hook.ps1; fi'",
            "timeout": 300
          }
        ]
      }
    ]
  }
}
```

## MCP Server Configuration

The setup hook also ensures that `mcp-lsp-bridge` is configured in `.mcp.json`:

```json
{
  "mcpServers": {
    "mcp-lsp-bridge": {
      "command": "npx",
      "args": [
        "-y",
        "mcp-lsp-bridge",
        "--",
        "csharp-ls"
      ]
    }
  }
}
```

And enabled in `.claude/settings.local.json`:

```json
{
  "enabledMcpjsonServers": [
    "context7",
    "serena",
    "sequential-thinking",
    "mcp-lsp-bridge"
  ]
}
```

## Manual Execution

You can also run the setup hook manually:

### Linux/Ubuntu
```bash
./setup-hook.sh
```

### Windows
```powershell
.\setup-hook.ps1
```

## Troubleshooting

### Linux: Permission Denied
If you get a permission denied error, make the script executable:
```bash
chmod +x setup-hook.sh
```

### Windows: Execution Policy
If PowerShell blocks script execution, run:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Node.js Not Found (Windows)
If Node.js is not installed on Windows, download and install it manually from:
https://nodejs.org/

Then run the setup hook again.

### .NET SDK Already Installed
The hook checks for existing installations and skips installation if .NET SDK 8.0 is already present.

### Hook Timeout
The hook has a 300-second (5-minute) timeout. If installation takes longer:
1. Check your internet connection
2. Run the setup script manually
3. Check the output for specific errors

## What's Next

After the setup hook runs successfully:
1. All development tools are ready to use
2. The MCP LSP bridge provides enhanced C# language support
3. You can start building and testing the project immediately

For build instructions, see [CLAUDE.md](CLAUDE.md#essential-commands).
