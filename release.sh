#!/bin/sh
set -e

# this one is needed for adding xunit targets, msbuild /r doesn't do.
msbuild /r /t:pack /p:Configuration=Release log4net.Ext.Json
msbuild /r /t:pack /p:Configuration=Release log4net.Ext.Json.Net
