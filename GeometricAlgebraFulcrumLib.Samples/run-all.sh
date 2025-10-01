#!/bin/bash
# Script to build and run all sample projects

echo "Building all samples..."
dotnet build Samples.sln

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

echo -e "\n========================================\n"

for dir in */; do
    if [[ -f "${dir}"*.csproj ]]; then
        projectname=$(basename "$dir")
        echo "=== Running $projectname ==="
        echo ""
        dotnet run --project "$dir" --no-build
        echo -e "\n========================================\n"
    fi
done

echo "All samples completed!"
