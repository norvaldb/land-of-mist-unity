# Unity Game Development - AI Coding Agent Instructions

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Project Architecture Philosophy

This document captures **universal development lessons** learned from the Land of Mist RPG project, adapted for **Unity game development**. Focus on architectural patterns, testing strategies, and development workflows that ensure maintainable, scalable game projects.

### Core Architectural Principles

**Clean Architecture Pattern**: Organize Unity projects with clear separation of concerns across layers:
```
Unity Project Structure:
├── Scripts/
│   ├── Core/                    # Game domain logic (entities, game rules)
│   ├── Systems/                 # Game systems (singleton MonoBehaviours)
│   ├── UI/                      # User interface components and controllers
│   ├── Data/                    # Data persistence and serialization
│   └── Utils/                   # Utility classes and extensions
├── Prefabs/                     # Reusable game objects
├── Scenes/                      # Game scenes
├── Resources/                   # Runtime-loaded assets
├── StreamingAssets/             # Platform-specific assets
└── Editor/                      # Custom editor tools
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
- **Interface Contracts**: All implementations of `IWeapon`, `IEnemy`, `IUI` must be interchangeable
- **Polymorphic Behavior**: Combat system works with any `IWeapon` implementation
- **Consistent Contracts**: All result types follow the same success/failure pattern

### I - Interface Segregation Principle
- **Focused Interfaces**: `IMovable`, `IDamageable`, `IInteractable` with specific purposes
- **UI Contracts**: Separate interfaces for different UI concerns
- **System Interfaces**: Each system exposes only necessary methods

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
    }
    
    [UnityTest]
    public IEnumerator ShouldHandlePlayerDeathSequence()
    {
        // Integration test with Unity components
        yield return null;
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
    public static event System.Action<int> OnPlayerHealthChanged;
    public static event System.Action<string> OnGameStateChanged;
    
    public static void TriggerHealthChange(int newHealth)
    {
        OnPlayerHealthChanged?.Invoke(newHealth);
    }
}

// Systems subscribe to events, not direct references
public class UIHealthBar : MonoBehaviour
{
    void OnEnable()
    {
        GameEventManager.OnPlayerHealthChanged += UpdateHealthBar;
    }
    
    void OnDisable()
    {
        GameEventManager.OnPlayerHealthChanged -= UpdateHealthBar;
    }
}
```

### Data-Driven Development
**Use ScriptableObjects for game balance and configuration:**

```csharp
[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health;
    public int damage;
    public float speed;
    // Data-driven approach allows balance changes without code changes
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
- Unity Version: 2023.3.0f1
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
