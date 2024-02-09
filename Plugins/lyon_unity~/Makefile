CARGO ?= cargo
CP ?= cp

SRC = $(wildcard src/*.rs) Cargo.toml Cargo.lock

CARGO_BUILD = $(CARGO) build --release --target $(1)
CARGO_BUILD_WITH_LINKER = $(CARGO) build --release --target $(1) --config target.$(1).linker=\"$(2)\"

# Windows
target/x86_64-pc-windows-gnu/release/lyon_unity.dll: $(SRC)
	$(call CARGO_BUILD,x86_64-pc-windows-gnu)

../Build/Windows/x86_64/lyon_unity.dll: target/x86_64-pc-windows-gnu/release/lyon_unity.dll
	$(CP) $< $@

windows-x86_64: ../Build/Windows/x86_64/lyon_unity.dll
all-windows: windows-x86_64

# Apple platforms
target/%/release/liblyon_unity.dylib: $(SRC)
	$(call CARGO_BUILD,$*)

../Build/macOS/x86_64/liblyon_unity.dylib: target/x86_64-apple-darwin/release/liblyon_unity.dylib
	$(CP) $< $@
../Build/macOS/arm64/liblyon_unity.dylib: target/aarch64-apple-darwin/release/liblyon_unity.dylib
	$(CP) $< $@
../Build/iOS/arm64/liblyon_unity.dylib: target/aarch64-apple-ios/release/liblyon_unity.dylib
	$(CP) $< $@

macos-x86_64: ../Build/macOS/x86_64/liblyon_unity.dylib
macos-arm64: ../Build/macOS/arm64/liblyon_unity.dylib
ios-arm64: ../Build/iOS/arm64/liblyon_unity.dylib
all-macos: macos-x86_64 macos-arm64
all-apple: all-macos ios-arm64

# Linux
target/x86_64-unknown-linux-gnu/release/liblyon_unity.so: $(SRC)
	$(call CARGO_BUILD,x86_64-unknown-linux-gnu)

../Build/Linux/x86_64/liblyon_unity.so: target/x86_64-unknown-linux-gnu/release/liblyon_unity.so
	$(CP) $< $@

linux-x86_64: ../Build/Linux/x86_64/liblyon_unity.so
all-linux: linux-x86_64

# Android
target/armv7-linux-androideabi/release/liblyon_unity.so: $(SRC)
	$(call CARGO_BUILD_WITH_LINKER,armv7-linux-androideabi,$(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/armv7a-linux-androideabi19-clang))
target/aarch64-linux-android/release/liblyon_unity.so: $(SRC)
	$(call CARGO_BUILD_WITH_LINKER,aarch64-linux-android,$(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/aarch64-linux-android21-clang))
target/i686-linux-android/release/liblyon_unity.so: $(SRC)
	$(call CARGO_BUILD_WITH_LINKER,i686-linux-android,$(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/i686-linux-android16-clang))
target/x86_64-linux-android/release/liblyon_unity.so: $(SRC)
	$(call CARGO_BUILD_WITH_LINKER,x86_64-linux-android,$(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/x86_64-linux-android21-clang))

../Build/Android/arm32/liblyon_unity.so: target/armv7-linux-androideabi/release/liblyon_unity.so
	$(CP) $< $@
../Build/Android/arm64/liblyon_unity.so: target/aarch64-linux-android/release/liblyon_unity.so
	$(CP) $< $@
../Build/Android/x86/liblyon_unity.so: target/i686-linux-android/release/liblyon_unity.so
	$(CP) $< $@
../Build/Android/x86_64/liblyon_unity.so: target/x86_64-linux-android/release/liblyon_unity.so
	$(CP) $< $@

android-arm32: ../Build/Android/arm32/liblyon_unity.so
android-arm64: ../Build/Android/arm64/liblyon_unity.so
android-x86: ../Build/Android/x86/liblyon_unity.so
android-x86_64: ../Build/Android/x86_64/liblyon_unity.so
all-android: android-arm32 android-arm64 android-x86 android-x86_64