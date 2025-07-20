using UnityEngine;
using LandOfMist.Core;

namespace LandOfMist.Utils
{
    /// <summary>
    /// Validates game configuration data for consistency and correctness
    /// Ensures all balance values are within acceptable ranges
    /// </summary>
    public static class ConfigurationValidator
    {
        /// <summary>
        /// Validates a complete game balance configuration
        /// </summary>
        /// <param name="config">Configuration to validate</param>
        /// <returns>True if configuration is valid, false otherwise</returns>
        public static bool ValidateConfiguration(GameBalanceConfig config)
        {
            if (config == null)
            {
                Debug.LogError("Configuration is null");
                return false;
            }

            bool isValid = true;

            // Validate Combat Configuration
            isValid &= ValidateCombatConfig(config.CombatConfig);

            // Validate Progression Configuration
            isValid &= ValidateProgressionConfig(config.ProgressionConfig);

            // Validate Economy Configuration
            isValid &= ValidateEconomyConfig(config.EconomyConfig);

            // Validate Magic Configuration
            isValid &= ValidateMagicConfig(config.MagicConfig);

            // Validate Poison Configuration
            isValid &= ValidatePoisonConfig(config.PoisonConfig);

            // Validate Difficulty Settings
            isValid &= ValidateDifficultySettings(config.EasySettings);
            isValid &= ValidateDifficultySettings(config.NormalSettings);
            isValid &= ValidateDifficultySettings(config.HardSettings);

            return isValid;
        }

