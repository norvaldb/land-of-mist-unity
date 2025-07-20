using UnityEngine;
using LandOfMist.Core;
using LandOfMist.Systems;

namespace LandOfMist.Examples
{
    /// <summary>
    /// Example integration showing how BalanceManager works with ScriptableObject data
    /// Demonstrates data-driven balance configuration in practice
    /// </summary>
    public class BalanceIntegrationExample : MonoBehaviour
    {
        [Header("Example Data")]
        [SerializeField] private CharacterData characterData;
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private SpellData spellData;
        
        private void Start()
        {
            // Demonstrate balance system integration
            DemonstrateBalanceIntegration();
        }
        
        private void DemonstrateBalanceIntegration()
        {
            Debug.Log("=== Land of Mist Balance Integration Example ===");
            
            // Access balance configuration
            var balanceManager = BalanceManager.Instance;
            if (balanceManager?.Config == null)
            {
                Debug.LogError("BalanceManager not initialized!");
                return;
            }
            
            // Show current difficulty and settings
            var currentDifficulty = balanceManager.CurrentDifficulty;
            var difficultySettings = balanceManager.CurrentDifficultySettings;
            
            Debug.Log($"Current Difficulty: {currentDifficulty}");
            Debug.Log($"Player Damage Multiplier: {difficultySettings.PlayerDamageMultiplier}");
            Debug.Log($"Enemy Health Multiplier: {difficultySettings.EnemyHealthMultiplier}");
            
            // Demonstrate combat calculations with balance values
            DemonstrateCombatCalculations();
            
            // Demonstrate progression calculations
            DemonstrateProgressionCalculations();
            
            // Demonstrate economy calculations
            DemonstrateEconomyCalculations();
            
            // Demonstrate magic calculations
            DemonstrateMagicCalculations();
            
            // Show how to change difficulty
            DemonstrateDifficultyChange();
        }
        
        private void DemonstrateCombatCalculations()
        {
            Debug.Log("\n--- Combat Balance Integration ---");
            
            var combatConfig = BalanceManager.Instance.GetCombatConfig();
            var difficultySettings = BalanceManager.Instance.CurrentDifficultySettings;
            
            if (weaponData != null)
            {
                // Calculate damage with balance modifiers
                int baseDamage = weaponData.BaseDamage;
                float balanceMultiplier = combatConfig.BaseDamageMultiplier;
                float difficultyMultiplier = difficultySettings.PlayerDamageMultiplier;
                
                int finalDamage = Mathf.RoundToInt(baseDamage * balanceMultiplier * difficultyMultiplier);
                
                Debug.Log($"Weapon: {weaponData.name}");
                Debug.Log($"Base Damage: {baseDamage}");
                Debug.Log($"Balance Multiplier: {balanceMultiplier}");
                Debug.Log($"Difficulty Multiplier: {difficultyMultiplier}");
                Debug.Log($"Final Damage: {finalDamage}");
                
                // Calculate critical hit chance
                float criticalChance = combatConfig.BaseCriticalChance + weaponData.CriticalChance;
                Debug.Log($"Total Critical Chance: {criticalChance * 100:F1}%");
            }
        }
        
        private void DemonstrateProgressionCalculations()
        {
            Debug.Log("\n--- Progression Balance Integration ---");
            
            var progressionConfig = BalanceManager.Instance.GetProgressionConfig();
            var difficultySettings = BalanceManager.Instance.CurrentDifficultySettings;
            
            if (characterData != null)
            {
                // Calculate experience requirements with difficulty modifiers
                int level = 5; // Example level
                float baseExpRequired = progressionConfig.BaseExperienceRequired * 
                                       Mathf.Pow(progressionConfig.ExperienceGrowthRate, level - 1);
                
                float difficultyModifier = difficultySettings.ExperienceMultiplier;
                int adjustedExpRequired = Mathf.RoundToInt(baseExpRequired / difficultyModifier);
                
                Debug.Log($"Character: {characterData.name}");
                Debug.Log($"Level {level} Base Experience Required: {baseExpRequired:F0}");
                Debug.Log($"Difficulty Modifier: {difficultyModifier}");
                Debug.Log($"Adjusted Experience Required: {adjustedExpRequired}");
                
                // Calculate health/mana per level
                int healthPerLevel = progressionConfig.BaseHealthPerLevel;
                int manaPerLevel = progressionConfig.BaseManaPerLevel;
                
                Debug.Log($"Health per Level: {healthPerLevel}");
                Debug.Log($"Mana per Level: {manaPerLevel}");
            }
        }
        
