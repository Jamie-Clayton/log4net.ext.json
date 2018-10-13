#!/bin/sh
set -e

nuget push '**/*.nupkg' "$1" -source https://api.nuget.org/v3/index.json
