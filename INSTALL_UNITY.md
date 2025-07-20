# Unity Game Engine Installation & Project Setup

## 🎯 **Current Status**

✅ **Unity project files created and configured**
❌ **Unity Game Engine not yet installed**
✅ **VS Code integration ready**
✅ **Project structure prepared**

## Installation Status

✅ **Unity Hub installed** via official Debian repository (v3.13.0)
❌ **Unity Editor not installed**

## Next Steps

1. Launch Unity Hub using `./launch-unity-hub.sh`
2. Install Unity 6.1 LTS through Unity Hub
3. Open this project in Unity Editor

### **Method 2: Direct Download**

1. **Download Unity 6.1 LTS:**

   ```bash
   # Visit Unity download page or use direct link
   wget https://download.unity3d.com/download_unity/[unity-hash]/UnitySetup-[version]

   # Or install via snap (if available)
   sudo snap install unity --classic
   ```

### **Method 3: Package Manager (Ubuntu/Debian)**

1. **Add Unity repository:**

   ```bash
   # Download Unity key
   wget -qO - https://hub.unity3d.com/linux/keys/public | sudo apt-key add -

   # Add repository
   sudo sh -c 'echo "deb https://hub.unity3d.com/linux/repos/deb stable main" > /etc/apt/sources.list.d/unityhub.list'

   # Update and install
   sudo apt update
   sudo apt install unityhub
   ```

## 🚀 **Open Unity Project**

Once Unity is installed:

### **Using Unity Hub:**

1. Open Unity Hub
2. Click "Open" or "Add project from disk"
3. Navigate to: `/home/norvald/projects/land-of-mist-unity/unity`
4. Select the unity folder
5. Unity will automatically detect and open the project

### **Using Command Line:**

```bash
# Navigate to project
cd /home/norvald/projects/land-of-mist-unity/unity

# Open with Unity (once installed)
unity -projectPath "$(pwd)"

# Or if using Unity Hub
unityhub://projects/add?path="$(pwd)"
```

## 📋 **Post-Installation Steps**

### **1. Verify Project Setup**

- Unity should open the MainScene.unity automatically
- Check Console for any errors (should be minimal)
- Verify Package Manager shows required packages

### **2. Install System.IO.Abstractions**

**In Unity Editor:**

1. Window > Package Manager
2. Click '+' dropdown > "Add package from git URL"
3. Enter: `https://github.com/TestableIO/System.IO.Abstractions.git#v19.2.91`

**Alternative - NuGet for Unity:**

1. Asset Store > Search "NuGet for Unity"
2. Download and import
3. Use NuGet interface to add System.IO.Abstractions

### **3. Configure Project Settings**

The project is pre-configured with:

- ✅ Scripting Define Symbols: `LAND_OF_MIST_RPG`
- ✅ API Compatibility Level: .NET Standard 2.1
- ✅ Company Name: "Land of Mist Studios"
- ✅ Product Name: "Land of Mist RPG"

### **4. Test VS Code Integration**

```bash
# Open VS Code in project directory
cd /home/norvald/projects/land-of-mist-unity/unity
code .
```

Verify:

- C# extension is active
- Unity Tools extension works
- IntelliSense provides autocomplete

## 🎮 **Begin Development**

Once Unity is set up, you can start implementing:

### **Priority Implementation Order:**

1. **GitHub Issue #1**: Core ScriptableObject Data Structures

   ```bash
   # Start with character and equipment data structures
   # Located in: Assets/Scripts/Data/
   ```

2. **GitHub Issue #2**: BalanceManager with JSON Configuration

   ```bash
   # JSON configuration system
   # Located in: Assets/StreamingAssets/GameBalance/
   ```

3. **GitHub Issue #6**: Save/Load System with System.IO.Abstractions

   ```bash
   # File-based save system with testable abstractions
   # Located in: Assets/Scripts/Systems/
   ```

### **Development Workflow:**

1. **Create Scripts** in appropriate folders
2. **Follow SOLID principles** per copilot-instructions.md
3. **Use Unity ScriptableObjects** for data-driven design
4. **Write unit tests** with Unity Test Framework
5. **Track progress** via GitHub Issues

## 🛠️ **Troubleshooting**

### **Unity Installation Issues:**

- **Permission errors**: Use `sudo` for system-wide installation
- **Missing dependencies**: Install required libraries for your distro
- **Hub won't start**: Try AppImage version or direct editor download

### **Project Won't Open:**

- Ensure Unity 6.1 LTS is installed
- Check project path has no special characters
- Verify all ProjectSettings files are present

### **Package Installation Issues:**

- Check internet connection for git-based packages
- Try Unity Package Manager cache refresh
- Use manual DLL installation as fallback

## 📞 **Next Actions**

1. **Install Unity Game Engine** using preferred method above
2. **Open the prepared project** in Unity
3. **Install System.IO.Abstractions** package
4. **Begin Phase 1A development** with GitHub Issue #1

Your Unity project structure is ready - you just need to install the Unity Game Engine! 🎯
