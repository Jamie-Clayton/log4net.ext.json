#!/bin/sh
set -e

msbuild /r /t:pack /p:Configuration=Release "/p:VersionSuffix=$1"
