# Land of Mist RPG - Unity Development Instructions

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Land of Mist RPG Project Overview

**Game Type:** Text-based turn-based RPG with party management
**Target Platform:** Unity 6.1 LTS, cross-platform (PC primary, mobile secondary)
**Repository:** https://github.com/norvaldb/land-of-mist-unity
**Development Phase:** Phase 1A - Core Systems Implementation

### Project-Specific Context

**Core Game Features:**
- **Party System**: 4-character party (Theron-Warrior, Sylvain-Ranger, Valdris-Mage, Caelum-Cleric)
- **Magic Schools**: Fire, Water, Earth magic systems with spell progression
- **Equipment System**: Weapons, armor, shields with poison enhancement capabilities
- **Currency System**: Three-tier currency (Copper/Silver/Gold) with automatic conversion
- **JSON Configuration**: All game balancing through JSON files for rapid iteration
- **Difficulty Levels**: Easy/Normal/Hard with comprehensive stat scaling
- **Enemy System**: JSON-configurable adversaries with abilities and behaviors
- **Save System**: File-based storage using Unity JsonUtility with compression

**Critical Dependencies:**
- **Unity 6.1**: MonoBehaviour, ScriptableObject, JsonUtility, Event System
- **System.IO.Abstractions**: For testable file operations and mocking
- **Standard .NET**: Collections, LINQ, IO for core functionality
- **StreamingAssets**: JSON configuration files for game balancing

**Architecture Decisions:**
- **No Database**: File-based storage sufficient for ~2K data points (~100KB total)
- **Event-Driven**: Loose coupling between systems through custom event system
- **ScriptableObject Data**: All game data (characters, equipment, spells) as assets
- **JSON Balancing**: External configuration for all numeric parameters
- **SOLID Compliance**: Strict adherence to SOLID principles throughout codebase

### Current Implementation Status

**Completed:**
- ✅ Project structure and documentation
- ✅ Technical architecture with Mermaid diagrams
- ✅ Comprehensive game design documentation
- ✅ JSON balance system design
- ✅ Enemy system architecture
- ✅ Difficulty scaling system

**Phase 1A - Current Focus:**
- 🔄 Core ScriptableObject data structures
- 🔄 BalanceManager implementation with JSON loading
- 🔄 Basic character system (Theron, Sylvain, Valdris, Caelum)
- 🔄 Equipment system with poison enhancement
- 🔄 Save/Load system with System.IO.Abstractions

**Key Files to Reference:**
- `Planning/Technical/system-architecture.md` - Complete technical specification
- `Planning/GameDesign/core-mechanics.md` - Game design and character details
- `StreamingAssets/GameBalance/GameConfig.json` - Balance configuration (to be created)
- `StreamingAssets/Enemies/EnemyDatabase.json` - Enemy definitions (to be created)

## Project Architecture Philosophy

This document captures **project-specific development requirements** for the Land of Mist RPG, building on universal Unity development patterns. Focus on implementing the specific architecture defined in our technical documentation while maintaining clean, testable, and scalable code.

### Core Architectural Principles

**Clean Architecture Pattern**: Organize Unity projects with clear separation of concerns across layers:
```
Land of Mist Unity Project Structure:
├── Assets/
│   ├── Scripts/
│   │   ├── Core/                # Game domain logic (Character, Equipment, Spell classes)
│   │   ├── Systems/             # Game systems (GameManager, BalanceManager, EnemyManager)
│   │   ├── Combat/              # Combat mechanics and turn-based logic
│   │   ├── UI/                  # Text-based UI components and menus
│   │   ├── Data/                # Save/Load system and data persistence
│   │   └── Utils/               # Utility classes and extensions
│   ├── ScriptableObjects/       # Data assets (Characters, Equipment, Spells, Enemies)
│   │   ├── Characters/          # Theron, Sylvain, Valdris, Caelum data
│   │   ├── Equipment/           # Weapons, armor, shields with poison capabilities
│   │   ├── Spells/              # Fire, Water, Earth magic school spells
│   │   └── Enemies/             # Enemy templates and boss configurations
│   ├── Prefabs/                 # UI prefabs and game object templates
│   ├── Scenes/                  # Game scenes (MainMenu, GameWorld, Combat)
│   └── Materials/               # UI materials and visual assets
├── StreamingAssets/             # JSON configuration files
│   ├── GameBalance/             # GameConfig.json for balance parameters
│   └── Enemies/                 # EnemyDatabase.json for enemy definitions
├── Planning/                    # Documentation and design specifications
│   ├── Technical/               # system-architecture.md (primary reference)
│   ├── GameDesign/              # core-mechanics.md, narrative-design.md
│   └── Production/              # development-roadmap.md
└── Tests/                       # Unit and integration tests
    ├── EditMode/                # Logic tests (balance, calculations)
    └── PlayMode/                # Integration tests (combat, save/load)
```

