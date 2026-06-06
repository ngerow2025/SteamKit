#!/usr/bin/env bash

# Exit immediately if a command exits with a non-zero status
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROTO_GEN_SRC="$SCRIPT_DIR/ProtobufGen"
PROTO_GEN_DLL="$PROTO_GEN_SRC/bin/Debug/ProtobufGen.dll"
PROTO_BASE="$(cd "$SCRIPT_DIR/../Protobufs" && pwd)"
SK2_BASE="$(cd "$SCRIPT_DIR/../../SteamKit2/SteamKit2/Base/Generated" && pwd)"

# Build the generator project
echo "Building ProtobufGen..."
dotnet build --configuration Debug "$PROTO_GEN_SRC"

# Parse target directories if specified
PROTO_DIRS=()
if [ "$1" = "-ProtoDir" ] || [ "$1" = "--proto-dir" ]; then
    IFS=',' read -r -a PROTO_DIRS <<< "$2"
elif [ -n "$1" ]; then
    IFS=',' read -r -a PROTO_DIRS <<< "$1"
fi

should_process() {
    local dir="$1"
    if [ ${#PROTO_DIRS[@]} -eq 0 ]; then
        return 0
    fi
    for d in "${PROTO_DIRS[@]}"; do
        if [ "$d" = "$dir" ]; then
            return 0
        fi
    done
    return 1
}

# Run one-off generation for gc.proto
echo "Running one-off generation for gc.proto..."
cd "$SCRIPT_DIR"
dotnet "$PROTO_GEN_DLL" --proto "gc.proto" --output "$SK2_BASE/GC/MsgBaseGC.cs" --namespace "SteamKit2.GC.Internal" > /dev/null

# Commented out protobuf dumper descriptor generation to match PowerShell script
# cd "$PROTO_BASE/google/protobuf"
# dotnet "$PROTO_GEN_DLL" --proto "descriptor.proto" --output "$SCRIPT_DIR/../ProtobufDumper/ProtobufDumper/Descriptor.cs" --namespace "google.protobuf" > /dev/null

# Read and process protos.csv
echo "Generating C# classes from protobufs..."
# Skip header line (line 1)
tail -n +2 "$SCRIPT_DIR/protos.csv" | while IFS=, read -r ProtoDir ProtoFileName ClassFilePath Namespace; do
    # Strip any Windows carriage return characters
    ProtoDir=$(echo "$ProtoDir" | tr -d '\r')
    ProtoFileName=$(echo "$ProtoFileName" | tr -d '\r')
    ClassFilePath=$(echo "$ClassFilePath" | tr -d '\r')
    Namespace=$(echo "$Namespace" | tr -d '\r')

    if [ -n "$ProtoDir" ] && should_process "$ProtoDir"; then
        # Convert backslashes to forward slashes for Linux paths
        ClassFilePathLinux=$(echo "$ClassFilePath" | tr '\\' '/')
        
        # Change directory to the specific protobuf directory
        cd "$PROTO_BASE/$ProtoDir"
        
        # Run ProtobufGen
        dotnet "$PROTO_GEN_DLL" --proto "$ProtoFileName" --output "$SK2_BASE/$ClassFilePathLinux" --namespace "$Namespace" > /dev/null
    fi
done

echo "Done!"
