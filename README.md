# Lyon Tesselation
(WIP) Tessellation of vector paths in Unity, powered by [lyon](https://github.com/nical/lyon).

## Features
- Supports both Fill and Stroke tessellation, with several configurations provided by lyon
- Easy to use [PathBuilder](Runtime/PathBuilder.cs) API for drawing paths that will be passed to the [Tessellator](Runtime/Tessellator.cs)
- Run multithreaded tessellation with the [Job System](https://docs.unity3d.com/Manual/JobSystemOverview.html) and [Burst](https://docs.unity3d.com/Packages/com.unity.burst@1.8/manual/index.html) by leveraging the `Create*Job` methods from `Tessellator`
- Both `PathBuilder` and `Tessellator` support reusing memory, just call `Clear()` instead of disposing of them and allocating new instances
- Prebuilt for the following platforms: Windows (x86_64), Linux (x86_64), macOS (x86_64, arm64), iOS (arm64), Android (arm32, arm64, x86, x86_64)
