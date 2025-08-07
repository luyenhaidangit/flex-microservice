#!/usr/bin/env bash
set -euo pipefail

if [ "${CI:-}" = "true" ]; then
  dotnet format --verify-no-changes
else
  dotnet format
fi
