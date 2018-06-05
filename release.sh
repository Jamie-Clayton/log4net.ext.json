#!/bin/sh
set -e

echo building log4net.Ext.Json
msbuild /r /t:pack /p:Configuration=Release log4net.Ext.Json
msbuild /r /t:pack /p:Configuration=Release log4net.Ext.Json.Net
msbuild /r /t:build /p:Configuration=Release log4net.Ext.Json.Xunit
