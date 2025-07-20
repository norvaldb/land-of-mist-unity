using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LandOfMist.Systems;
using LandOfMist.Core;
using LandOfMist.Utils;
using System.IO;

namespace LandOfMist.Tests.EditMode
{
    /// <summary>
    /// Unit tests for the BalanceManager system
    /// Tests JSON configuration loading, validation, and difficulty management
    /// </summary>
    public class BalanceManagerTests
    {
        private BalanceManager _balanceManager;
        private GameObject _testGameObject;
        private string _testConfigPath;
        private string _originalConfig;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject with BalanceManager
            _testGameObject = new GameObject("TestBalanceManager");
            _balanceManager = _testGameObject.AddComponent<BalanceManager>();
            
            // Setup test configuration path
            _testConfigPath = Path.Combine(Application.streamingAssetsPath, "GameBalance", "TestConfig.json");
            
            // Ensure directory exists
            string directory = Path.GetDirectoryName(_testConfigPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Store original config if it exists
            if (File.Exists(_testConfigPath))
            {
                _originalConfig = File.ReadAllText(_testConfigPath);
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Restore original config
            if (!string.IsNullOrEmpty(_originalConfig))
            {
                File.WriteAllText(_testConfigPath, _originalConfig);
            }
            else if (File.Exists(_testConfigPath))
            {
                File.Delete(_testConfigPath);
            }
            
            // Clean up test objects
            if (_testGameObject != null)
            {
                Object.DestroyImmediate(_testGameObject);
            }
        }

        [Test]
        public void BalanceManager_ShouldCreateSingleton()
        {
            // Test singleton pattern
            var instance1 = BalanceManager.Instance;
            var instance2 = BalanceManager.Instance;
            
            Assert.IsNotNull(instance1);
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void BalanceManager_ShouldLoadDefaultConfiguration()
        {
            // When no config file exists, should create default
            var config = _balanceManager.Config;
            
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.CombatConfig);
            Assert.IsNotNull(config.ProgressionConfig);
            Assert.IsNotNull(config.EconomyConfig);
            Assert.IsNotNull(config.MagicConfig);
            Assert.IsNotNull(config.PoisonConfig);
            Assert.IsNotNull(config.EasySettings);
            Assert.IsNotNull(config.NormalSettings);
            Assert.IsNotNull(config.HardSettings);
        }

        [Test]
        public void BalanceManager_ShouldValidateConfiguration()
        {
            var config = CreateValidTestConfig();
            bool isValid = ConfigurationValidator.ValidateConfiguration(config);
            
            Assert.IsTrue(isValid);
        }

        [Test]
        public void BalanceManager_ShouldRejectInvalidConfiguration()
        {
            var config = CreateInvalidTestConfig();
            bool isValid = ConfigurationValidator.ValidateConfiguration(config);
            
            Assert.IsFalse(isValid);
        }

        [Test]
        public void BalanceManager_ShouldChangeDifficulty()
        {
            var originalDifficulty = _balanceManager.CurrentDifficulty;
            var newDifficulty = DifficultyLevel.Hard;
            
            _balanceManager.SetDifficulty(newDifficulty);
            
            Assert.AreEqual(newDifficulty, _balanceManager.CurrentDifficulty);
            Assert.AreNotEqual(originalDifficulty, _balanceManager.CurrentDifficulty);
        }

        [Test]
        public void BalanceManager_ShouldReturnCorrectDifficultySettings()
        {
            _balanceManager.SetDifficulty(DifficultyLevel.Easy);
            var easySettings = _balanceManager.CurrentDifficultySettings;
            
            _balanceManager.SetDifficulty(DifficultyLevel.Hard);
            var hardSettings = _balanceManager.CurrentDifficultySettings;
            
            Assert.IsNotNull(easySettings);
            Assert.IsNotNull(hardSettings);
            Assert.AreNotSame(easySettings, hardSettings);
            
            // Easy should have higher player multipliers
            Assert.Greater(easySettings.PlayerDamageMultiplier, hardSettings.PlayerDamageMultiplier);
            Assert.Greater(easySettings.PlayerHealthMultiplier, hardSettings.PlayerHealthMultiplier);
        }

        [Test]
        public void BalanceManager_ShouldProvideConfigurationGetters()
        {
            var combatConfig = _balanceManager.GetCombatConfig();
            var progressionConfig = _balanceManager.GetProgressionConfig();
            var economyConfig = _balanceManager.GetEconomyConfig();
            var magicConfig = _balanceManager.GetMagicConfig();
            var poisonConfig = _balanceManager.GetPoisonConfig();
            
            Assert.IsNotNull(combatConfig);
            Assert.IsNotNull(progressionConfig);
            Assert.IsNotNull(economyConfig);
            Assert.IsNotNull(magicConfig);
            Assert.IsNotNull(poisonConfig);
        }

        [Test]
        public void ConfigurationValidator_ShouldValidateCombatConfig()
        {
            var config = new GameBalanceConfig
            {
                CombatConfig = new CombatConfig
                {
                    BaseCriticalChance = 0.05f,
                    CriticalHitDamageMultiplier = 1.5f,
                    ArmorDamageReduction = 0.02f,
                    MaxArmorReduction = 0.8f,
                    BaseDamageMultiplier = 1.0f,
                    StrengthDamageBonus = 0.2f,
                    BaseStatusEffectDuration = 3
                }
            };
            
            bool isValid = ConfigurationValidator.ValidateConfiguration(config);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void ConfigurationValidator_ShouldRejectInvalidCriticalChance()
        {
            var config = new GameBalanceConfig
            {
                CombatConfig = new CombatConfig
                {
                    BaseCriticalChance = 1.5f, // Invalid: > 1.0
                    CriticalHitDamageMultiplier = 1.5f,
                    ArmorDamageReduction = 0.02f,
                    MaxArmorReduction = 0.8f,
                    BaseDamageMultiplier = 1.0f,
                    StrengthDamageBonus = 0.2f,
                    BaseStatusEffectDuration = 3
                }
            };
            
            bool isValid = ConfigurationValidator.ValidateConfiguration(config);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void ConfigurationValidator_ShouldRejectNegativeValues()
        {
            var config = new GameBalanceConfig
            {
                ProgressionConfig = new ProgressionConfig
                {
                    BaseExperienceRequired = -100, // Invalid: negative
                    ExperienceGrowthRate = 1.2f,
                    MaxLevel = 50,
                    AttributePointsPerLevel = 1,
                    BaseHealthPerLevel = 10,
                    BaseManaPerLevel = 5
                }
            };
            
            bool isValid = ConfigurationValidator.ValidateConfiguration(config);
            Assert.IsFalse(isValid);
        }

        // Helper methods
        private GameBalanceConfig CreateValidTestConfig()
        {
            return new GameBalanceConfig
            {
                ConfigVersion = "1.0.0",
                LastModified = System.DateTime.Now.ToString(),
                Description = "Test Configuration",
                CombatConfig = new CombatConfig
                {
                    BaseCriticalChance = 0.05f,
                    CriticalHitDamageMultiplier = 1.5f,
                    ArmorDamageReduction = 0.02f,
                    MaxArmorReduction = 0.8f,
                    BaseDamageMultiplier = 1.0f,
                    StrengthDamageBonus = 0.2f,
                    BaseStatusEffectDuration = 3
                },
                ProgressionConfig = new ProgressionConfig
                {
                    BaseExperienceRequired = 100,
                    ExperienceGrowthRate = 1.2f,
                    MaxLevel = 50,
                    AttributePointsPerLevel = 1,
                    BaseHealthPerLevel = 10,
                    BaseManaPerLevel = 5
                },
                EconomyConfig = new EconomyConfig
                {
                    CopperToSilverRate = 100,
                    SilverToGoldRate = 100,
                    BaseBuyPriceMultiplier = 1.0f,
                    BaseSellPriceMultiplier = 0.5f,
                    StartingGold = 100
                },
                MagicConfig = new MagicConfig
                {
                    BaseManaMultiplier = 1.0f,
                    ManaPerSpellLevel = 5,
                    IntelligenceDamageBonus = 0.3f,
                    SpellLevelDamageMultiplier = 1.2f,
                    BaseElementalResistance = 0.1f,
                    ResistancePerLevel = 0.01f,
                    MaxElementalResistance = 0.9f
                },
                PoisonConfig = new PoisonConfig
                {
                    BasePoisonDamage = 5,
                    BasePoisonDuration = 5,
                    PoisonTickInterval = 1.0f,
                    EnhancementCostCopper = 50,
                    EnhancementCostSilver = 25,
                    EnhancementCostGold = 10
                },
                NormalSettings = new DifficultySettings
                {
                    PlayerDamageMultiplier = 1.0f,
                    EnemyDamageMultiplier = 1.0f,
                    PlayerHealthMultiplier = 1.0f,
                    EnemyHealthMultiplier = 1.0f,
                    ExperienceMultiplier = 1.0f,
                    GoldMultiplier = 1.0f,
                    LootChanceMultiplier = 1.0f
                }
            };
        }

        private GameBalanceConfig CreateInvalidTestConfig()
        {
            return new GameBalanceConfig
            {
                ConfigVersion = "1.0.0",
                CombatConfig = new CombatConfig
                {
                    BaseCriticalChance = -0.5f, // Invalid: negative
                    CriticalHitDamageMultiplier = 0.5f, // Invalid: <= 1.0
                    ArmorDamageReduction = 1.5f, // Invalid: > 1.0
                    MaxArmorReduction = -0.2f, // Invalid: negative
                    BaseDamageMultiplier = -1.0f, // Invalid: negative
                    StrengthDamageBonus = -0.2f, // Invalid: negative
                    BaseStatusEffectDuration = -3 // Invalid: negative
                }
            };
        }
    }
}
