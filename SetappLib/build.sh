xcodebuild -configuration Release
mkdir -p ../SetappSharp/runtimes/osx/native
cp ./build/Release/libSetappLib.dylib ../SetappSharp/runtimes/osx/native