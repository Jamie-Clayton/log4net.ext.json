#!/bin/sh
set -e
export
msbuild /r /t:build,pack /p:Configuration=Release "/p:VersionSuffix=$1"
