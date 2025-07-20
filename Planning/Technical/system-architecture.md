# System Architecture - Land of Mist RPG

## Purpose

Define the technical architecture for the Unity text-based RPG, focusing on SOLID principles, scalable design patterns, and maintainable code structure.

## Architecture Overview

### High-Level System Design

```text
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   UI Layer      │    │  Game Logic     │    │   Data Layer    │
│                 │    │                 │    │                 │
│ - Text Renderer │◄──►│ - Game Manager  │◄──►│ - Save System   │
│ - Input Handler │    │ - Combat System │    │ - ScriptableObj │
│ - UI Manager    │    │ - Party Manager │    │ - Config Data   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         └───────────────────────┼───────────────────────┘
                                 │
                    ┌─────────────────┐
                    │  Event System   │
                    │                 │
                    │ - Game Events   │
                    │ - UI Events     │
                    │ - Combat Events │
                    └─────────────────┘
```

## Core System Components

### 1. Game Manager (Singleton)

**Responsibilities:**

- Game state management
- Scene transitions
- Global game rules enforcement
- System coordination

**Implementation:**

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState currentState;
    [SerializeField] private PartyManager partyManager;
    [SerializeField] private CombatManager combatManager;

    // SOLID: Single responsibility - only manages game state
}
```

### 2. Party Management System

**Components:**

- `PartyManager`: Overall party coordination
- `Character`: Individual character data and behavior
- `CharacterClass`: ScriptableObject defining class properties

**Data Flow:**

```text
PartyManager
    ├── Character[0] (Warrior)
    ├── Character[1] (Ranger)
    ├── Character[2] (Mage)
    └── Character[3] (Cleric)
```

### 3. Combat System Architecture

**Turn-Based Combat Flow:**

1. Initialize combat encounter
2. Calculate initiative order
3. Process character turns sequentially
4. Apply damage/effects
5. Check victory conditions
6. Resolve combat

**Key Interfaces:**

```csharp
public interface ICombatant
{
    int Initiative { get; }
    bool IsAlive { get; }
    void TakeDamage(int damage);
    CombatAction ChooseAction();
}

public interface IWeapon
{
    WeaponType Type { get; }
    WeaponHandedness Handedness { get; } // OneHanded, TwoHanded
    int BaseDamage { get; }
    float CriticalChance { get; }
    bool CanBeEnhanced { get; }
    PoisonType AppliedPoison { get; }
    int PoisonCharges { get; }
    bool HasArmorPenetration { get; }
}

public interface IArmor
{
    ArmorType Type { get; } // Light, Medium, Heavy
    int DefenseBonus { get; }
    float MovementPenalty { get; }
    float StealthPenalty { get; }
    ElementalResistance[] Resistances { get; }
}

public interface IShield
{
    ShieldType Type { get; } // Buckler, Round, Tower, Magic
    int DefenseBonus { get; }
    float BlockChance { get; }
    bool BlocksRangedAttacks { get; }
    ElementalResistance[] Resistances { get; }
}

public interface ISpell
{
    SpellSchool School { get; }
    int ManaCost { get; }
    TargetType Target { get; }
    void Cast(ICombatant caster, ICombatant[] targets);
}
```

### 4. Currency System

**Implementation Strategy:**

```csharp
[System.Serializable]
public struct Currency
{
    private int totalCopper;

    public int Gold => totalCopper / 10000;
    public int Silver => (totalCopper % 10000) / 100;
    public int Copper => totalCopper % 100;

    public static Currency FromCopper(int copper) => new Currency { totalCopper = copper };
    public static Currency operator +(Currency a, Currency b) => FromCopper(a.totalCopper + b.totalCopper);
}
```

## Data Architecture

### ScriptableObject Design

**Character Classes:**

```csharp
[CreateAssetMenu(fileName = "New Character Class", menuName = "RPG/Character Class")]
public class CharacterClassData : ScriptableObject
{
    public string className;
    public AttributeModifiers baseAttributes;
    public SkillProgression[] skillTrees;
    public EquipmentRestrictions allowedEquipment;
}
```

**Equipment System:**

```csharp
[CreateAssetMenu(fileName = "New Weapon", menuName = "RPG/Weapon")]
public class WeaponData : ScriptableObject, IWeapon
{
    public WeaponType weaponType;
    public WeaponHandedness handedness;
    public int baseDamage;
    public float criticalChance;
    public bool canBeEnhanced;
    public bool hasArmorPenetration;
    public float poisonEffectiveness;
    public AttributeRequirements requirements;

