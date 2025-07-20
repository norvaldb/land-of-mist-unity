using UnityEngine;
using LandOfMist.Core;

namespace LandOfMist.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject for enemy data including stats, abilities, and loot drops
    /// Supports boss encounters with multiple phases and special abilities
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Land of Mist/Enemies/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [Header("Enemy Identity")]
        [SerializeField] private string enemyName;
        [SerializeField] private string description;
        [SerializeField] private Sprite portrait;
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private bool isBoss = false;
        
        [Header("Base Stats")]
        [SerializeField] private int baseHP;
        [SerializeField] private int baseMP;
        [SerializeField] private AttributeData attributes;
        [SerializeField] private int armorClass; // Natural armor rating
        [SerializeField] private float initiative = 10f;
        
        [Header("Combat Abilities")]
        [SerializeField] private EnemyAttack[] attacks;
        [SerializeField] private EnemyAbility[] specialAbilities;
        [SerializeField] private SpellData[] spells;
        
        [Header("Resistances & Immunities")]
        [SerializeField] private ElementalResistance[] resistances;
        [SerializeField] private StatusEffectImmunity[] immunities;
        
        [Header("Behavior")]
        [SerializeField] private EnemyBehavior behavior;
        [SerializeField] private float aggressionLevel = 0.5f; // 0 = defensive, 1 = very aggressive
        [SerializeField] private string[] behaviorPatterns;
        
        [Header("Boss Mechanics")]
        [SerializeField] private BossPhase[] bossPhases; // For multi-phase boss encounters
        [SerializeField] private bool hasPhasedEncounter = false;
        
        [Header("Loot & Rewards")]
        [SerializeField] private LootDrop[] lootTable;
        [SerializeField] private int experienceReward;
        [SerializeField] private Currency currencyDrop;
        [SerializeField] private float lootDropChance = 0.3f; // 30% chance per item
        
        [Header("Scaling")]
        [SerializeField] private float difficultyMultiplier = 1.0f;
        [SerializeField] private bool scalesWithPartyLevel = true;
        
        // Public properties
        public string EnemyName => enemyName;
        public string Description => description;
        public Sprite Portrait => portrait;
        public EnemyType Type => enemyType;
        public bool IsBoss => isBoss;
        public int BaseHP => baseHP;
        public int BaseMP => baseMP;
        public AttributeData Attributes => attributes;
        public int ArmorClass => armorClass;
        public float Initiative => initiative;
        public EnemyAttack[] Attacks => attacks;
        public EnemyAbility[] SpecialAbilities => specialAbilities;
        public SpellData[] Spells => spells;
        public ElementalResistance[] Resistances => resistances;
        public StatusEffectImmunity[] Immunities => immunities;
        public EnemyBehavior Behavior => behavior;
        public float AggressionLevel => aggressionLevel;
        public string[] BehaviorPatterns => behaviorPatterns;
        public BossPhase[] BossPhases => bossPhases;
        public bool HasPhasedEncounter => hasPhasedEncounter;
        public LootDrop[] LootTable => lootTable;
        public int ExperienceReward => experienceReward;
        public Currency CurrencyDrop => currencyDrop;
        public float LootDropChance => lootDropChance;
        public float DifficultyMultiplier => difficultyMultiplier;
        public bool ScalesWithPartyLevel => scalesWithPartyLevel;
        
        /// <summary>
        /// Get scaled stats based on party level and difficulty
        /// </summary>
        public EnemyStats GetScaledStats(int partyLevel, float difficultyScale = 1.0f)
        {
            float levelScale = scalesWithPartyLevel ? (1f + (partyLevel - 1) * 0.15f) : 1f; // 15% per level above 1
            float totalScale = levelScale * difficultyMultiplier * difficultyScale;
            
            return new EnemyStats
            {
                HP = Mathf.RoundToInt(baseHP * totalScale),
                MP = Mathf.RoundToInt(baseMP * totalScale),
                ScaledAttributes = ScaleAttributes(attributes, totalScale),
                AC = armorClass + Mathf.RoundToInt((partyLevel - 1) * 0.5f), // +0.5 AC per level
                Initiative = initiative * Mathf.Sqrt(totalScale), // Square root scaling for initiative
                ExperienceReward = Mathf.RoundToInt(experienceReward * totalScale),
                CurrencyDrop = new Currency(
                    Mathf.RoundToInt(currencyDrop.Gold * totalScale),
                    Mathf.RoundToInt(currencyDrop.Silver * totalScale),
                    Mathf.RoundToInt(currencyDrop.Copper * totalScale)
                )
            };
        }
        
        /// <summary>
        /// Scale enemy attributes
        /// </summary>
        private AttributeData ScaleAttributes(AttributeData baseAttribs, float scale)
        {
            return new AttributeData(
                Mathf.RoundToInt(baseAttribs.Strength * scale),
                Mathf.RoundToInt(baseAttribs.Dexterity * scale),
                Mathf.RoundToInt(baseAttribs.Constitution * scale),
                Mathf.RoundToInt(baseAttribs.Intelligence * scale),
                Mathf.RoundToInt(baseAttribs.Wisdom * scale),
                Mathf.RoundToInt(baseAttribs.Charisma * scale)
            );
        }
        
        /// <summary>
        /// Get resistance value for specific element
        /// </summary>
        public float GetResistance(ElementType elementType)
        {
            if (resistances == null) return 0f;
            
            foreach (var resistance in resistances)
            {
                if (resistance.elementType == elementType)
                    return resistance.resistancePercentage;
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Check if enemy is immune to status effect
        /// </summary>
        public bool IsImmuneToStatus(StatusEffectType statusType)
        {
            if (immunities == null) return false;
            
            foreach (var immunity in immunities)
            {
                if (immunity.statusType == statusType)
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Get current boss phase based on HP percentage
        /// </summary>
        public BossPhase GetCurrentPhase(float currentHPPercentage)
        {
            if (!isBoss || !hasPhasedEncounter || bossPhases == null)
                return null;
            
            // Find the appropriate phase based on HP
            foreach (var phase in bossPhases)
            {
                if (currentHPPercentage >= phase.hpThreshold)
                    return phase;
            }
            
            // Return last phase if no threshold met
            return bossPhases[bossPhases.Length - 1];
        }
        
        /// <summary>
        /// Generate loot drops based on luck and drop chances
        /// </summary>
        public LootResult[] GenerateLoot(float luckModifier = 1.0f)
        {
            if (lootTable == null) return new LootResult[0];
            
            var results = new System.Collections.Generic.List<LootResult>();
            
            foreach (var drop in lootTable)
            {
                float adjustedChance = drop.dropChance * luckModifier;
                
                if (Random.value < adjustedChance)
                {
                    int quantity = Random.Range(drop.minQuantity, drop.maxQuantity + 1);
                    results.Add(new LootResult
                    {
                        item = drop.item,
                        quantity = quantity
                    });
                }
            }
            
            return results.ToArray();
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Validate enemy data in Unity Editor
        /// </summary>
        private void OnValidate()
        {
            baseHP = Mathf.Max(1, baseHP);
            baseMP = Mathf.Max(0, baseMP);
            armorClass = Mathf.Max(0, armorClass);
            initiative = Mathf.Max(0.1f, initiative);
            aggressionLevel = Mathf.Clamp01(aggressionLevel);
            experienceReward = Mathf.Max(0, experienceReward);
            lootDropChance = Mathf.Clamp01(lootDropChance);
            difficultyMultiplier = Mathf.Max(0.1f, difficultyMultiplier);
            
            if (string.IsNullOrEmpty(enemyName))
            {
                enemyName = enemyType.ToString();
            }
            
            // Validate boss phases
            if (bossPhases != null && bossPhases.Length > 0)
            {
                hasPhasedEncounter = true;
                
                // Sort phases by HP threshold (highest first)
                System.Array.Sort(bossPhases, (a, b) => b.hpThreshold.CompareTo(a.hpThreshold));
            }
        }
        #endif
    }
    
    /// <summary>
    /// Enemy type categories
    /// </summary>
    public enum EnemyType
    {
        Minion,     // Weak enemies, appear in groups
        Elite,      // Stronger individual enemies
        Boss,       // Major encounters
        Champion    // Mid-tier boss encounters
    }
    
    /// <summary>
    /// Enemy behavior patterns
    /// </summary>
    public enum EnemyBehavior
    {
        Aggressive,     // Always attacks, prioritizes damage
        Defensive,      // Uses defensive abilities, careful positioning
        Tactical,       // Uses abilities strategically
        Berserker,      // Becomes more aggressive when wounded
        Cowardly,       // Attempts to flee when low on health
        Supportive      // Focuses on buffing allies and debuffing enemies
    }
    
    /// <summary>
    /// Status effect immunity types
    /// </summary>
    public enum StatusEffectType
    {
        Poison,
        Paralysis,
        Sleep,
        Charm,
        Fear,
        Weakness,
        Burning,
        Frozen
    }
    
    /// <summary>
    /// Enemy attack data
    /// </summary>
    [System.Serializable]
    public struct EnemyAttack
    {
        public string attackName;
        public int baseDamage;
        public ElementType damageType;
        public float accuracy;
        public float criticalChance;
        public StatusEffectType statusEffect;
        public float statusChance;
        public string description;
    }
    
    /// <summary>
    /// Enemy special ability data
    /// </summary>
    [System.Serializable]
    public struct EnemyAbility
    {
        public string abilityName;
        public string description;
        public int manaCost;
        public int cooldownTurns;
        public TargetType targetType;
        public EffectData[] effects;
    }
    
    /// <summary>
    /// Status effect immunity data
    /// </summary>
    [System.Serializable]
    public struct StatusEffectImmunity
    {
        public StatusEffectType statusType;
        public bool isComplete; // True = immune, False = resistant (50% chance)
    }
    
    /// <summary>
    /// Boss phase data for multi-phase encounters
    /// </summary>
    [System.Serializable]
    public struct BossPhase
    {
        public string phaseName;
        public float hpThreshold; // HP percentage to trigger this phase
        public EnemyAbility[] phaseAbilities;
        public string[] newBehaviorPatterns;
        public float damageMultiplier;
        public bool triggersOnce; // If true, abilities only trigger once when entering phase
    }
    
    /// <summary>
    /// Loot drop configuration
    /// </summary>
    [System.Serializable]
    public struct LootDrop
    {
        public ScriptableObject item; // Can be weapon, armor, shield, consumable, etc.
        public float dropChance;
        public int minQuantity;
        public int maxQuantity;
    }
    
    /// <summary>
    /// Scaled enemy stats result
    /// </summary>
    public struct EnemyStats
    {
        public int HP;
        public int MP;
        public AttributeData ScaledAttributes;
        public int AC;
        public float Initiative;
        public int ExperienceReward;
        public Currency CurrencyDrop;
    }
    
    /// <summary>
    /// Loot generation result
    /// </summary>
    public struct LootResult
    {
        public ScriptableObject item;
        public int quantity;
    }
}
