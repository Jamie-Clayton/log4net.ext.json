#!/bin/sh
set -e

msbuild /r /t:build,pack /p:Configuration=Release "/p:VersionSuffix=$1"