## SOLID Principles Implementation

**All development must strictly follow SOLID principles.** Unity projects benefit enormously from these architectural guidelines:

### S - Single Responsibility Principle
- **MonoBehaviour Scripts**: Each script has one clear purpose
  - `PlayerController` - Only handles player input and movement
  - `CombatSystem` - Only handles combat mechanics
  - `InventoryManager` - Only handles item management
- **ScriptableObjects**: Single-purpose data containers
- **Managers**: Each manager handles one specific game system

### O - Open/Closed Principle
- **ScriptableObject Systems**: Extend functionality through data assets, not code modification
- **Event-Driven Architecture**: Add new features through event subscriptions
- **Component-Based Design**: Extend GameObject behavior through composition

### L - Liskov Substitution Principle
- **Interface Contracts**: All implementations of `IWeapon`, `IEnemy`, `ISpell` must be interchangeable
- **Polymorphic Behavior**: Combat system works with any `IWeapon` implementation (sword, bow, staff)
- **Consistent Contracts**: All result types follow the same success/failure pattern

### I - Interface Segregation Principle
- **Focused Interfaces**: `ICombatant`, `IDamageable`, `IEquippable` with specific purposes
- **UI Contracts**: Separate interfaces for `IGameMenu`, `ICombatUI`, `IInventoryUI`
- **System Interfaces**: Each system exposes only necessary methods (`IBalanceProvider`, `ISaveSystem`)

### D - Dependency Inversion Principle
- **ScriptableObject Dependencies**: Systems depend on data assets, not hardcoded values
- **Event System**: Loose coupling through Unity Events or custom event systems
- **Service Locator**: Access to managers through abstractions

### SOLID Development Rules for Unity
1. **New Features**: Create new components/systems, don't expand existing ones
2. **Extensions**: Use ScriptableObjects and composition over inheritance
3. **Testing**: Each component must be testable in isolation
4. **Interfaces**: Keep system contracts minimal and focused
5. **Dependencies**: Always depend on abstractions (interfaces/ScriptableObjects)

## Critical Developer Workflows

### Unity Project Development Process
- **Scene Management**: Use additive scene loading for complex game states
- **Prefab Workflows**: Maintain prefab integrity, use prefab variants for specialization
- **Asset Organization**: Consistent naming conventions and folder structure
- **Version Control**: Use Unity's .gitignore, LFS for large assets

### Testing Architecture Guidelines
**Unity requires special testing considerations:**

#### **Testing Framework Setup**
```csharp
// Use Unity Test Framework patterns
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

// EditMode tests for logic, PlayMode tests for integration
[TestFixture]
public class CombatSystemTests
{
    [Test]
    public void ShouldCalculateDamageCorrectly()
    {
        // Unit test logic without MonoBehaviour dependencies
        var weapon = ScriptableObject.CreateInstance<WeaponData>();
        weapon.baseDamage = 10;
        var result = CombatCalculator.CalculateDamage(weapon, 15); // strength 15
        Assert.AreEqual(12, result); // 10 base + 2 strength bonus
    }

    [UnityTest]
    public IEnumerator ShouldHandleCharacterLevelUp()
    {
        // Integration test with Unity components
        var character = CreateTestCharacter();
        character.GainExperience(100);
        yield return null;
        Assert.AreEqual(2, character.Level);
    }
}
```

#### **Testing Patterns**
- **Logic Testing**: Test pure C# logic in EditMode tests
- **Integration Testing**: Test MonoBehaviour interactions in PlayMode tests
- **Mock Dependencies**: Use interfaces to mock Unity-specific dependencies
- **ScriptableObject Testing**: Test data-driven systems with test assets

