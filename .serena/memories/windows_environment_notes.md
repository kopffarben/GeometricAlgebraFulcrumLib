# Development Environment Notes

This repository is routinely exercised inside Bash shells (Git Bash, WSL, or a
Linux terminal), but many contributors still work on Windows. The notes below
aim to keep both workflows aligned.

## Command Line
- **Preferred shell**: Bash (Git Bash, WSL, or any Unix-like shell). All
  automation/scripts are written with POSIX-style commands in mind.
- **Alternative**: PowerShell or Command Prompt also work; PowerShell accepts
  both Unix-like and Win32 commands if you already have it in muscle memory.

## Path Separators
- Bash + .NET tooling handle `/` everywhere. Use `/` in documentation,
  scripts, and code (`Path.Combine` in C#) for portability.
- Windows APIs also accept `\`, but avoiding it keeps instructions consistent.

## Common Commands
### Bash / Git Bash / WSL
```bash
# Directory navigation
cd <directory>
pwd

# List files
ls -lah

# View file contents
cat <file>

# Find files / search
find . -name "*.cs"
rg "pattern" src/
```

### PowerShell
```powershell
cd <directory>
pwd
dir    # or ls
type <file>    # or cat <file>
Get-ChildItem -Recurse -Filter "*.cs"
Select-String -Path "*.cs" -Pattern "pattern"
```

### Command Prompt (limited but available)
```cmd
cd <directory>
dir
type <file>
dir /s *.cs
```

## Line Endings
- Windows uses CRLF (`\r\n`) line endings
- Git is typically configured to handle line endings automatically
- Check `.gitattributes` for line ending configuration

## File System
- Case-insensitive (but case-preserving)
- Maximum path length: 260 characters (legacy) or unlimited (with long path support)
- Reserved filenames: CON, PRN, AUX, NUL, COM1-9, LPT1-9

## .NET Development on Windows
### Visual Studio
- Recommended IDE: Visual Studio 2022 (Version 17+)
- Full-featured with excellent C# support
- Built-in testing, debugging, and profiling tools

### Visual Studio Code
- Lightweight alternative
- Requires C# extension
- Project includes `.vscode` configuration

### JetBrains Rider
- Alternative full-featured IDE
- Cross-platform
- Excellent for .NET development

## Git on Windows
- Prefer Git Bash/WSL for scripting parity.
- Configure line endings if you frequently switch OSes:
  ```powershell
  git config --global core.autocrlf true
  ```

## Environment Variables
```powershell
# View environment variable
echo $env:PATH

# Set environment variable (session)
$env:VARIABLE_NAME = "value"

# Set environment variable (permanent)
[Environment]::SetEnvironmentVariable("VARIABLE_NAME", "value", "User")
```

## .NET CLI
All standard `dotnet` commands behave the same on Bash, PowerShell, or CMD. In
cross-platform instructions we default to:
```bash
DOTNET_ROOT="$(pwd)/.dotnet8"
$DOTNET_ROOT/dotnet restore …
```
When using PowerShell, set the same variables via `$env:DOTNET_ROOT`.

## Performance Considerations
- Windows Defender or antivirus may slow builds
- Consider excluding build directories from real-time scanning
- Use SSD for development workspace

## Troubleshooting
### Long paths
If you encounter "path too long" errors:
```powershell
# Enable long paths (requires admin)
New-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" -Name "LongPathsEnabled" -Value 1 -PropertyType DWORD -Force
```

### Permission issues
Run PowerShell or Command Prompt as Administrator when needed.

### Execution policy
If PowerShell scripts don't run:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```
