#!/bin/bash
# Script to download the banner image for the Zwift TTT Race Simulator
# This should be run once to download and commit the banner image

set -e

BANNER_URL="https://github.com/user-attachments/assets/f849a4fb-2561-43bc-bc7b-d8ef61f56f54"
OUTPUT_FILE="docs/banner.png"

echo "Downloading banner image..."
curl -L "$BANNER_URL" -o "$OUTPUT_FILE"

if [ -f "$OUTPUT_FILE" ] && [ -s "$OUTPUT_FILE" ]; then
    echo "✓ Banner image downloaded successfully to $OUTPUT_FILE"
    echo "  File size: $(du -h "$OUTPUT_FILE" | cut -f1)"
    echo ""
    echo "Next steps:"
    echo "  1. Verify the image: open $OUTPUT_FILE"
    echo "  2. Commit the image: git add $OUTPUT_FILE && git commit -m 'Add banner image'"
else
    echo "✗ Failed to download banner image"
    echo "  Please download manually from: $BANNER_URL"
    echo "  Save it to: $OUTPUT_FILE"
    exit 1
fi
