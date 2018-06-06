#!/bin/sh
set -e
export
nuget push '**/*.nupkg' "$NUGET_GL_L4N" -Source https://api.nuget.org/v3/index.json