        private static bool ValidateCombatConfig(CombatConfig combat)
        {
            if (combat == null)
            {
                Debug.LogError("Combat configuration is null");
                return false;
            }

            bool isValid = true;

            // Base Damage Multipliers
            if (combat.BaseDamageMultiplier <= 0)
            {
                Debug.LogError($"Invalid BaseDamageMultiplier: {combat.BaseDamageMultiplier}");
                isValid = false;
            }

            if (combat.StrengthDamageBonus < 0)
            {
                Debug.LogError($"Invalid StrengthDamageBonus: {combat.StrengthDamageBonus}");
                isValid = false;
            }

            // Critical Hit Settings
            if (combat.CriticalHitDamageMultiplier <= 1.0f)
            {
                Debug.LogWarning($"CriticalHitDamageMultiplier should be > 1.0: {combat.CriticalHitDamageMultiplier}");
            }

            if (combat.BaseCriticalChance < 0 || combat.BaseCriticalChance > 1.0f)
            {
                Debug.LogError($"Invalid BaseCriticalChance: {combat.BaseCriticalChance}");
                isValid = false;
            }

            // Armor Settings
            if (combat.ArmorDamageReduction < 0 || combat.ArmorDamageReduction > 1.0f)
            {
                Debug.LogError($"Invalid ArmorDamageReduction: {combat.ArmorDamageReduction}");
                isValid = false;
            }

            if (combat.MaxArmorReduction <= 0 || combat.MaxArmorReduction > 1.0f)
            {
                Debug.LogError($"Invalid MaxArmorReduction: {combat.MaxArmorReduction}");
                isValid = false;
            }

            // Shield Settings
            if (combat.BaseBlockChance < 0 || combat.BaseBlockChance > 1.0f)
            {
                Debug.LogError($"Invalid BaseBlockChance: {combat.BaseBlockChance}");
                isValid = false;
            }

            if (combat.BlockDamageReduction < 0 || combat.BlockDamageReduction > 1.0f)
            {
                Debug.LogError($"Invalid BlockDamageReduction: {combat.BlockDamageReduction}");
                isValid = false;
            }

            // Status Effect Duration
            if (combat.BaseStatusEffectDuration <= 0)
            {
                Debug.LogError($"Invalid BaseStatusEffectDuration: {combat.BaseStatusEffectDuration}");
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateProgressionConfig(ProgressionConfig progression)
        {
            if (progression == null)
            {
                Debug.LogError("Progression configuration is null");
                return false;
            }

            bool isValid = true;

            if (progression.BaseExperienceRequired <= 0)
            {
                Debug.LogError($"Invalid BaseExperienceRequired: {progression.BaseExperienceRequired}");
                isValid = false;
            }

            if (progression.ExperienceGrowthRate <= 1.0f)
            {
                Debug.LogError($"Invalid ExperienceGrowthRate: {progression.ExperienceGrowthRate}");
                isValid = false;
            }

            if (progression.MaxLevel <= 1)
            {
                Debug.LogError($"Invalid MaxLevel: {progression.MaxLevel}");
                isValid = false;
            }

            if (progression.AttributePointsPerLevel <= 0)
            {
                Debug.LogError($"Invalid AttributePointsPerLevel: {progression.AttributePointsPerLevel}");
                isValid = false;
            }

            if (progression.BaseHealthPerLevel <= 0)
            {
                Debug.LogError($"Invalid BaseHealthPerLevel: {progression.BaseHealthPerLevel}");
                isValid = false;
            }

            if (progression.BaseManaPerLevel <= 0)
            {
                Debug.LogError($"Invalid BaseManaPerLevel: {progression.BaseManaPerLevel}");
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateEconomyConfig(EconomyConfig economy)
        {
            if (economy == null)
            {
                Debug.LogError("Economy configuration is null");
                return false;
            }

            bool isValid = true;

            // Currency Conversion
            if (economy.CopperToSilverRate <= 0)
            {
                Debug.LogError($"Invalid CopperToSilverRate: {economy.CopperToSilverRate}");
                isValid = false;
            }

            if (economy.SilverToGoldRate <= 0)
            {
                Debug.LogError($"Invalid SilverToGoldRate: {economy.SilverToGoldRate}");
                isValid = false;
            }

            // Shop Settings
            if (economy.BaseBuyPriceMultiplier <= 0)
            {
                Debug.LogError($"Invalid BaseBuyPriceMultiplier: {economy.BaseBuyPriceMultiplier}");
                isValid = false;
            }

            if (economy.BaseSellPriceMultiplier <= 0 || economy.BaseSellPriceMultiplier > economy.BaseBuyPriceMultiplier)
            {
                Debug.LogError($"Invalid BaseSellPriceMultiplier: {economy.BaseSellPriceMultiplier}");
                isValid = false;
            }

            // Starting Gold
            if (economy.StartingGold < 0)
            {
                Debug.LogError($"Invalid StartingGold: {economy.StartingGold}");
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateMagicConfig(MagicConfig magic)
        {
            if (magic == null)
            {
                Debug.LogError("Magic configuration is null");
                return false;
            }

            bool isValid = true;

            // Mana Costs
            if (magic.BaseManaMultiplier <= 0)
            {
                Debug.LogError($"Invalid BaseManaMultiplier: {magic.BaseManaMultiplier}");
                isValid = false;
            }

            if (magic.ManaPerSpellLevel <= 0)
            {
                Debug.LogError($"Invalid ManaPerSpellLevel: {magic.ManaPerSpellLevel}");
                isValid = false;
            }

            // Damage Scaling
            if (magic.IntelligenceDamageBonus < 0)
            {
                Debug.LogError($"Invalid IntelligenceDamageBonus: {magic.IntelligenceDamageBonus}");
                isValid = false;
            }

            if (magic.SpellLevelDamageMultiplier <= 0)
            {
                Debug.LogError($"Invalid SpellLevelDamageMultiplier: {magic.SpellLevelDamageMultiplier}");
                isValid = false;
            }

            // Resistances
            if (magic.BaseElementalResistance < 0 || magic.BaseElementalResistance > 1.0f)
            {
                Debug.LogError($"Invalid BaseElementalResistance: {magic.BaseElementalResistance}");
                isValid = false;
            }

            if (magic.ResistancePerLevel < 0)
            {
                Debug.LogError($"Invalid ResistancePerLevel: {magic.ResistancePerLevel}");
                isValid = false;
            }

            if (magic.MaxElementalResistance < 0 || magic.MaxElementalResistance > 1.0f)
            {
                Debug.LogError($"Invalid MaxElementalResistance: {magic.MaxElementalResistance}");
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidatePoisonConfig(PoisonConfig poison)
        {
            if (poison == null)
            {
                Debug.LogError("Poison configuration is null");
                return false;
            }

            bool isValid = true;

            // Base Settings
            if (poison.BasePoisonDamage <= 0)
            {
                Debug.LogError($"Invalid BasePoisonDamage: {poison.BasePoisonDamage}");
                isValid = false;
            }

            if (poison.BasePoisonDuration <= 0)
            {
                Debug.LogError($"Invalid BasePoisonDuration: {poison.BasePoisonDuration}");
                isValid = false;
            }

            if (poison.PoisonTickInterval <= 0)
            {
                Debug.LogError($"Invalid PoisonTickInterval: {poison.PoisonTickInterval}");
                isValid = false;
            }

            // Enhancement Costs
            if (poison.EnhancementCostCopper < 0)
            {
                Debug.LogError($"Invalid EnhancementCostCopper: {poison.EnhancementCostCopper}");
                isValid = false;
            }

            if (poison.EnhancementCostSilver < 0)
            {
                Debug.LogError($"Invalid EnhancementCostSilver: {poison.EnhancementCostSilver}");
                isValid = false;
            }

            if (poison.EnhancementCostGold < 0)
            {
                Debug.LogError($"Invalid EnhancementCostGold: {poison.EnhancementCostGold}");
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateDifficultySettings(DifficultySettings difficulty)
        {
            if (difficulty == null)
            {
                Debug.LogError("Difficulty settings are null");
                return false;
            }

            bool isValid = true;

            // Damage Multipliers
            if (difficulty.PlayerDamageMultiplier <= 0)
            {
                Debug.LogError($"Invalid PlayerDamageMultiplier: {difficulty.PlayerDamageMultiplier}");
                isValid = false;
            }

            if (difficulty.EnemyDamageMultiplier <= 0)
            {
                Debug.LogError($"Invalid EnemyDamageMultiplier: {difficulty.EnemyDamageMultiplier}");
                isValid = false;
            }

            // Health Multipliers
            if (difficulty.PlayerHealthMultiplier <= 0)
            {
                Debug.LogError($"Invalid PlayerHealthMultiplier: {difficulty.PlayerHealthMultiplier}");
                isValid = false;
            }

            if (difficulty.EnemyHealthMultiplier <= 0)
            {
                Debug.LogError($"Invalid EnemyHealthMultiplier: {difficulty.EnemyHealthMultiplier}");
                isValid = false;
            }

            // Experience and Gold
            if (difficulty.ExperienceMultiplier <= 0)
            {
                Debug.LogError($"Invalid ExperienceMultiplier: {difficulty.ExperienceMultiplier}");
                isValid = false;
            }

            if (difficulty.GoldMultiplier <= 0)
            {
                Debug.LogError($"Invalid GoldMultiplier: {difficulty.GoldMultiplier}");
                isValid = false;
            }

            // Loot Chance
            if (difficulty.LootChanceMultiplier < 0)
            {
                Debug.LogError($"Invalid LootChanceMultiplier: {difficulty.LootChanceMultiplier}");
                isValid = false;
            }

            return isValid;
        }
    }
}
