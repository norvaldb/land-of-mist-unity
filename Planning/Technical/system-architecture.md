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

**File-Based Data Storage Strategy:**

For this text-based RPG with limited data volume (~2,000 data points maximum), a database is unnecessary. File-based storage provides optimal simplicity, performance, and maintainability.

**Data Storage Analysis:**

```text
Static Data (ScriptableObjects):
- Character classes: ~20 assets × 500 bytes = 10KB
- Equipment definitions: ~100 items × 200 bytes = 20KB
- Spell data: ~50 spells × 300 bytes = 15KB
- Balance config: ~5KB JSON file
Total Static: ~50KB

Dynamic Data (Save Files):
- Party data: 4 characters × 2KB = 8KB
- Inventory: ~100 items × 100 bytes = 10KB
- Progress flags: ~500 bools × 1 byte = 500 bytes
- Game state: ~2KB
Total per Save: ~20KB
```

**Save Data Structure:**

```csharp
[System.Serializable]
public class SaveData
{
    public string version = "1.0.0";
    public DateTime saveDate;
    public GameProgress gameProgress;
    public PartyData partyData;
    public Currency playerCurrency;
    public InventoryData inventory;
    public Dictionary<string, bool> gameFlags;
    public Dictionary<string, int> questProgress;
    public SceneTransitionData currentLocation;
}

[System.Serializable]
public class PartyData
{
    public CharacterSaveData[] characters = new CharacterSaveData[4];
    public int activePartySize;
}

[System.Serializable]
public class CharacterSaveData
{
    public string characterName;
    public string className;
    public int level;
    public int experience;
    public AttributeData attributes;
    public EquipmentData equipment;
    public StatusEffectData[] activeEffects;
}
```

**File-Based Save Strategy:**

```csharp
public class SaveManager : MonoBehaviour
{
    private static readonly string SAVE_DIRECTORY = "GameSaves";
    private static readonly string BACKUP_DIRECTORY = "GameSaves/Backups";

    public void SaveGame(int slotNumber)
    {
        var saveData = CollectSaveData();

        // Primary save
        string savePath = GetSaveFilePath(slotNumber);
        string json = JsonUtility.ToJson(saveData, prettyPrint: true);

        // Create backup before overwriting
        CreateBackup(slotNumber);

        // Write with compression
        byte[] compressed = Compress(json);
        File.WriteAllBytes(savePath, compressed);

        // Verify save integrity
        ValidateSaveFile(savePath);
    }

    public SaveData LoadGame(int slotNumber)
    {
        string savePath = GetSaveFilePath(slotNumber);

        if (!File.Exists(savePath))
            return null;

        try
        {
            byte[] compressed = File.ReadAllBytes(savePath);
            string json = Decompress(compressed);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Save corruption detected, attempting backup restore: {ex.Message}");
            return LoadBackup(slotNumber);
        }
    }
}
```

**Save Strategy Benefits:**

- **JSON Serialization**: Human-readable for debugging and modding
- **Compression**: GZip compression reduces file size by ~70%
- **Versioning**: Save format versioning for future compatibility
- **Backup System**: Automatic backups prevent data loss
- **Validation**: Checksum validation detects file corruption
- **Multiple Slots**: Support for multiple save files
- **Cross-Platform**: Works identically across all Unity platforms

### Database vs File Storage Decision Matrix

**When to Use File-Based Storage (Our Choice):**

✅ **Perfect for Land of Mist RPG:**

- Data volume < 100MB
- Single-player experience
- Simple data relationships
- Infrequent data access patterns
- Need for save file portability
- Rapid development requirements

**When Databases Become Necessary:**

❌ **Not needed for this project, but useful for:**

- **Multiplayer Games**: Concurrent player data access
- **Large Scale**: >10,000 items, complex item relationships
- **Analytics**: Player behavior tracking and analysis
- **Live Operations**: Server-side events, leaderboards
- **Complex Queries**: "Show all fire weapons owned by rangers level 10+"
- **Real-time Updates**: Live economy, auction houses

**Comparison Table:**

| Aspect | File-Based (Our Choice) | Database |
|--------|------------------------|----------|
| **Setup Complexity** | Minimal | Moderate-High |
| **Data Volume** | <100MB ✅ | >100MB |
| **Query Complexity** | Simple lookups ✅ | Complex relationships |
| **Concurrent Access** | Single user ✅ | Multiple users |
| **Backup/Sync** | File copy ✅ | Export/Import |
| **Modding Support** | Easy JSON editing ✅ | Requires tools |
| **Performance** | Excellent for small data ✅ | Better for large datasets |
| **Debugging** | Human-readable JSON ✅ | Requires DB tools |

### Future Considerations

**If the game grows beyond current scope:**

```csharp
// Easy migration path if needed later
public interface IDataStorage
{
    Task<SaveData> LoadGameAsync(int slotNumber);
    Task SaveGameAsync(SaveData data, int slotNumber);
    Task<InventoryItem[]> GetInventoryAsync();
}

// Current implementation
public class FileDataStorage : IDataStorage { }

// Future database implementation
public class DatabaseStorage : IDataStorage { }
```

**Migration Strategy:**

