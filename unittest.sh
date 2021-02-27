#!/bin/sh
set -e

msbuild /r /t:build,Test /p:Configuration=Release log4net.Ext.Json.Xunit
