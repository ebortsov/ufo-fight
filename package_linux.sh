#!/usr/bin/env bash
set -Eeuo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BUILD_DIR="$PROJECT_ROOT/build"

APP_NAME="UFO-Fight"
ARCH="linux-x86_64"
PACKAGE_ROOT="${APP_NAME}-${ARCH}"
OUTPUT_NAME="${APP_NAME}-${ARCH}.tar.gz"
OUTPUT_PATH="$PROJECT_ROOT/$OUTPUT_NAME"

if [[ ! -d "$BUILD_DIR" ]]; then
  echo "Error: build folder not found: $BUILD_DIR" >&2
  exit 1
fi

EXECUTABLE="$(find "$BUILD_DIR" -maxdepth 1 -type f -name '*.x86_64' -printf '%f\n' | head -n 1)"

if [[ -z "${EXECUTABLE:-}" ]]; then
  echo "Error: no Linux Unity executable (*.x86_64) found in $BUILD_DIR" >&2
  exit 1
fi

TMP_DIR="$(mktemp -d)"
cleanup() {
  rm -rf "$TMP_DIR"
}
trap cleanup EXIT

STAGING_DIR="$TMP_DIR/$PACKAGE_ROOT"
mkdir -p "$STAGING_DIR"

# Copy the Unity build contents.
# We exclude Burst debug info because Unity marks it DoNotShip.
tar \
  --exclude='*_BurstDebugInformation_DoNotShip' \
  -C "$BUILD_DIR" \
  -cf - . | tar -C "$STAGING_DIR" -xf -

# Ensure the game executable remains runnable.
chmod +x "$STAGING_DIR/$EXECUTABLE"

cat > "$STAGING_DIR/README.txt" <<EOF
$APP_NAME - Linux x86_64

How to run:

  tar -xzf $OUTPUT_NAME
  cd $PACKAGE_ROOT
  ./$EXECUTABLE

Notes:

- Keep $EXECUTABLE next to its *_Data folder and .so files.
- Do not move the executable out of this folder.
- This build is for 64-bit Linux.
- If the executable bit was lost during transfer, run:

  chmod +x $EXECUTABLE

EOF

rm -f "$OUTPUT_PATH"

tar \
  -C "$TMP_DIR" \
  -czf "$OUTPUT_PATH" \
  "$PACKAGE_ROOT"

sha256sum "$OUTPUT_PATH" > "$OUTPUT_PATH.sha256"

echo "Created:"
echo "  $OUTPUT_PATH"
echo "  $OUTPUT_PATH.sha256"