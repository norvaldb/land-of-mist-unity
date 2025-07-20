#!/bin/bash
# Unity Hub Launcher Script
# This script launches Unity Hub from the system installation

echo "Launching Unity Hub..."

# Check if Unity Hub is installed
if ! command -v unityhub &> /dev/null; then
    echo "Error: Unity Hub not found in system PATH."
    echo "Please install Unity Hub first:"
    echo "sudo apt-get install unityhub"
    exit 1
fi

# Launch Unity Hub
unityhub > unity-hub.log 2>&1 &
UNITY_PID=$!

echo "Unity Hub launched with PID: $UNITY_PID"
echo "Log file: $(pwd)/unity-hub.log"
echo ""
echo "Unity Hub should open in a moment."
echo "To view the log: tail -f unity-hub.log"
echo "To stop Unity Hub: kill $UNITY_PID"
