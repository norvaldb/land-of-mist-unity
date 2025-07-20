# Unity Package Installation Guide

## Required Unity Packages

### Install via Unity Package Manager (Window > Package Manager)

1. **TextMeshPro** (Usually pre-installed)
   - Enhanced text rendering for UI
   - Medieval font support

2. **Unity Test Framework** (Usually pre-installed)
   - EditMode and PlayMode testing
   - Essential for our SOLID architecture

### External Dependencies

#### System.IO.Abstractions (NuGet Package)

This is the ONLY external dependency we need for testable file operations.

**Installation Options:**

**Option A: NuGet for Unity (Recommended)**

1. Install NuGet for Unity from Asset Store or GitHub
2. Add System.IO.Abstractions via NuGet interface

**Option B: Manual DLL Installation**

1. Download System.IO.Abstractions.dll
2. Place in Assets/Plugins/
3. Configure for .NET Standard 2.1

**Option C: Unity Package Manager (Git URL)**

```
https://github.com/TestableIO/System.IO.Abstractions.git#v19.2.91
```

## Architecture Benefits

### Why System.IO.Abstractions?

- **100% Testable File Operations**: Mock file system for unit tests
- **Cross-Platform Compatibility**: Unified file handling
- **SOLID Compliance**: Dependency inversion for file operations
- **Future-Proof**: Easy to swap file backends

### File Operation Example

```csharp
// Testable save system
public class SaveManager
{
    private readonly IFileSystem fileSystem;

    public SaveManager(IFileSystem fileSystem = null)
    {
        this.fileSystem = fileSystem ?? new FileSystem();
    }

    // Now fully testable with MockFileSystem
}
```

## Optional Enhancements

### Development Quality of Life

- **Unity Console Pro** - Enhanced console (optional)
- **Odin Inspector** - Advanced inspector (optional, premium)
- **DOTween** - Animation system (not needed for text-based RPG)

## Project Setup Steps

1. **Open Unity Hub**
2. **Add Project** from `/home/norvald/projects/land-of-mist-unity/unity`
3. **Open with Unity 6.1 LTS**
4. **Install System.IO.Abstractions** via preferred method
5. **Verify TextMeshPro and Test Framework** are available

## No Database Required

- File-based storage using Unity JsonUtility
- No SQL or NoSQL database setup needed
- Simple JSON configuration files in StreamingAssets

## Performance Tools (Built into Unity)

- **Unity Profiler** - Performance monitoring
- **Memory Profiler** - Memory usage analysis
- **Unity Test Framework** - Comprehensive testing

## Total Setup Time: ~30 minutes

- Unity project opening: 5 minutes
- Package installations: 15 minutes
- Initial file structure: 10 minutes
