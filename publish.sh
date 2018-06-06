#!/bin/sh
set -e

msbuild /r /t:Test /p:Configuration=Release log4net.Ext.Json.Xunit
