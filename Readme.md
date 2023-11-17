

# Build Steps

1. Copy `Setapp.xcframework` (https://github.com/MacPaw/Setapp-framework) into `setappLib`
2. Build the dylib by running

   `xcodebuild -project SetappLib/SetappLib.xcodeproj -configuration Release`
3. Copy the dylib to the rider project

    ```cp SetappLib/build/Release/libSetappLib.dylib SetappSharp/Resources```
