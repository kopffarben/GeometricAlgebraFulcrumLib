# Windows Environment Notes

This project is developed on Windows. Here are important notes about working in this environment:

## Command Line
Use **PowerShell** or **Command Prompt** for terminal operations. PowerShell is recommended as it supports both Unix-like and Windows commands.

## Path Separators
- Windows uses backslash `\` as path separator
- PowerShell accepts both `\` and `/`
- When specifying paths in code, use forward slash `/` or `Path.Combine()` for cross-platform compatibility

## Common Commands
### PowerShell (supports both syntaxes)
```powershell
# Directory navigation
cd <directory>
pwd

# List files
dir
ls  # PowerShell also supports this

# View file contents
type <file>
cat <file>  # PowerShell also supports this

# Find files
Get-ChildItem -Recurse -Filter "*.cs"

# Search in files (like grep)
Select-String -Path "*.cs" -Pattern "pattern"
```

### Command Prompt
```cmd
# Directory navigation
cd <directory>

# List files
dir

# View file contents
type <file>

# Find files
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
- Use Git Bash, PowerShell, or Command Prompt
- Configure line endings:
  ```
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

## .NET CLI on Windows
All standard dotnet commands work the same:
```powershell
dotnet --version
dotnet build
dotnet test
dotnet run
```

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
