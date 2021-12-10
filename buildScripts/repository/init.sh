#!/bin/bash

SCRIPT_PATH="$(dirname "$(realpath "$0")")"
ROOT_PATH="$SCRIPT_PATH/../.."
cd $ROOT_PATH

git submodule update --init --recursive

cd external/discord.net-labs
git checkout a65affc4b6a9eb4de32fd172ad93fba6110675c2

cd $ROOT_PATH/buildScripts

# Make scripts executeable
find ./ -type f -name "*.sh" -exec chmod +x "{}" \;