1. **Phase 1** (Current): File-based for MVP
2. **Phase 2** (If needed): Hybrid approach (files for saves, DB for analytics)
3. **Phase 3** (Multiplayer): Full database transition

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
  },
  "difficulty": {
    "easy": {
      "playerDamageMultiplier": 1.25,
      "enemyDamageMultiplier": 0.8,
      "enemyHealthMultiplier": 0.9,
      "experienceMultiplier": 1.2,
      "lootMultiplier": 1.1,
      "encounterFrequency": 0.8,
      "bossPhaseThresholds": [70, 35],
      "saveOnDeath": true,
      "permadeath": false,
      "description": "Recommended for new players. Increased damage dealt, reduced enemy strength, more experience and loot."
    },
    "normal": {
      "playerDamageMultiplier": 1.0,
      "enemyDamageMultiplier": 1.0,
      "enemyHealthMultiplier": 1.0,
      "experienceMultiplier": 1.0,
      "lootMultiplier": 1.0,
      "encounterFrequency": 1.0,
      "bossPhaseThresholds": [65, 30],
      "saveOnDeath": true,
      "permadeath": false,
      "description": "Balanced experience as intended by the designers. Standard combat and progression."
    },
    "hard": {
      "playerDamageMultiplier": 0.85,
      "enemyDamageMultiplier": 1.3,
      "enemyHealthMultiplier": 1.2,
      "experienceMultiplier": 0.9,
      "lootMultiplier": 0.95,
      "encounterFrequency": 1.3,
      "bossPhaseThresholds": [60, 25],
      "saveOnDeath": false,
      "permadeath": true,
      "description": "For experienced players seeking challenge. Reduced player effectiveness, stronger enemies, permadeath."
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
    public DifficultyConfig difficulty;
}

[System.Serializable]
public class DifficultyConfig
{
    public DifficultySettings easy;
    public DifficultySettings normal;
    public DifficultySettings hard;
}

[System.Serializable]
public class DifficultySettings
{
    public float playerDamageMultiplier;
    public float enemyDamageMultiplier;
    public float enemyHealthMultiplier;
    public float experienceMultiplier;
    public float lootMultiplier;
    public float encounterFrequency;
    public int[] bossPhaseThresholds;
    public bool saveOnDeath;
    public bool permadeath;
    public string description;
}

public class BalanceManager : MonoBehaviour
{
    public static BalanceManager Instance { get; private set; }

    [SerializeField] private GameBalanceConfig config;
    [SerializeField] private DifficultyLevel currentDifficulty = DifficultyLevel.Normal;

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

    public void SetDifficulty(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;
        GameEvents.OnDifficultyChanged?.Invoke(difficulty);
    }

    public DifficultySettings GetCurrentDifficultySettings()
    {
        return currentDifficulty switch
        {
            DifficultyLevel.Easy => config.difficulty.easy,
            DifficultyLevel.Normal => config.difficulty.normal,
            DifficultyLevel.Hard => config.difficulty.hard,
            _ => config.difficulty.normal
        };
    }

    public float GetDamageMultiplier(WeaponType weaponType)
    {
        float baseMultiplier = weaponType switch
        {
            WeaponType.OneHanded => config.combat.damageMultipliers.oneHandedWeapons,
            WeaponType.TwoHanded => config.combat.damageMultipliers.twoHandedWeapons,
            WeaponType.Ranged => config.combat.damageMultipliers.rangedWeapons,
            _ => 1.0f
        };
        
        // Apply difficulty modifier
        return baseMultiplier * GetCurrentDifficultySettings().playerDamageMultiplier;
    }

    public int GetExperienceRequired(int level)
    {
        if (level > 0 && level <= config.progression.experienceTable.Length)
        {
            int baseXP = config.progression.experienceTable[level - 1];
            // Apply difficulty modifier (easier = less XP needed, harder = more XP needed)
            return Mathf.RoundToInt(baseXP / GetCurrentDifficultySettings().experienceMultiplier);
        }
        return int.MaxValue;
    }

    public int GetItemPrice(string category, string itemName)
    {
        // Dynamic price lookup from JSON configuration
        return config.economy.shopPrices.GetPrice(category, itemName);
    }

    public float GetEnemyStatMultiplier(EnemyStatType statType)
    {
        var difficultySettings = GetCurrentDifficultySettings();
        return statType switch
        {
            EnemyStatType.Health => difficultySettings.enemyHealthMultiplier,
            EnemyStatType.Damage => difficultySettings.enemyDamageMultiplier,
            EnemyStatType.Experience => difficultySettings.experienceMultiplier,
            EnemyStatType.Loot => difficultySettings.lootMultiplier,
            _ => 1.0f
        };
    }
}

public enum DifficultyLevel
{
    Easy,
    Normal,
    Hard
}

public enum EnemyStatType
{
    Health,
    Damage,
    Experience,
    Loot
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

## Adversary System Architecture

### JSON-Configurable Enemy Design

**Purpose:** Create a diverse roster of enemies with configurable stats, abilities, and behaviors through JSON files, enabling rich tactical encounters without code changes.

### Enemy Classification System

**Enemy Categories:**

```text
Adversary Hierarchy:
├── Humanoid Enemies
│   ├── Bandits & Outlaws
│   ├── Rival Adventurers
│   └── Corrupted Guards
├── Wilderness Creatures
│   ├── Beasts & Animals
│   ├── Magical Creatures
│   └── Elemental Beings
├── Undead Entities
│   ├── Skeletons & Zombies
│   ├── Wraiths & Spirits
│   └── Undead Lords
└── Boss Encounters
    ├── Named Champions
    ├── Ancient Evils
    └── Dragon-kin
```

### Enemy Configuration Files

**Master Enemy Database:** `StreamingAssets/Enemies/EnemyDatabase.json`

```json
{
  "version": "1.0.0",
  "lastModified": "2025-07-20T12:00:00Z",
  "enemyCategories": {
    "humanoids": {
      "banditRaider": {
        "name": "Bandit Raider",
        "description": "A desperate outlaw wielding crude weapons",
        "category": "humanoid",
        "subtype": "bandit",
        "level": 2,
        "attributes": {
          "strength": 12,
          "dexterity": 14,
          "constitution": 11,
          "intelligence": 8,
          "wisdom": 10,
          "charisma": 9
        },
        "combat": {
          "health": 25,
          "mana": 0,
          "armorClass": 12,
          "initiative": 14,
          "attackBonus": 3,
          "damageResistances": [],
          "statusImmunities": []
        },
        "equipment": {
          "weapon": "rustyKnife",
          "armor": "leatherVest",
          "shield": null,
          "accessories": ["dirtyBandana"]
        },
        "abilities": [
          {
            "name": "Desperate Strike",
            "type": "active",
            "description": "Increased damage when below 50% health",
            "cooldown": 3,
            "effects": [
              {
                "type": "conditionalDamage",
                "condition": "healthBelow50",
                "bonusDamage": 5
              }
            ]
          },
          {
            "name": "Coward's Retreat",
            "type": "passive",
            "description": "50% chance to flee when below 25% health",
            "effects": [
              {
                "type": "fleeChance",
                "condition": "healthBelow25",
                "chance": 0.5
              }
            ]
          }
        ],
        "loot": {
          "guaranteed": [
            {
              "item": "copper",
              "amount": "2d6"
            }
          ],
          "possible": [
            {
              "item": "rustyKnife",
              "chance": 0.3
            },
            {
              "item": "basicPoisonVial",
              "chance": 0.1
            }
          ]
        },
        "behavior": {
          "aggressionLevel": "medium",
          "preferredRange": "melee",
          "tacticalIntelligence": "low",
          "groupBehavior": "individual"
        }
      },
      "veteranSoldier": {
        "name": "Veteran Soldier",
        "description": "A skilled warrior with military training",
        "category": "humanoid",
        "subtype": "soldier",
        "level": 5,
        "attributes": {
          "strength": 16,
          "dexterity": 13,
          "constitution": 15,
          "intelligence": 12,
          "wisdom": 14,
          "charisma": 11
        },
        "combat": {
          "health": 45,
          "mana": 10,
          "armorClass": 16,
          "initiative": 13,
          "attackBonus": 6,
          "damageResistances": ["slashing"],
          "statusImmunities": ["fear"]
        },
        "equipment": {
          "weapon": "ironSword",
          "armor": "chainMail",
          "shield": "ironShield",
          "accessories": ["militaryInsignia"]
        },
        "abilities": [
          {
            "name": "Shield Bash",
            "type": "active",
            "description": "Stun target for 1 turn",
            "cooldown": 4,
            "effects": [
              {
                "type": "damage",
                "amount": "1d4+2"
              },
              {
                "type": "statusEffect",
                "effect": "stunned",
                "duration": 1
              }
            ]
          },
          {
            "name": "Combat Veteran",
            "type": "passive",
            "description": "Immune to fear and +2 to initiative",
            "effects": [
              {
                "type": "statusImmunity",
                "status": "fear"
              },
              {
                "type": "attributeBonus",
                "attribute": "initiative",
                "bonus": 2
              }
            ]
          }
        ],
        "loot": {
          "guaranteed": [
            {
              "item": "silver",
              "amount": "1d4"
            }
          ],
          "possible": [
            {
              "item": "ironSword",
              "chance": 0.4
            },
            {
              "item": "chainMail",
              "chance": 0.2
            },
            {
              "item": "healthPotion",
              "chance": 0.6
            }
          ]
        },
        "behavior": {
          "aggressionLevel": "high",
          "preferredRange": "melee",
          "tacticalIntelligence": "medium",
          "groupBehavior": "formation"
        }
      }
    },
    "wilderness": {
      "direwolf": {
        "name": "Dire Wolf",
        "description": "A massive wolf with glowing red eyes",
        "category": "beast",
        "subtype": "canine",
        "level": 3,
        "attributes": {
          "strength": 15,
          "dexterity": 16,
          "constitution": 14,
          "intelligence": 3,
          "wisdom": 12,
          "charisma": 7
        },
        "combat": {
          "health": 35,
          "mana": 0,
          "armorClass": 14,
          "initiative": 16,
          "attackBonus": 4,
          "damageResistances": [],
          "statusImmunities": ["charm"]
        },
        "equipment": {
          "weapon": "naturalClaws",
          "armor": "thickFur",
          "shield": null,
          "accessories": []
        },
        "abilities": [
          {
            "name": "Pack Leader",
            "type": "passive",
            "description": "Nearby wolves gain +2 damage",
            "effects": [
              {
                "type": "auraBonus",
                "range": 2,
                "targets": ["wolf", "direwolf"],
                "bonus": {
                  "damage": 2
                }
              }
            ]
          },
          {
            "name": "Pounce",
            "type": "active",
            "description": "Leap attack with knockdown chance",
            "cooldown": 3,
            "effects": [
              {
                "type": "damage",
                "amount": "2d6+3"
              },
              {
                "type": "statusChance",
                "effect": "prone",
                "chance": 0.6,
                "duration": 1
              }
            ]
          }
        ],
        "loot": {
          "guaranteed": [
            {
              "item": "wolfPelt",
              "amount": 1
            }
          ],
          "possible": [
            {
              "item": "wolfFang",
              "chance": 0.8
            },
            {
              "item": "copper",
              "amount": "1d4",
              "chance": 0.3
            }
          ]
        },
        "behavior": {
          "aggressionLevel": "high",
          "preferredRange": "melee",
          "tacticalIntelligence": "low",
          "groupBehavior": "pack"
        }
      },
      "forestSprite": {
        "name": "Forest Sprite",
        "description": "A mischievous fae creature wreathed in leaves",
        "category": "fae",
        "subtype": "sprite",
        "level": 4,
        "attributes": {
          "strength": 6,
          "dexterity": 18,
          "constitution": 10,
          "intelligence": 14,
          "wisdom": 16,
          "charisma": 15
        },
        "combat": {
          "health": 20,
          "mana": 40,
          "armorClass": 16,
          "initiative": 18,
          "attackBonus": 2,
          "damageResistances": ["earth"],
          "statusImmunities": ["entangle"]
        },
        "equipment": {
          "weapon": "thornDart",
          "armor": "barkSkin",
          "shield": null,
          "accessories": ["flowerCrown"]
        },
        "abilities": [
          {
            "name": "Nature's Wrath",
            "type": "active",
            "description": "Entangle all enemies in thorny vines",
            "cooldown": 5,
            "manaCost": 15,
            "effects": [
              {
                "type": "areaStatusEffect",
                "effect": "entangled",
                "duration": 2,
                "targets": "allEnemies"
              }
            ]
          },
          {
            "name": "Pixie Dust",
            "type": "active",
            "description": "Confuse target, causing them to attack randomly",
            "cooldown": 3,
            "manaCost": 8,
            "effects": [
              {
                "type": "statusEffect",
                "effect": "confused",
                "duration": 2
              }
            ]
          },
          {
            "name": "Forest Camouflage",
            "type": "passive",
            "description": "50% chance to avoid physical attacks in forest",
            "effects": [
              {
                "type": "evasionBonus",
                "condition": "terrainType:forest",
                "bonus": 0.5
              }
            ]
          }
        ],
        "loot": {
          "guaranteed": [
            {
              "item": "spriteEssence",
              "amount": 1
            }
          ],
          "possible": [
            {
              "item": "manaPotion",
              "chance": 0.7
            },
            {
              "item": "enchantedBerry",
              "chance": 0.5
            },
            {
              "item": "gold",
              "amount": 1,
              "chance": 0.2
            }
          ]
        },
        "behavior": {
          "aggressionLevel": "low",
          "preferredRange": "ranged",
          "tacticalIntelligence": "high",
          "groupBehavior": "support"
        }
      }
    },
    "undead": {
      "skeletonWarrior": {
        "name": "Skeleton Warrior",
        "description": "Animated bones wielding ancient weapons",
        "category": "undead",
        "subtype": "skeleton",
        "level": 3,
        "attributes": {
          "strength": 13,
          "dexterity": 14,
          "constitution": 15,
          "intelligence": 6,
          "wisdom": 8,
          "charisma": 5
        },
        "combat": {
          "health": 30,
          "mana": 0,
          "armorClass": 13,
          "initiative": 14,
          "attackBonus": 4,
          "damageResistances": ["piercing", "slashing"],
          "statusImmunities": ["poison", "fear", "charm"]
        },
        "equipment": {
          "weapon": "ancientSword",
          "armor": "brittleChainmail",
          "shield": "rottenShield",
          "accessories": ["tatteredBanner"]
        },
        "abilities": [
          {
            "name": "Undead Fortitude",
            "type": "passive",
            "description": "25% chance to ignore fatal blow and continue fighting",
            "effects": [
              {
                "type": "deathSave",
                "chance": 0.25
              }
            ]
          },
          {
            "name": "Bone Shatter",
            "type": "active",
            "description": "Explode bone fragments in area when destroyed",
            "cooldown": "onDeath",
            "effects": [
              {
                "type": "areaDamage",
                "range": 1,
                "amount": "1d6"
              }
            ]
          }
        ],
        "loot": {
          "guaranteed": [
            {
              "item": "ancientBone",
              "amount": "1d3"
            }
          ],
          "possible": [
            {
              "item": "ancientSword",
              "chance": 0.3
            },
            {
              "item": "copper",
              "amount": "1d6",
              "chance": 0.4
            }
          ]
        },
        "behavior": {
          "aggressionLevel": "medium",
          "preferredRange": "melee",
          "tacticalIntelligence": "minimal",
          "groupBehavior": "swarm"
        }
      }
    },
    "bosses": {
      "banditKing": {
        "name": "Gareth the Bandit King",
        "description": "A ruthless leader who commands fear and respect",
        "category": "humanoid",
        "subtype": "banditBoss",
        "level": 8,
        "attributes": {
          "strength": 18,
          "dexterity": 16,
          "constitution": 16,
          "intelligence": 14,
          "wisdom": 12,
          "charisma": 17
        },
        "combat": {
          "health": 80,
          "mana": 20,
          "armorClass": 18,
          "initiative": 16,
          "attackBonus": 8,
          "damageResistances": ["poison"],
          "statusImmunities": ["fear", "charm"]
        },
        "equipment": {
          "weapon": "masterworkSword",
          "armor": "reinforcedLeather",
          "shield": null,
          "accessories": ["crownOfThorns", "crimsonCloak"]
        },
        "abilities": [
          {
            "name": "Command",
            "type": "active",
            "description": "Rally all allies, giving them extra actions",
            "cooldown": 5,
            "effects": [
              {
                "type": "allyBuff",
                "effect": "extraAction",
                "duration": 1,
                "targets": "allAllies"
              }
            ]
          },
          {
            "name": "Whirlwind Attack",
            "type": "active",
            "description": "Attack all adjacent enemies",
            "cooldown": 4,
            "effects": [
              {
                "type": "meleeAreaAttack",
                "range": 1,
                "damage": "2d8+5"
              }
            ]
          },
          {
            "name": "King's Presence",
            "type": "passive",
            "description": "All allies gain +2 to attack and damage",
            "effects": [
              {
                "type": "auraBonus",
                "range": 3,
                "targets": "allies",
                "bonus": {
                  "attack": 2,
                  "damage": 2
                }
              }
            ]
          }
        ],
        "phases": [
          {
            "healthThreshold": 100,
            "abilities": ["Command", "normalAttack"],
            "behavior": "defensive"
          },
          {
            "healthThreshold": 50,
            "abilities": ["Command", "WhirlwindAttack", "normalAttack"],
            "behavior": "aggressive"
          },
          {
            "healthThreshold": 25,
            "abilities": ["WhirlwindAttack", "desperateStrike"],
            "behavior": "berserk"
          }
        ],
        "loot": {
          "guaranteed": [
            {
              "item": "gold",
              "amount": "3d10"
            },
            {
              "item": "masterworkSword",
              "amount": 1
            }
          ],
          "possible": [
            {
              "item": "crownOfThorns",
              "chance": 1.0
            },
            {
              "item": "banditKingTreasure",
              "chance": 0.8
            }
          ]
        },
        "behavior": {
          "aggressionLevel": "variable",
          "preferredRange": "melee",
          "tacticalIntelligence": "high",
          "groupBehavior": "leader"
        }
      }
    }
  }
}
```

### Enemy System Implementation

**Enemy Data Management:**

```csharp
[System.Serializable]
public class EnemyData
{
    public string name;
    public string description;
    public string category;
    public string subtype;
    public int level;

    public AttributeData attributes;
    public CombatData combat;
    public EquipmentData equipment;
    public EnemyAbility[] abilities;
    public LootData loot;
    public BehaviorData behavior;
    public PhaseData[] phases; // For boss encounters
}

[System.Serializable]
public class EnemyAbility
{
    public string name;
    public string type; // "active", "passive", "reaction"
    public string description;
    public int cooldown;
    public int manaCost;
    public EffectData[] effects;
    public ConditionData[] conditions;
}

[System.Serializable]
public class BehaviorData
{
    public string aggressionLevel; // "low", "medium", "high", "variable"
    public string preferredRange; // "melee", "ranged", "any"
    public string tacticalIntelligence; // "minimal", "low", "medium", "high"
    public string groupBehavior; // "individual", "pack", "formation", "support", "leader"
}

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] private Dictionary<string, EnemyData> enemyDatabase;
    [SerializeField] private List<GameObject> activeEnemies;

    private void Awake()
    {
        Instance = this;
        LoadEnemyDatabase();
    }

    private void LoadEnemyDatabase()
    {
        string dbPath = Path.Combine(Application.streamingAssetsPath, "Enemies", "EnemyDatabase.json");

        if (File.Exists(dbPath))
        {
            string json = File.ReadAllText(dbPath);
            var database = JsonUtility.FromJson<EnemyDatabase>(json);
            enemyDatabase = database.FlattenToDict();
        }
        else
        {
            Debug.LogError("Enemy database not found!");
        }
    }

    public GameObject SpawnEnemy(string enemyId, Vector3 position)
    {
        if (!enemyDatabase.ContainsKey(enemyId))
        {
            Debug.LogError($"Enemy {enemyId} not found in database!");
            return null;
        }

        var enemyData = enemyDatabase[enemyId];
        var scaledEnemyData = ApplyDifficultyScaling(enemyData);
        var enemyGO = CreateEnemyFromData(scaledEnemyData);
        enemyGO.transform.position = position;

        activeEnemies.Add(enemyGO);
        return enemyGO;
    }

    private EnemyData ApplyDifficultyScaling(EnemyData baseData)
    {
        var scaledData = baseData.Clone();
        var difficultySettings = BalanceManager.Instance.GetCurrentDifficultySettings();

        // Scale health
        scaledData.combat.health = Mathf.RoundToInt(
            scaledData.combat.health * difficultySettings.enemyHealthMultiplier
        );

        // Scale damage output (applies to all enemy attacks)
        scaledData.combat.baseDamageMultiplier = difficultySettings.enemyDamageMultiplier;

        // Scale experience reward
        scaledData.experienceReward = Mathf.RoundToInt(
            scaledData.experienceReward * difficultySettings.experienceMultiplier
        );

        // Scale loot chances and amounts
        foreach (var lootItem in scaledData.loot.possible)
        {
            lootItem.chance *= difficultySettings.lootMultiplier;
            lootItem.chance = Mathf.Clamp01(lootItem.chance); // Keep within 0-1 range
        }

        return scaledData;
    }

    private GameObject CreateEnemyFromData(EnemyData data)
    {
        var enemyGO = new GameObject(data.name);
        var enemy = enemyGO.AddComponent<Enemy>();

        // Initialize enemy with difficulty-scaled data
        enemy.Initialize(data);

        return enemyGO;
    }
}
```

**Enemy Behavior AI:**

```csharp
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Enemy enemy;
    [SerializeField] private float decisionDelay = 1f;

    public CombatAction ChooseAction()
    {
        return enemyData.behavior.tacticalIntelligence switch
        {
            "minimal" => ChooseMinimalAction(),
            "low" => ChooseLowIntelligenceAction(),
            "medium" => ChooseMediumIntelligenceAction(),
            "high" => ChooseHighIntelligenceAction(),
            _ => ChooseMinimalAction()
        };
    }

    private CombatAction ChooseMinimalAction()
    {
        // Simple: Always attack nearest enemy
        var target = FindNearestTarget();
        return new AttackAction(target);
    }

    private CombatAction ChooseLowIntelligenceAction()
    {
        // Basic tactics: Use abilities if available, otherwise attack
        var availableAbilities = GetAvailableAbilities();

        if (availableAbilities.Count > 0 && Random.value < 0.3f)
        {
            var ability = availableAbilities[Random.Range(0, availableAbilities.Count)];
            return new AbilityAction(ability, SelectTargetForAbility(ability));
        }

        return new AttackAction(FindNearestTarget());
    }

    private CombatAction ChooseMediumIntelligenceAction()
    {
        // Tactical: Consider health, positioning, and ability synergies

        // Prioritize healing if health is low
        if (enemy.HealthPercent < 0.3f)
        {
            var healingAbility = FindHealingAbility();
            if (healingAbility != null)
                return new AbilityAction(healingAbility, enemy);
        }

        // Use area abilities when multiple targets are clustered
        var areaAbility = FindAreaAbility();
        if (areaAbility != null && CountTargetsInRange(areaAbility) >= 2)
        {
            return new AbilityAction(areaAbility, FindOptimalAreaTarget(areaAbility));
        }

        // Default to attacking weakest target
        return new AttackAction(FindWeakestTarget());
    }

    private CombatAction ChooseHighIntelligenceAction()
    {
        // Advanced tactics: Multi-turn planning, counter-strategies

        // Analyze party composition and adjust strategy
        var partyAnalysis = AnalyzeEnemyParty();

        // Counter specific threats (e.g., prioritize mages)
        if (partyAnalysis.HasDangerousMage)
        {
            var mage = FindEnemyMage();
            if (mage != null)
                return new AttackAction(mage);
        }

        // Use support abilities to buff allies
        if (HasAlliesNearby() && Random.value < 0.4f)
        {
            var supportAbility = FindSupportAbility();
            if (supportAbility != null)
                return new AbilityAction(supportAbility, FindBestAllyTarget());
        }

        // Fallback to medium intelligence behavior
        return ChooseMediumIntelligenceAction();
    }
}
```

### Encounter Generation System

**Dynamic Encounter Configuration:**

```json
// StreamingAssets/Encounters/EncounterTable.json
{
  "version": "1.0.0",
  "encountersByRegion": {
    "darkForest": {
      "common": [
        {
          "name": "Wolf Pack",
          "weight": 40,
          "enemies": [
            { "id": "direwolf", "count": 1 },
            { "id": "wolf", "count": "2d3" }
          ],
          "environment": "forest",
          "difficulty": "medium"
        },
        {
          "name": "Bandit Ambush",
          "weight": 35,
          "enemies": [
            { "id": "banditRaider", "count": "2d2" },
            { "id": "banditArcher", "count": 1 }
          ],
          "environment": "roadside",
          "difficulty": "medium"
        }
      ],
      "rare": [
        {
          "name": "Ancient Grove Guardian",
          "weight": 5,
          "enemies": [
            { "id": "forestSprite", "count": 2 },
            { "id": "entWarden", "count": 1 }
          ],
          "environment": "ancientGrove",
          "difficulty": "hard"
        }
      ],
      "boss": [
        {
          "name": "The Bandit King's Lair",
          "weight": 1,
          "enemies": [
            { "id": "banditKing", "count": 1 },
            { "id": "veteranSoldier", "count": 2 },
            { "id": "banditRaider", "count": 3 }
          ],
          "environment": "banditCamp",
          "difficulty": "boss"
        }
      ]
    }
  }
}
```

**Encounter Manager:**

```csharp
public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EncounterTable encounterTable;
    [SerializeField] private string currentRegion = "darkForest";
    [SerializeField] private float baseEncounterChance = 0.15f; // 15% chance per movement
    [SerializeField] private float distanceSinceLastEncounter = 0f;

    public bool ShouldTriggerEncounter()
    {
        var difficultySettings = BalanceManager.Instance.GetCurrentDifficultySettings();
        float adjustedChance = baseEncounterChance * difficultySettings.encounterFrequency;
        
        // Increase chance based on distance traveled without encounter
        float scaledChance = adjustedChance + (distanceSinceLastEncounter * 0.02f);
        
        bool shouldTrigger = Random.value < scaledChance;
        
        if (shouldTrigger)
        {
            distanceSinceLastEncounter = 0f;
        }
        else
        {
            distanceSinceLastEncounter += 1f;
        }
        
        return shouldTrigger;
    }

    public Encounter GenerateRandomEncounter()
    {
        var regionEncounters = encounterTable.encountersByRegion[currentRegion];
        var difficultySettings = BalanceManager.Instance.GetCurrentDifficultySettings();

        // Adjust encounter type probabilities based on difficulty
        var roll = Random.Range(0f, 100f);
        string encounterType;
        
        if (BalanceManager.Instance.currentDifficulty == DifficultyLevel.Easy)
        {
            // Easy: More common encounters, fewer bosses
            encounterType = roll switch
            {
                < 85f => "common",
                < 98f => "rare",
                _ => "boss"
            };
        }
        else if (BalanceManager.Instance.currentDifficulty == DifficultyLevel.Hard)
        {
            // Hard: More challenging encounters, more bosses
            encounterType = roll switch
            {
                < 65f => "common",
                < 90f => "rare",
                _ => "boss"
            };
        }
        else
        {
            // Normal: Balanced distribution
            encounterType = roll switch
            {
                < 75f => "common",
                < 95f => "rare",
                _ => "boss"
            };
        }

        var possibleEncounters = regionEncounters[encounterType];
        var selectedEncounter = WeightedRandom.Choose(possibleEncounters);

        return CreateEncounter(selectedEncounter);
    }

    private Encounter CreateEncounter(EncounterData data)
    {
        var encounter = new Encounter();
        encounter.name = data.name;
        encounter.environment = data.environment;
        encounter.difficulty = data.difficulty;

        foreach (var enemyGroup in data.enemies)
        {
            int count = DiceRoller.Roll(enemyGroup.count);
            
            // Scale enemy count based on difficulty
            var difficultySettings = BalanceManager.Instance.GetCurrentDifficultySettings();
            if (difficultySettings.encounterFrequency > 1.1f) // Hard difficulty
            {
                count = Mathf.RoundToInt(count * 1.2f); // 20% more enemies
            }
            else if (difficultySettings.encounterFrequency < 0.9f) // Easy difficulty
            {
                count = Mathf.Max(1, Mathf.RoundToInt(count * 0.8f)); // 20% fewer enemies, minimum 1
            }
            
            for (int i = 0; i < count; i++)
            {
                var enemy = EnemyManager.Instance.SpawnEnemy(
                    enemyGroup.id,
                    GetSpawnPosition()
                );
                encounter.enemies.Add(enemy);
            }
        }

        return encounter;
    }
}
```

## Difficulty System Architecture

### Three-Tier Difficulty Design

**Purpose:** Provide accessible gameplay for all skill levels while maintaining engaging challenges for experienced players.

### Difficulty Level Definitions

**🟢 Easy Mode - "Apprentice":**
- **Target Audience**: New RPG players, casual gamers
- **Player Advantages**: +25% damage dealt, -20% enemy damage received
- **Enemy Scaling**: -10% enemy health, -20% enemy damage
- **Progression**: +20% experience gain, +10% loot quality
- **Quality of Life**: Save on death, no permadeath, reduced encounter frequency
- **Boss Fights**: Phase transitions at 70% and 35% health (gentler curve)

**🟡 Normal Mode - "Adventurer":**
- **Target Audience**: Standard RPG experience
- **Balanced Gameplay**: All multipliers at 1.0 (baseline)
- **Standard Progression**: Normal experience and loot rates
- **Classic Features**: Save on death, no permadeath
- **Boss Fights**: Phase transitions at 65% and 30% health (intended design)

**🔴 Hard Mode - "Legend":**
- **Target Audience**: Experienced players seeking challenge
- **Player Disadvantages**: -15% damage dealt, +30% enemy damage received
- **Enemy Scaling**: +20% enemy health, +30% enemy damage
- **Progression**: -10% experience gain, -5% loot quality
- **Hardcore Features**: No save on death, permadeath enabled, +30% encounter frequency
- **Boss Fights**: Phase transitions at 60% and 25% health (aggressive phases)

### Difficulty Selection System

**Game Start Implementation:**

```csharp
public class DifficultySelectionUI : MonoBehaviour
{
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private TextMeshProUGUI descriptionText;
    
    private void Start()
    {
        SetupDifficultyButtons();
        ShowDifficultyDescription(DifficultyLevel.Normal); // Default selection
    }
    
    private void SetupDifficultyButtons()
    {
        easyButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Easy));
        normalButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Normal));
        hardButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Hard));
    }
    
    private void SelectDifficulty(DifficultyLevel difficulty)
    {
        BalanceManager.Instance.SetDifficulty(difficulty);
        ShowDifficultyDescription(difficulty);
        
        // Save difficulty preference
        PlayerPrefs.SetInt("SelectedDifficulty", (int)difficulty);
        
        // Proceed to character creation
        GameManager.Instance.StartNewGame(difficulty);
    }
    
    private void ShowDifficultyDescription(DifficultyLevel difficulty)
    {
        var settings = BalanceManager.Instance.GetDifficultySettings(difficulty);
        descriptionText.text = settings.description;
        
        // Update visual indicators
        UpdateButtonHighlights(difficulty);
    }
}
```

### Save System Integration

**Updated Save Data Structure:**

```csharp
[System.Serializable]
public class SaveData
{
    public string version = "1.0.0";
    public DateTime saveDate;
    public DifficultyLevel selectedDifficulty;
    public bool permadeathEnabled;
    public GameProgress gameProgress;
    public PartyData partyData;
    public Currency playerCurrency;
    public InventoryData inventory;
    public Dictionary<string, bool> gameFlags;
    public Dictionary<string, int> questProgress;
    public SceneTransitionData currentLocation;
    public DifficultyProgressTracking difficultyStats;
}

[System.Serializable]
public class DifficultyProgressTracking
{
    public int combatsWon;
    public int combatsLost;
    public int charactersLost; // For permadeath tracking
    public float totalPlayTime;
    public int bossesDefeated;
    public bool hasChangedDifficulty; // Prevents achievements on difficulty switching
}
```

### Combat Integration

**Damage Calculation with Difficulty:**

```csharp
public class CombatCalculator
{
    public static int CalculateDamage(ICombatant attacker, ICombatant target, IWeapon weapon)
    {
        // Base damage calculation
        int baseDamage = weapon.BaseDamage + attacker.GetAttributeModifier(AttributeType.Strength);
        
        // Apply weapon type multiplier (includes difficulty scaling)
        float weaponMultiplier = BalanceManager.Instance.GetDamageMultiplier(weapon.Type);
        float finalDamage = baseDamage * weaponMultiplier;
        
        // Apply enemy damage scaling if target is player
        if (target is Character playerCharacter)
        {
            var difficultySettings = BalanceManager.Instance.GetCurrentDifficultySettings();
            finalDamage *= difficultySettings.enemyDamageMultiplier;
        }
        
        int armorReduction = target.GetArmorDefense();
        return Mathf.Max(1, Mathf.RoundToInt(finalDamage) - armorReduction);
    }
}
```

This comprehensive difficulty system ensures that players of all skill levels can enjoy the Land of Mist RPG while providing meaningful choices that affect gameplay, progression, and challenge level throughout the entire experience.
```

### Suggested Enemy Roster

**Humanoid Adversaries:**

1. **Bandits & Outlaws**:
   - Bandit Raider (Level 2): Basic melee fighter
   - Bandit Archer (Level 3): Ranged attacks with poison arrows
   - Bandit Enforcer (Level 4): Heavy armor, intimidation abilities
   - Bandit King (Level 8): Boss with command abilities

2. **Corrupted Guards**:
   - Fallen Soldier (Level 3): Former guards turned to darkness
   - Dark Captain (Level 6): Tactical leader with area buffs
   - Corrupted Paladin (Level 7): Fallen holy warrior with dark magic

3. **Rival Adventurers**:
   - Mercenary Warrior (Level 4): Skilled fighter with multiple weapons
   - Rogue Mage (Level 5): Dangerous spellcaster with forbidden magic
   - Treasure Hunter (Level 3): Uses traps and dirty fighting

**Wilderness Creatures:**

1. **Beasts & Animals**:
   - Wolf (Level 1): Pack hunters with coordination
   - Dire Wolf (Level 3): Alpha predators with leadership abilities
   - Brown Bear (Level 4): Powerful melee attacker with rage
   - Giant Spider (Level 2): Web attacks and poison

2. **Magical Creatures**:
   - Forest Sprite (Level 4): Nature magic and illusions
   - Will-o'-Wisp (Level 3): Ethereal being with confusion abilities
   - Treant Sapling (Level 5): Earth magic and entangle attacks

3. **Elemental Beings**:
   - Fire Salamander (Level 4): Fire damage and burn effects
   - Water Elemental (Level 5): Healing and area attacks
   - Earth Golem (Level 6): High defense, earthquake abilities

**Undead Entities:**

1. **Lesser Undead**:
   - Skeleton Warrior (Level 3): Damage resistance, bone attacks
   - Zombie (Level 2): Slow but persistent, disease attacks
   - Ghost (Level 4): Incorporeal, fear and possession abilities

2. **Greater Undead**:
   - Wraith (Level 6): Life drain and ethereal movement
   - Lich (Level 10): Powerful necromancer boss
   - Death Knight (Level 8): Fallen paladin with dark powers

This architecture ensures the game follows SOLID principles while maintaining the flexibility to add new features and content through data-driven design, with comprehensive JSON-based balancing capabilities.

```

This architecture ensures the game follows SOLID principles while maintaining the flexibility to add new features and content through data-driven design, with comprehensive JSON-based balancing capabilities.
