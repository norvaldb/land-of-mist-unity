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

## Game Balancing System

### JSON Configuration Architecture

**Purpose:** Enable game designers to adjust all gameplay parameters without touching code, supporting rapid iteration and live balancing updates.

### Configuration File Structure

**Master Configuration:** `StreamingAssets/GameBalance/GameConfig.json`

```json
{
  "version": "1.0.0",
  "lastModified": "2025-07-20T10:30:00Z",
  "combat": {
    "damageMultipliers": {
      "oneHandedWeapons": 1.0,
      "twoHandedWeapons": 1.25,
      "rangedWeapons": 0.9,
      "magic": 1.1
    },
    "armorEffectiveness": {
      "lightArmor": 0.8,
      "mediumArmor": 1.0,
      "heavyArmor": 1.3
    },
    "criticalHitChance": {
      "base": 0.05,
      "knife": 0.15,
      "sword": 0.08,
      "axe": 0.12,
      "bow": 0.1
    },
    "poisonDamage": {
      "basicPoison": {
        "damagePerTurn": 3,
        "duration": 3,
        "cost": 50
      },
      "paralyticPoison": {
        "stunChance": 0.3,
        "duration": 1,
        "cost": 120
      },
      "weakeningPoison": {
        "strengthReduction": 0.25,
        "duration": 5,
        "cost": 80
      },
      "deadlyPoison": {
        "damagePerTurn": 8,
        "duration": 4,
        "cost": 200
      }
    }
  },
  "progression": {
    "experienceTable": [
      100, 250, 450, 700, 1000, 1350, 1750, 2200, 2700, 3250,
      3850, 4500, 5200, 5950, 6750, 7600, 8500, 9450, 10450, 11500
    ],
    "attributeGrowth": {
      "healthPerLevel": 5,
      "manaPerLevel": 3,
      "attributePointsPerLevel": 1
    },
    "classModifiers": {
      "warrior": {
        "healthMultiplier": 1.5,
        "manaMultiplier": 0.5,
        "damageMultiplier": 1.2
      },
      "ranger": {
        "healthMultiplier": 1.0,
        "manaMultiplier": 0.8,
        "damageMultiplier": 1.0
      },
      "mage": {
        "healthMultiplier": 0.7,
        "manaMultiplier": 1.8,
        "damageMultiplier": 0.8
      },
      "cleric": {
        "healthMultiplier": 1.1,
        "manaMultiplier": 1.3,
        "damageMultiplier": 0.9
      }
    }
  },
  "economy": {
    "startingCurrency": 500,
    "shopPrices": {
      "weapons": {
        "knife": 150,
        "sword": 800,
        "axe": 900,
        "greatSword": 2500,
        "greatAxe": 2800,
        "spear": 600,
        "bow": 1200,
        "crossbow": 1800,
        "staff": 1500
      },
      "armor": {
        "lightArmor": 500,
        "mediumArmor": 1500,
        "heavyArmor": 4000
      },
      "shields": {
        "buckler": 300,
        "roundShield": 800,
        "towerShield": 2000,
        "magicShield": 5000
      },
      "consumables": {
        "healthPotion": 50,
        "manaPotion": 75,
        "basicPoisonVial": 50,
        "paralyticPoisonVial": 120,
        "weakeningPoisonVial": 80,
        "deadlyPoisonVial": 200
      }
    },
    "currencyConversion": {
      "copperToSilver": 100,
      "silverToGold": 100
    }
  },
  "magic": {
    "manaCosts": {
      "fire": {
        "fireball": 8,
        "flameBurst": 12,
        "igniteWeapon": 6
      },
      "water": {
        "healingSpring": 10,
        "tidalWave": 15,
        "purify": 5,
        "mistForm": 8
      },
      "earth": {
        "stoneArmor": 12,
        "earthquake": 18,
        "entangle": 10,
        "boulderThrow": 14
      }
    },
    "spellEffects": {
      "healingSpring": {
        "healPerTurn": 8,
        "duration": 3
      },
      "stoneArmor": {
        "defenseBonus": 5,
        "movementPenalty": 0.3,
        "duration": 10
      },
      "igniteWeapon": {
        "bonusDamage": 4,
        "duration": 5
      }
    },
    "manaRegeneration": {
      "basePerTurn": 2,
      "staffBonus": 1.5,
      "restMultiplier": 3.0
    }
  }
}
```

### Configuration Management System

**Configuration Loader:**

