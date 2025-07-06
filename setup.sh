#!/bin/bash

cd ./Submodules/CubivoxCore || exit

# Run the "dotnet build" command
dotnet build

# Check if the build was successful
if [ $? -eq 0 ]; then
  echo "Build successful!"

  # Move the generated CubivoxCore.dll to the target directory
  mv ./bin/Debug/netstandard2.1/CubivoxCore.dll ../../Assets/CoreBuilds
  echo "CubivoxCore build successful: Cubivox is now setup and ready for use"
else
  echo "CubivoxCore build failed."
  exit 1
fi