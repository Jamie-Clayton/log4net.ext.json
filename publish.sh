#!/bin/sh
set -e

dotnet nuget push '**/*.nupkg' --api-key "$1" --source https://api.nuget.org/v3/index.json