```csharp
[System.Serializable]
public class GameBalanceConfig
{
    public string version;
    public string lastModified;
    public CombatConfig combat;
    public ProgressionConfig progression;
    public EconomyConfig economy;
    public MagicConfig magic;
}

public class BalanceManager : MonoBehaviour
{
    public static BalanceManager Instance { get; private set; }
    
    [SerializeField] private GameBalanceConfig config;
    
    private void Awake()
    {
        Instance = this;
        LoadConfiguration();
    }
    
    private void LoadConfiguration()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "GameBalance", "GameConfig.json");
        
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            config = JsonUtility.FromJson<GameBalanceConfig>(json);
            ValidateConfiguration();
        }
        else
        {
            CreateDefaultConfiguration();
        }
    }
    
    public float GetDamageMultiplier(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.OneHanded => config.combat.damageMultipliers.oneHandedWeapons,
            WeaponType.TwoHanded => config.combat.damageMultipliers.twoHandedWeapons,
            WeaponType.Ranged => config.combat.damageMultipliers.rangedWeapons,
            _ => 1.0f
        };
    }
    
    public int GetExperienceRequired(int level)
    {
        if (level > 0 && level <= config.progression.experienceTable.Length)
            return config.progression.experienceTable[level - 1];
        return int.MaxValue;
    }
    
    public int GetItemPrice(string category, string itemName)
    {
        // Dynamic price lookup from JSON configuration
        return config.economy.shopPrices.GetPrice(category, itemName);
    }
}
```

### Hot-Reload Configuration System

**Live Configuration Updates:**

```csharp
public class ConfigurationWatcher : MonoBehaviour
{
    private FileSystemWatcher fileWatcher;
    private string configDirectory;
    
    private void Start()
    {
        configDirectory = Path.Combine(Application.streamingAssetsPath, "GameBalance");
        SetupFileWatcher();
    }
    
    private void SetupFileWatcher()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        fileWatcher = new FileSystemWatcher(configDirectory, "*.json");
        fileWatcher.Changed += OnConfigurationChanged;
        fileWatcher.EnableRaisingEvents = true;
        #endif
    }
    
    private void OnConfigurationChanged(object sender, FileSystemEventArgs e)
    {
        // Reload configuration on file change (development builds only)
        if (e.Name == "GameConfig.json")
        {
            StartCoroutine(ReloadConfigurationDelayed());
        }
    }
    
    private IEnumerator ReloadConfigurationDelayed()
    {
        yield return new WaitForSeconds(0.5f); // Wait for file write to complete
        BalanceManager.Instance.LoadConfiguration();
        GameEvents.OnConfigurationReloaded?.Invoke();
    }
}
```

### Configuration Validation System

**Data Integrity Checks:**

```csharp
public class ConfigurationValidator
{
    public static List<string> ValidateConfiguration(GameBalanceConfig config)
    {
        var errors = new List<string>();
        
        // Validate damage multipliers are reasonable
        if (config.combat.damageMultipliers.twoHandedWeapons <= config.combat.damageMultipliers.oneHandedWeapons)
            errors.Add("Two-handed weapons should have higher damage multiplier than one-handed");
            
        // Validate experience table progression
        for (int i = 1; i < config.progression.experienceTable.Length; i++)
        {
            if (config.progression.experienceTable[i] <= config.progression.experienceTable[i - 1])
                errors.Add($"Experience required for level {i + 1} should be higher than level {i}");
        }
        
        // Validate price consistency
        if (config.economy.shopPrices.weapons.greatSword <= config.economy.shopPrices.weapons.sword)
            errors.Add("Great sword should cost more than regular sword");
            
        // Validate mana costs are positive
        foreach (var school in new[] { config.magic.manaCosts.fire, config.magic.manaCosts.water, config.magic.manaCosts.earth })
        {
            var costs = school.GetType().GetFields();
            foreach (var cost in costs)
            {
                if ((int)cost.GetValue(school) <= 0)
                    errors.Add($"Mana cost for {cost.Name} must be positive");
            }
        }
        
        return errors;
    }
}
```

### Environment-Specific Configuration

**Configuration Overrides for Testing:**

```json
// StreamingAssets/GameBalance/TestConfig.json
{
  "baseConfig": "GameConfig.json",
  "overrides": {
    "combat.damageMultipliers.oneHandedWeapons": 10.0,
    "progression.experienceTable": [10, 20, 30, 40, 50],
    "economy.startingCurrency": 100000
  }
}
```

**Configuration Loading Logic:**

```csharp
public void LoadTestConfiguration()
{
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    string testConfigPath = Path.Combine(Application.streamingAssetsPath, "GameBalance", "TestConfig.json");
    if (File.Exists(testConfigPath))
    {
        LoadConfiguration(); // Load base config first
        ApplyConfigurationOverrides(testConfigPath);
    }
    #endif
}
```

### Benefits of JSON Configuration System

1. **Designer Empowerment**: Game designers can adjust values without programmer intervention
2. **Rapid Iteration**: Changes take effect immediately without recompilation
3. **Version Control**: Configuration changes are tracked alongside code changes
4. **A/B Testing**: Easy to create different balance configurations for testing
5. **Live Balancing**: Post-release balance updates through configuration patches
6. **Platform Specific**: Different configurations for mobile vs desktop
7. **Debugging**: Test configurations for development and QA
8. **Validation**: Built-in checks ensure configuration integrity

This architecture ensures the game follows SOLID principles while maintaining the flexibility to add new features and content through data-driven design, with comprehensive JSON-based balancing capabilities.
