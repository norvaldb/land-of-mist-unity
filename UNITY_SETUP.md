# Unity Project Setup Guide - Land of Mist RPG

## 🎯 **Project Status**

✅ **Unity project files created and configured**
✅ **Essential folder structure established**
✅ **VS Code integration ready**
✅ **Basic scene setup completed**

## 📋 **Next Steps to Complete Setup**

### **1. Open Unity Project**

**Method A: Using Unity Hub (Recommended)**

```bash
# Open Unity Hub
unity-hub

# Then:
# 1. Click "Add project from disk"
# 2. Navigate to: /home/norvald/projects/land-of-mist-unity/unity
# 3. Select the unity folder
# 4. Open with Unity 6.1 LTS
```

**Method B: Command Line**

```bash
# Navigate to project directory
cd /home/norvald/projects/land-of-mist-unity/unity

# Open with Unity (if Unity is in PATH)
unity -projectPath .
```

### **2. Install Required Packages**

Once Unity opens, install these packages via **Window > Package Manager**:

#### **Essential Packages:**

- **TextMeshPro** (usually pre-installed)
- **Unity Test Framework** (usually pre-installed)

#### **System.IO.Abstractions Installation:**

**Option A: NuGet for Unity (Recommended)**

1. Install "NuGet for Unity" from Asset Store or GitHub
2. Use NuGet interface to add System.IO.Abstractions

**Option B: Package Manager Git URL**

1. Window > Package Manager
2. Click '+' > Add package from git URL
3. Enter: `https://github.com/TestableIO/System.IO.Abstractions.git#v19.2.91`

**Option C: Manual DLL (Fallback)**

1. Download System.IO.Abstractions.dll
2. Place in `Assets/Plugins/`

### **3. Verify Project Structure**

Your project should now have:

```
unity/
├── Assets/
│   ├── Scenes/
│   │   └── MainScene.unity ✅
│   ├── Scripts/
│   │   ├── Core/ ✅
│   │   ├── Systems/ ✅
│   │   ├── Data/ ✅
│   │   ├── UI/ ✅
│   │   ├── Combat/ ✅
│   │   └── Utils/ ✅
│   ├── ScriptableObjects/ ✅
│   └── StreamingAssets/
│       ├── GameBalance/ ✅
│       └── Enemies/ ✅
├── Packages/
│   └── manifest.json ✅
└── ProjectSettings/ ✅
```

### **4. Test Unity Setup**

1. **Open MainScene.unity**
2. **Verify Console** - No critical errors
3. **Check Package Manager** - All packages loaded
4. **Test Play Mode** - Scene runs without errors

### **5. VS Code Integration Test**

1. **Open VS Code** in project directory:

   ```bash
   cd /home/norvald/projects/land-of-mist-unity/unity
   code .
   ```

2. **Verify Extensions:**
   - C# extension active
   - Unity Tools extension active
   - IntelliSense working

3. **Test Integration:**
   - Create a simple C# script in Unity
   - Open it in VS Code
   - Verify syntax highlighting and autocomplete

## 🚀 **Ready for Development**

Once setup is complete, you can begin implementing:

### **Phase 1A Priority Order:**

1. **GitHub Issue #1**: Core ScriptableObject Data Structures
2. **GitHub Issue #2**: BalanceManager with JSON Configuration
3. **GitHub Issue #4**: Basic Character System
4. **GitHub Issue #6**: Save/Load System with System.IO.Abstractions

### **Development Workflow:**

1. **Use GitHub Issues** for task tracking
2. **Follow SOLID principles** as outlined in copilot-instructions.md
3. **Write unit tests** using Unity Test Framework
4. **Use JSON configuration** for data-driven development

## 🛠️ **Troubleshooting**

### **Common Issues:**

**Unity won't open project:**

- Ensure Unity 6.1 LTS is installed
- Check that all ProjectSettings files are present
- Try opening Unity Hub first, then adding project

**Package installation fails:**

- Check internet connection
- Try different installation methods for System.IO.Abstractions
- Use Unity's Package Manager cache refresh

**VS Code integration issues:**

- Ensure C# extension is installed and active
- Restart VS Code after Unity project opens
- Check that Unity is generating .csproj files

**Build errors:**

- Verify all essential directories exist
- Check that no critical packages are missing
- Ensure scripting define symbols are correct

## 📞 **Next Steps**

After Unity setup is complete, proceed with:

1. **System.IO.Abstractions integration testing**
2. **First ScriptableObject implementation**
3. **JSON configuration system setup**
4. **Unit test framework verification**

The project is now ready for Phase 1A development! 🎮