    [Header("Runtime Poison State")]
    public PoisonType appliedPoison;
    public int poisonCharges;
}

[CreateAssetMenu(fileName = "New Armor", menuName = "RPG/Armor")]
public class ArmorData : ScriptableObject, IArmor
{
    public ArmorType armorType;
    public int defenseBonus;
    public float movementPenalty;
    public float stealthPenalty;
    public ElementalResistance[] resistances;
    public AttributeRequirements requirements;
}

[CreateAssetMenu(fileName = "New Shield", menuName = "RPG/Shield")]
public class ShieldData : ScriptableObject, IShield
{
    public ShieldType shieldType;
    public int defenseBonus;
    public float blockChance;
    public bool blocksRangedAttacks;
    public ElementalResistance[] resistances;
    public AttributeRequirements requirements;
}
```

**Spell System:**

```csharp
[CreateAssetMenu(fileName = "New Spell", menuName = "RPG/Spell")]
public class SpellData : ScriptableObject, ISpell
{
    public SpellSchool school;
    public int manaCost;
    public TargetType targetType;
    public EffectData[] effects;
}
```

## UI Architecture

### Text-Based UI System

**Component Hierarchy:**

```text
UIManager
├── GameHUD
│   ├── PartyStatusPanel
│   ├── CurrencyDisplay
│   └── MessageLog
├── CombatUI
│   ├── InitiativeOrder
│   ├── ActionButtons
│   └── TargetSelection
└── MenuSystem
    ├── InventoryMenu
    ├── CharacterSheet
    └── SettingsMenu
```

**UI Event System:**

```csharp
public static class UIEvents
{
    public static event System.Action<string> OnDisplayMessage;
    public static event System.Action<Character> OnCharacterUpdated;
    public static event System.Action<Currency> OnCurrencyChanged;
}
```

## Performance Considerations

### Memory Management

**Object Pooling for Combat:**

- Damage number displays
- Effect particles (if any)
- UI elements that appear/disappear frequently

**Efficient Text Rendering:**

- Text mesh caching for static elements
- StringBuilder for dynamic text generation
- Minimal string concatenation in Update loops

### Save System Architecture

**Save Data Structure:**

```csharp
[System.Serializable]
public class SaveData
{
    public GameProgress gameProgress;
    public PartyData partyData;
    public Currency playerCurrency;
    public InventoryData inventory;
    public Dictionary<string, bool> gameFlags;
}
```

**Save Strategy:**

- JSON serialization for human readability
- Compression for file size optimization
- Versioning system for save compatibility
- Backup saves for corruption protection

## Event-Driven Architecture

### Core Event Categories

**Game Events:**

```csharp
public static class GameEvents
{
    public static event System.Action<GameState> OnGameStateChanged;
    public static event System.Action<int> OnExperienceGained;
    public static event System.Action<Character> OnCharacterLevelUp;
}
```

**Combat Events:**

```csharp
public static class CombatEvents
{
    public static event System.Action OnCombatStart;
    public static event System.Action<ICombatant> OnCombatantTurn;
    public static event System.Action<CombatResult> OnCombatEnd;
}
```

## Testing Architecture

### Unit Testing Structure

**Test Categories:**

- **Logic Tests**: Pure C# logic without Unity dependencies
- **Integration Tests**: System interactions with mocked dependencies
- **UI Tests**: Interface behavior and event handling

**Example Test Structure:**

```csharp
[TestFixture]
public class CurrencySystemTests
{
    [Test]
    public void Currency_AutoConversion_ShouldPromoteCopper()
    {
        // Arrange
        var currency = Currency.FromCopper(150);

        // Act & Assert
        Assert.AreEqual(1, currency.Silver);
        Assert.AreEqual(50, currency.Copper);
    }
}
```

### Test Data Management

**ScriptableObject Test Assets:**

- Test character classes with known values
- Mock equipment for damage calculations
- Predictable spell data for combat tests

## Deployment Considerations

### Platform Targets

- **Primary**: PC (Windows/Mac/Linux)
- **Secondary**: Mobile (with adapted UI)
- **Stretch**: Web (WebGL build)

### Build Configuration

- Development builds with debug UI
- Release builds with optimized text rendering
- Platform-specific input handling

### Performance Targets

- **Frame Rate**: 60 FPS (text-based, should be easily achievable)
- **Memory**: <500MB RAM usage
- **Load Times**: <2 seconds for scene transitions
- **Save/Load**: <1 second for game state persistence

This architecture ensures the game follows SOLID principles while maintaining the flexibility to add new features and content through data-driven design.
