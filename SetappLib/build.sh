xcodebuild -configuration Release
mkdir -p ../SetappSharp/runtimes/macos/native
cp ./build/Release/libSetappLib.dylib ../SetappSharp/runtimes/macos/native