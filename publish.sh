#!/bin/sh
set -e

nuget push '**/*.nupkg' $NUGET_GL_L4N -Source https://api.nuget.org/v3/index.json
