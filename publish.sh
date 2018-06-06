#!/bin/sh
set -e

nuget push '**/*.nupkg' "$1" -source https://www.nuget.org/api/v2/package
#https://api.nuget.org/v3/index.json