        private void DemonstrateEconomyCalculations()
        {
            Debug.Log("\n--- Economy Balance Integration ---");
            
            var economyConfig = BalanceManager.Instance.GetEconomyConfig();
            var difficultySettings = BalanceManager.Instance.CurrentDifficultySettings;
            
            // Currency conversion rates
            Debug.Log($"Copper to Silver Rate: {economyConfig.CopperToSilverRate}");
            Debug.Log($"Silver to Gold Rate: {economyConfig.SilverToGoldRate}");
            
            // Price calculations with difficulty
            if (weaponData != null)
            {
                var currency = weaponData.Cost;
                int totalCopperValue = currency.TotalInCopper(economyConfig.CopperToSilverRate, economyConfig.SilverToGoldRate);
                
                float buyMultiplier = economyConfig.BaseBuyPriceMultiplier;
                float sellMultiplier = economyConfig.BaseSellPriceMultiplier;
                float goldMultiplier = difficultySettings.GoldMultiplier;
                
                int buyPrice = Mathf.RoundToInt(totalCopperValue * buyMultiplier / goldMultiplier);
                int sellPrice = Mathf.RoundToInt(totalCopperValue * sellMultiplier / goldMultiplier);
                
                Debug.Log($"Item: {weaponData.name}");
                Debug.Log($"Base Value: {totalCopperValue} copper");
                Debug.Log($"Buy Price: {buyPrice} copper");
                Debug.Log($"Sell Price: {sellPrice} copper");
            }
            
            // Starting gold with difficulty modifier
            int startingGold = Mathf.RoundToInt(economyConfig.StartingGold * difficultySettings.GoldMultiplier);
            Debug.Log($"Starting Gold (difficulty adjusted): {startingGold}");
        }
        
        private void DemonstrateMagicCalculations()
        {
            Debug.Log("\n--- Magic Balance Integration ---");
            
            var magicConfig = BalanceManager.Instance.GetMagicConfig();
            
            if (spellData != null)
            {
                // Calculate spell damage and mana cost
                int baseManaCost = spellData.ManaCost;
                float manaMultiplier = magicConfig.BaseManaMultiplier;
                int levelMultiplier = magicConfig.ManaPerSpellLevel * spellData.SpellLevel;
                
                int totalManaCost = Mathf.RoundToInt((baseManaCost + levelMultiplier) * manaMultiplier);
                
                Debug.Log($"Spell: {spellData.name}");
                Debug.Log($"Base Mana Cost: {baseManaCost}");
                Debug.Log($"Level {spellData.SpellLevel} Additional Cost: {levelMultiplier}");
                Debug.Log($"Final Mana Cost: {totalManaCost}");
                
                // Calculate spell damage scaling
                int baseDamage = spellData.BaseDamage;
                float levelDamageMultiplier = Mathf.Pow(magicConfig.SpellLevelDamageMultiplier, spellData.SpellLevel - 1);
                int scaledDamage = Mathf.RoundToInt(baseDamage * levelDamageMultiplier);
                
                Debug.Log($"Base Damage: {baseDamage}");
                Debug.Log($"Level Damage Multiplier: {levelDamageMultiplier:F2}");
                Debug.Log($"Scaled Damage: {scaledDamage}");
                
                // Show elemental resistance calculations
                float baseResistance = magicConfig.BaseElementalResistance;
                Debug.Log($"Base Elemental Resistance: {baseResistance * 100:F1}%");
                Debug.Log($"Max Elemental Resistance: {magicConfig.MaxElementalResistance * 100:F1}%");
            }
        }
        
        private void DemonstrateDifficultyChange()
        {
            Debug.Log("\n--- Difficulty Change Example ---");
            
            var balanceManager = BalanceManager.Instance;
            
            // Show all difficulty levels
            foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
            {
                balanceManager.SetDifficulty(difficulty);
                var settings = balanceManager.CurrentDifficultySettings;
                
                Debug.Log($"{difficulty} Difficulty:");
                Debug.Log($"  Player Damage: {settings.PlayerDamageMultiplier}x");
                Debug.Log($"  Enemy Damage: {settings.EnemyDamageMultiplier}x");
                Debug.Log($"  Experience Gain: {settings.ExperienceMultiplier}x");
                Debug.Log($"  Gold Gain: {settings.GoldMultiplier}x");
            }
            
            // Reset to Normal
            balanceManager.SetDifficulty(DifficultyLevel.Normal);
        }
        
        [ContextMenu("Run Balance Integration Example")]
        public void RunExample()
        {
            DemonstrateBalanceIntegration();
        }
        
        [ContextMenu("Test Hot Reload")]
        public void TestHotReload()
        {
            Debug.Log("Testing configuration hot-reload...");
            BalanceManager.Instance.ReloadConfiguration();
            Debug.Log("Configuration reloaded. Check console for reload confirmation.");
        }
    }
}
