using UnityEngine;
using LandOfMist.Core;

namespace LandOfMist.Systems
{
    /// <summary>
    /// Difficulty levels available in the game
    /// </summary>
    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard
    }
    
    /// <summary>
    /// Master configuration class containing all game balance settings
    /// Serializable for JSON loading via Unity's JsonUtility
    /// </summary>
    [System.Serializable]
    public class GameBalanceConfig
    {
        [Header("Configuration Info")]
        public string configVersion = "1.0.0";
        public string lastModified;
        public string description = "Land of Mist RPG Balance Configuration";
        
        [Header("Core Systems")]
        public CombatConfig combat = new CombatConfig();
        public ProgressionConfig progression = new ProgressionConfig();
        public EconomyConfig economy = new EconomyConfig();
        public MagicConfig magic = new MagicConfig();
        public PoisonConfig poison = new PoisonConfig();
        
        [Header("Difficulty Settings")]
        public DifficultySettings easy = new DifficultySettings();
        public DifficultySettings normal = new DifficultySettings();
        public DifficultySettings hard = new DifficultySettings();
        
        /// <summary>
        /// Get difficulty settings for specified level
        /// </summary>
        public DifficultySettings GetDifficultySettings(DifficultyLevel level)
        {
            return level switch
            {
                DifficultyLevel.Easy => easy,
                DifficultyLevel.Normal => normal,
                DifficultyLevel.Hard => hard,
                _ => normal
            };
        }
    }
    
    /// <summary>
    /// Combat system balance configuration
    /// </summary>
    [System.Serializable]
    public struct CombatConfig
    {
        [Header("Base Combat")]
        public float baseCriticalChance;
        public float criticalDamageMultiplier;
        public float baseDodgeChance;
        public float baseBlockChance;
        
        [Header("Weapon Type Multipliers")]
        public float swordDamageMultiplier;
        public float bowDamageMultiplier;
        public float staffDamageMultiplier;
        public float maceDamageMultiplier;
        public float knifeDamageMultiplier;
        public float spearDamageMultiplier;
        public float axeDamageMultiplier;
        public float crossbowDamageMultiplier;
        public float greatSwordDamageMultiplier;
        public float greatAxeDamageMultiplier;
        
        [Header("Armor Effectiveness")]
        public float lightArmorEffectiveness;
        public float mediumArmorEffectiveness;
        public float heavyArmorEffectiveness;
        
        [Header("Initiative")]
        public float baseInitiative;
        public float dexterityInitiativeBonus;
        public float armorInitiativePenalty;
        
        /// <summary>
        /// Get damage multiplier for specific weapon type
        /// </summary>
        public float GetWeaponDamageMultiplier(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Sword => swordDamageMultiplier,
                WeaponType.Bow => bowDamageMultiplier,
                WeaponType.Staff => staffDamageMultiplier,
                WeaponType.Mace => maceDamageMultiplier,
                WeaponType.Knife => knifeDamageMultiplier,
                WeaponType.Spear => spearDamageMultiplier,
                WeaponType.Axe => axeDamageMultiplier,
                WeaponType.Crossbow => crossbowDamageMultiplier,
                WeaponType.GreatSword => greatSwordDamageMultiplier,
                WeaponType.GreatAxe => greatAxeDamageMultiplier,
                _ => 1.0f
            };
        }
        
        /// <summary>
        /// Get armor effectiveness multiplier
        /// </summary>
        public float GetArmorEffectiveness(ArmorType armorType)
        {
            return armorType switch
            {
                ArmorType.Light => lightArmorEffectiveness,
                ArmorType.Medium => mediumArmorEffectiveness,
                ArmorType.Heavy => heavyArmorEffectiveness,
                _ => 1.0f
            };
        }
    }
    
    /// <summary>
    /// Character progression and leveling configuration
    /// </summary>
    [System.Serializable]
    public struct ProgressionConfig
    {
        [Header("Experience")]
        public int baseExperienceRequired;
        public float experienceScaling;
        public int maxLevel;
        
        [Header("Attribute Growth")]
        public float warriorAttributeGrowth;
        public float rangerAttributeGrowth;
        public float mageAttributeGrowth;
        public float clericAttributeGrowth;
        
        [Header("HP/MP Growth")]
        public int baseHPPerLevel;
        public int baseMPPerLevel;
        public float constitutionHPBonus;
        public float intelligenceMPBonus;
        public float wisdomMPBonus;
        
        /// <summary>
        /// Calculate experience required for specific level
        /// </summary>
        public int GetExperienceRequired(int level)
        {
            if (level <= 1) return 0;
            
            float experience = baseExperienceRequired;
            for (int i = 2; i <= level; i++)
            {
                experience *= experienceScaling;
            }
            
            return Mathf.RoundToInt(experience);
        }
        
        /// <summary>
        /// Get attribute growth multiplier for character class
        /// </summary>
        public float GetAttributeGrowthMultiplier(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => warriorAttributeGrowth,
                CharacterClass.Ranger => rangerAttributeGrowth,
                CharacterClass.Mage => mageAttributeGrowth,
                CharacterClass.Cleric => clericAttributeGrowth,
                _ => 1.0f
            };
        }
    }
    
    /// <summary>
    /// Economic system configuration
    /// </summary>
    [System.Serializable]
    public struct EconomyConfig
    {
        [Header("Starting Resources")]
        public int startingCopper;
        public int startingSilver;
        public int startingGold;
        
        [Header("Price Multipliers")]
        public float weaponPriceMultiplier;
        public float armorPriceMultiplier;
        public float shieldPriceMultiplier;
        public float consumablePriceMultiplier;
        public float poisonPriceMultiplier;
        
        [Header("Shop Configuration")]
        public float shopBuyPriceMultiplier;
        public float shopSellPriceMultiplier;
        public float rarityPriceMultiplier;
        
        [Header("Loot")]
        public float baseLootChance;
        public float luckLootBonus;
        public Currency baseCurrencyDrop;
        
        /// <summary>
        /// Get starting currency for new characters
        /// </summary>
        public Currency GetStartingCurrency()
        {
            return new Currency(startingGold, startingSilver, startingCopper);
        }
        
        /// <summary>
        /// Calculate item price with multipliers
        /// </summary>
        public Currency CalculateItemPrice(Currency basePrice, float rarityMultiplier = 1.0f)
        {
            float totalMultiplier = rarityMultiplier * rarityPriceMultiplier;
            int adjustedCopper = Mathf.RoundToInt(basePrice.TotalCopper * totalMultiplier);
            return Currency.FromCopper(adjustedCopper);
        }
    }
    
    /// <summary>
    /// Magic system configuration
    /// </summary>
    [System.Serializable]
    public struct MagicConfig
    {
        [Header("Mana")]
        public float baseManaRegenerationRate;
        public float manaRegenerationMultiplier;
        public float intelligenceManaBonus;
        public float wisdomManaBonus;
        
        [Header("Spell Schools")]
        public float fireSpellPowerMultiplier;
        public float waterSpellPowerMultiplier;
        public float earthSpellPowerMultiplier;
        
        [Header("Spell Costs")]
        public float baseManaScaling;
        public float spellLevelCostMultiplier;
        public float intelligenceCostReduction;
        
        [Header("Critical Magic")]
        public float spellCriticalChance;
        public float spellCriticalMultiplier;
        
        /// <summary>
        /// Get spell power multiplier for magic school
        /// </summary>
        public float GetSpellPowerMultiplier(SpellSchool school)
        {
            return school switch
            {
                SpellSchool.Fire => fireSpellPowerMultiplier,
                SpellSchool.Water => waterSpellPowerMultiplier,
                SpellSchool.Earth => earthSpellPowerMultiplier,
                _ => 1.0f
            };
        }
        
        /// <summary>
        /// Calculate effective mana cost for spell
        /// </summary>
        public int CalculateManaCost(int baseCost, int spellLevel, int intelligence)
        {
            float cost = baseCost * (1f + (spellLevel - 1) * spellLevelCostMultiplier);
            float reduction = intelligence * intelligenceCostReduction;
            cost *= (1f - reduction);
            
            return Mathf.Max(1, Mathf.RoundToInt(cost));
        }
    }
    
    /// <summary>
    /// Poison enhancement system configuration
    /// </summary>
    [System.Serializable]
    public struct PoisonConfig
    {
        [Header("Poison Types")]
        public int weakPoisonDamage;
        public int strongPoisonDamage;
        public int paralysisPoisonDamage;
        public int weaknessPoisonDamage;
        
        [Header("Poison Duration")]
        public int weakPoisonDuration;
        public int strongPoisonDuration;
        public int paralysisPoisonDuration;
        public int weaknessPoisonDuration;
        
        [Header("Poison Chances")]
        public float poisonApplicationChance;
        public float poisonResistanceBase;
        public float constitutionResistanceBonus;
        
        [Header("Charges")]
        public int baseChargesPerApplication;
        public int chargesPerPoisonLevel;
        
        /// <summary>
        /// Get poison damage for specific type
        /// </summary>
        public int GetPoisonDamage(PoisonType poisonType)
        {
            return poisonType switch
            {
                PoisonType.Weak => weakPoisonDamage,
                PoisonType.Strong => strongPoisonDamage,
                PoisonType.Paralysis => paralysisPoisonDamage,
                PoisonType.Weakness => weaknessPoisonDamage,
                _ => 0
            };
        }
        
        /// <summary>
        /// Get poison duration for specific type
        /// </summary>
        public int GetPoisonDuration(PoisonType poisonType)
        {
            return poisonType switch
            {
                PoisonType.Weak => weakPoisonDuration,
                PoisonType.Strong => strongPoisonDuration,
                PoisonType.Paralysis => paralysisPoisonDuration,
                PoisonType.Weakness => weaknessPoisonDuration,
                _ => 0
            };
        }
    }
    
    /// <summary>
    /// Difficulty-specific multipliers and settings
    /// </summary>
    [System.Serializable]
    public struct DifficultySettings
    {
        [Header("Player Modifiers")]
        public float playerDamageMultiplier;
        public float playerHealthMultiplier;
        public float playerManaMultiplier;
        public float playerExperienceMultiplier;
        
        [Header("Enemy Modifiers")]
        public float enemyDamageMultiplier;
        public float enemyHealthMultiplier;
        public float enemyAccuracyMultiplier;
        public float enemyLootMultiplier;
        
        [Header("Economic Modifiers")]
        public float currencyDropMultiplier;
        public float shopPriceMultiplier;
        public float repairCostMultiplier;
        
        [Header("Gameplay")]
        public bool permadeathEnabled;
        public bool autoSaveEnabled;
        public float poisonDamageMultiplier;
        public float magicDamageMultiplier;
        
        /// <summary>
        /// Apply difficulty scaling to base value
        /// </summary>
        public float ApplyPlayerScaling(float baseValue, ScalingType type)
        {
            return type switch
            {
                ScalingType.Damage => baseValue * playerDamageMultiplier,
                ScalingType.Health => baseValue * playerHealthMultiplier,
                ScalingType.Mana => baseValue * playerManaMultiplier,
                ScalingType.Experience => baseValue * playerExperienceMultiplier,
                _ => baseValue
            };
        }
        
        /// <summary>
        /// Apply difficulty scaling to enemy values
        /// </summary>
        public float ApplyEnemyScaling(float baseValue, ScalingType type)
        {
            return type switch
            {
                ScalingType.Damage => baseValue * enemyDamageMultiplier,
                ScalingType.Health => baseValue * enemyHealthMultiplier,
                ScalingType.Accuracy => baseValue * enemyAccuracyMultiplier,
                ScalingType.Loot => baseValue * enemyLootMultiplier,
                _ => baseValue
            };
        }
    }
    
    /// <summary>
    /// Types of scaling that can be applied
    /// </summary>
    public enum ScalingType
    {
        Damage,
        Health,
        Mana,
        Experience,
        Accuracy,
        Loot,
        Currency
    }
}