### Build and Deployment Workflow
- **Build Automation**: Use Unity Cloud Build or custom CI/CD pipelines
- **Platform Testing**: Test builds on target platforms regularly
- **Performance Profiling**: Regular profiling sessions, especially for mobile
- **Memory Management**: Monitor memory usage and garbage collection

## Game-Agnostic Development Patterns

### Event-Driven Architecture
**Universal pattern for game communication:**

```csharp
// Event system for loose coupling
public class GameEventManager : MonoBehaviour
{
    public static event System.Action<Character> OnCharacterHealthChanged;
    public static event System.Action<GameState> OnGameStateChanged;
    public static event System.Action<CombatResult> OnCombatEnded;
    public static event System.Action<DifficultyLevel> OnDifficultyChanged;

    public static void TriggerHealthChange(Character character)
    {
        OnCharacterHealthChanged?.Invoke(character);
    }
}

// Systems subscribe to events, not direct references
public class UIPartyPanel : MonoBehaviour
{
    void OnEnable()
    {
        GameEventManager.OnCharacterHealthChanged += UpdateCharacterDisplay;
    }

    void OnDisable()
    {
        GameEventManager.OnCharacterHealthChanged -= UpdateCharacterDisplay;
    }
}
```

### Data-Driven Development
**Use ScriptableObjects for game balance and configuration:**

```csharp
[CreateAssetMenu(fileName = "New Character", menuName = "Land of Mist/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public CharacterClass characterClass;
    public AttributeData baseAttributes;
    public EquipmentData startingEquipment;
    // Data-driven approach allows character changes without code changes
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Land of Mist/Weapon")]
public class WeaponData : ScriptableObject, IWeapon
{
    public WeaponType weaponType;
    public int baseDamage;
    public float criticalChance;
    public bool canBeEnhanced;
    public PoisonType supportedPoisons;
    // Equipment configured through data assets
}
```

### Result Pattern Implementation
**Consistent error handling across all systems:**

```csharp
public class GameResult<T>
{
    public bool Success { get; private set; }
    public T Data { get; private set; }
    public string ErrorMessage { get; private set; }

    public static GameResult<T> Success(T data)
    {
        return new GameResult<T> { Success = true, Data = data };
    }

    public static GameResult<T> Failure(string error)
    {
        return new GameResult<T> { Success = false, ErrorMessage = error };
    }
}

// Usage in game systems
public GameResult<bool> AttemptPurchase(Item item, int playerGold)
{
    if (playerGold < item.cost)
        return GameResult<bool>.Failure("Insufficient gold");

    return GameResult<bool>.Success(true);
}
```

## Universal Testing Strategies

### Test Organization
**Mirror your production code structure in tests:**

```
Tests/
├── EditMode/
│   ├── Core/               # Logic tests
│   ├── Systems/            # System logic tests
│   └── Utils/              # Utility tests
└── PlayMode/
    ├── Integration/        # Full system tests
    ├── UI/                 # UI interaction tests
    └── Performance/        # Performance tests
```

### Test-Driven Development Patterns
1. **Red-Green-Refactor**: Write failing test → Make it pass → Refactor
2. **Arrange-Act-Assert**: Clear test structure for all test cases
3. **Mock External Dependencies**: Use interfaces to isolate systems under test
4. **Test Data Management**: Use ScriptableObjects for test data consistency

### Critical Test Coverage Areas
- **Game Logic**: Core mechanics, calculations, state transitions
- **Save/Load Systems**: Data persistence and retrieval
- **UI Interactions**: Menu navigation, input handling
- **Performance**: Frame rate, memory usage, loading times
- **Platform Compatibility**: Different screen sizes, input methods

## Development Workflow Excellence

### Version Control Best Practices
**Git workflow optimized for Unity development:**

```bash
# Meaningful commit messages with context
git commit -m "feat: implement player inventory system with drag-drop UI

- Add InventoryManager singleton for item management
- Create draggable UI components for inventory slots
- Implement item stacking and sorting functionality
- Add comprehensive test coverage for inventory operations
- Support for different item types (weapons, consumables, quest items)

Closes #42"
```

