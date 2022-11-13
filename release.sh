#!/bin/sh
set -e

dotnet build -c Release
dotnet pack -c Release --version-suffix "$1"