### Code Review Guidelines
- **Architecture Review**: Does this follow SOLID principles?
- **Performance Impact**: Any potential performance bottlenecks?
- **Testing Coverage**: Are critical paths tested?
- **Unity Best Practices**: Proper use of Unity patterns and lifecycle?

### Documentation Standards
- **System Documentation**: How each major system works
- **API Documentation**: Public interface documentation
- **Setup Instructions**: How to run and test the project
- **Architecture Decisions**: Why certain patterns were chosen

## Performance and Optimization Principles

### Memory Management
- **Object Pooling**: Reuse GameObjects instead of instantiate/destroy
- **Garbage Collection**: Minimize allocations in Update loops
- **Texture Memory**: Use appropriate texture formats and compression
- **Audio Management**: Load/unload audio clips based on context

### Performance Monitoring
- **Profiler Integration**: Regular profiling sessions during development
- **Performance Budgets**: Set and monitor frame time budgets
- **Memory Budgets**: Monitor memory usage across different platforms
- **Build Size**: Keep track of build size growth

## Project Management Integration

### Issue Tracking
**Use GitHub Issues effectively for Unity projects:**

```markdown
# Bug Report Template
## 🐛 Bug Description
Brief description of the bug

## 🔍 Steps to Reproduce
1. Step one
2. Step two
3. Step three

## 📱 Platform Information
- Unity Version: 6.1.0f1
- Platform: Windows/Mac/Mobile
- Build Configuration: Debug/Release

## 📊 Expected vs Actual Behavior
- Expected: What should happen
- Actual: What actually happens

## 🔧 Additional Context
- Console errors
- Performance impact
- Related systems affected
```

### Task Management
- **Feature Branches**: Use feature branches for new development
- **Pull Requests**: Require code review before merging
- **Continuous Integration**: Automated testing on commits
- **Release Planning**: Regular release cycles with proper testing

## Error Handling and Debugging

### Comprehensive Error Handling
```csharp
public class GameLogger : MonoBehaviour
{
    public static void LogError(string system, string message, System.Exception ex = null)
    {
        string logMessage = $"[{system}] {message}";
        if (ex != null)
            logMessage += $"\nException: {ex.Message}\nStack: {ex.StackTrace}";

        Debug.LogError(logMessage);

        // Optional: Send to analytics/crash reporting
        // AnalyticsSystem.LogError(system, message, ex);
    }
}
```

### Debug Tools Development
- **Debug Menus**: In-game debug menus for testing
- **Gizmos**: Visual debugging tools in Scene view
- **Console Commands**: Runtime command system for testing
- **Performance Overlays**: Real-time performance metrics

## Cross-Platform Considerations

### Platform-Specific Code
```csharp
#if UNITY_ANDROID
    // Android-specific code
#elif UNITY_IOS
    // iOS-specific code
#elif UNITY_STANDALONE
    // Desktop-specific code
#endif
```

### Input System Abstraction
- **Input Interfaces**: Abstract input handling for different platforms
- **Touch vs Mouse**: Handle different input methods gracefully
- **Controller Support**: Gamepad support where appropriate
- **Accessibility**: Consider accessibility features from the start

## Security and Data Protection

### Save Data Security
- **Encryption**: Encrypt sensitive save data
- **Validation**: Validate loaded data for corruption/tampering
- **Backup Systems**: Multiple save slots and backup mechanisms
- **Cloud Integration**: Consider cloud save integration

### User Privacy
- **Analytics**: Be transparent about data collection
- **Permissions**: Request only necessary permissions
- **GDPR Compliance**: Consider privacy regulations
- **Data Minimization**: Collect only necessary user data

## Conclusion

These instructions provide a **language-agnostic**, **game-agnostic** foundation for Unity development based on proven architectural patterns and development practices. The emphasis on SOLID principles, comprehensive testing, and professional development workflows ensures that Unity projects remain maintainable, scalable, and robust throughout their development lifecycle.

**Key Takeaways:**
1. **Architecture First**: Plan system interactions before implementation
2. **Test Everything**: Comprehensive testing saves time and prevents bugs
3. **Document Decisions**: Future developers (including yourself) will thank you
4. **Performance Matters**: Consider performance impact of architectural decisions
5. **Iterate Professionally**: Use proper version control and issue tracking

Apply these principles consistently to create Unity games that are not only fun to play but also maintainable and professional in their implementation.
