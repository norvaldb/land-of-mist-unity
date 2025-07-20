using UnityEngine;
using LandOfMist.Core;

namespace LandOfMist.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject for shield data implementing the IShield interface
    /// Supports different shield types with blocking mechanics and resistances
    /// </summary>
    [CreateAssetMenu(fileName = "New Shield", menuName = "Land of Mist/Equipment/Shield")]
    public class ShieldData : ScriptableObject, IShield
    {
        [Header("Shield Identity")]
        [SerializeField] private string shieldName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        
        [Header("Shield Properties")]
        [SerializeField] private ShieldType shieldType;
        [SerializeField] private int defenseBonus;
        [SerializeField] private float blockChance = 0.15f;        // 15% default block chance
        [SerializeField] private bool blocksRangedAttacks = false;
        
        [Header("Elemental Protection")]
        [SerializeField] private ElementalResistance[] resistances;
        
        [Header("Requirements")]
        [SerializeField] private AttributeRequirements requirements;
        
        [Header("Economic")]
        [SerializeField] private Currency value;
        
        [Header("Shield Mechanics")]
        [SerializeField] private float staminaCost = 5f;          // Stamina cost per block
        [SerializeField] private float counterAttackChance = 0f;  // Chance to counter after successful block
        [SerializeField] private int counterAttackDamage = 0;     // Damage dealt on counter
        
        // Interface implementations
        public ShieldType Type => shieldType;
        public int DefenseBonus => defenseBonus;
        public float BlockChance => blockChance;
        public bool BlocksRangedAttacks => blocksRangedAttacks;
        public ElementalResistance[] Resistances => resistances;
        public AttributeRequirements Requirements => requirements;
        
        // Additional properties
        public string ShieldName => shieldName;
        public string Description => description;
        public Sprite Icon => icon;
        public Currency Value => value;
        public float StaminaCost => staminaCost;
        public float CounterAttackChance => counterAttackChance;
        public int CounterAttackDamage => counterAttackDamage;
        
        /// <summary>
        /// Check if character can equip this shield
        /// </summary>
        public bool CanEquip(AttributeData characterAttributes)
        {
            // Check attribute requirements
            if (characterAttributes.Strength < requirements.minimumStrength) return false;
            if (characterAttributes.Dexterity < requirements.minimumDexterity) return false;
            
            return true;
        }
        
        /// <summary>
        /// Get resistance value for a specific element type
        /// </summary>
        public float GetResistance(ElementType elementType)
        {
            if (resistances == null) return 0f;
            
            foreach (var resistance in resistances)
            {
                if (resistance.elementType == elementType)
                {
                    return resistance.resistancePercentage;
                }
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Calculate effective block chance considering character's dexterity
        /// </summary>
        public float GetEffectiveBlockChance(AttributeData characterAttributes)
        {
            float baseChance = blockChance;
            
            // Dexterity improves block chance
            float dexterityBonus = characterAttributes.DexterityModifier * 0.02f; // 2% per dexterity modifier
            
            // Shield type modifier
            float typeModifier = shieldType switch
            {
                ShieldType.Buckler => 1.2f,  // Small shields are easier to maneuver
                ShieldType.Round => 1.0f,    // Standard shields
                ShieldType.Tower => 0.8f,    // Large shields are harder to position quickly
                ShieldType.Magic => 1.1f,    // Magic shields have enhanced effectiveness
                _ => 1.0f
            };
            
            float effectiveChance = (baseChance + dexterityBonus) * typeModifier;
            return Mathf.Clamp01(effectiveChance);
        }
        
        /// <summary>
        /// Calculate effective defense considering character's attributes
        /// </summary>
        public int GetEffectiveDefense(AttributeData characterAttributes)
        {
            int baseDefense = defenseBonus;
            
            // Strength helps with heavier shields
            if (shieldType == ShieldType.Tower)
            {
                baseDefense += characterAttributes.StrengthModifier;
            }
            
            // Dexterity helps with lighter shields
            if (shieldType == ShieldType.Buckler)
            {
                baseDefense += characterAttributes.DexterityModifier;
            }
            
            return Mathf.Max(0, baseDefense);
        }
        
        /// <summary>
        /// Check if shield can block a specific attack type
        /// </summary>
        public bool CanBlockAttack(bool isRangedAttack, bool isMagicalAttack)
        {
            // Magic shields can block magical attacks
            if (isMagicalAttack && shieldType == ShieldType.Magic)
                return true;
            
            // Check ranged attack blocking
            if (isRangedAttack)
                return blocksRangedAttacks;
            
            // All shields can block melee attacks
            return !isMagicalAttack;
        }
        
        /// <summary>
        /// Calculate counter attack chance after successful block
        /// </summary>
        public float GetEffectiveCounterChance(AttributeData characterAttributes)
        {
            if (counterAttackChance <= 0f) return 0f;
            
            // Dexterity improves counter attack chance
            float dexterityBonus = characterAttributes.DexterityModifier * 0.01f; // 1% per dexterity modifier
            
            return Mathf.Clamp01(counterAttackChance + dexterityBonus);
        }
        
        /// <summary>
        /// Calculate counter attack damage
        /// </summary>
        public int GetCounterAttackDamage(AttributeData characterAttributes)
        {
            if (counterAttackDamage <= 0) return 0;
            
            int damage = counterAttackDamage;
            
            // Add strength modifier for shield bash damage
            damage += characterAttributes.StrengthModifier;
            
            return Mathf.Max(1, damage);
        }
        
        /// <summary>
        /// Get shield weight for encumbrance calculations
        /// </summary>
        public float GetShieldWeight()
        {
            return shieldType switch
            {
                ShieldType.Buckler => 1.0f,
                ShieldType.Round => 2.0f,
                ShieldType.Tower => 4.0f,
                ShieldType.Magic => 1.5f,
                _ => 2.0f
            };
        }
        
        /// <summary>
        /// Get movement penalty from shield weight and size
        /// </summary>
        public float GetMovementPenalty()
        {
            return shieldType switch
            {
                ShieldType.Buckler => 0f,     // No penalty
                ShieldType.Round => 0.02f,    // 2% penalty
                ShieldType.Tower => 0.08f,    // 8% penalty
                ShieldType.Magic => 0f,       // No penalty (magical)
                _ => 0.02f
            };
        }
        
        /// <summary>
        /// Check if shield is compatible with specific character classes
        /// </summary>
        public bool IsClassCompatible(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => true,        // Warriors can use all shields
                CharacterClass.Cleric => true,         // Clerics can use all shields
                CharacterClass.Ranger => shieldType != ShieldType.Tower, // Rangers can't use tower shields
                CharacterClass.Mage => shieldType == ShieldType.Magic,   // Mages can only use magic shields
                _ => false
            };
        }
        
        /// <summary>
        /// Calculate durability based on shield type and usage
        /// </summary>
        public float GetDurabilityMultiplier()
        {
            return shieldType switch
            {
                ShieldType.Buckler => 0.9f,  // Less durable due to size
                ShieldType.Round => 1.0f,    // Standard durability
                ShieldType.Tower => 1.2f,    // More durable due to construction
                ShieldType.Magic => 1.3f,    // Magically enhanced durability
                _ => 1.0f
            };
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Validate shield data in the Unity Editor
        /// </summary>
        private void OnValidate()
        {
            // Ensure reasonable values
            defenseBonus = Mathf.Max(0, defenseBonus);
            blockChance = Mathf.Clamp01(blockChance);
            staminaCost = Mathf.Max(0f, staminaCost);
            counterAttackChance = Mathf.Clamp01(counterAttackChance);
            counterAttackDamage = Mathf.Max(0, counterAttackDamage);
            
            // Ensure shield name
            if (string.IsNullOrEmpty(shieldName))
            {
                shieldName = shieldType.ToString() + " Shield";
            }
            
            // Set reasonable defaults based on shield type
            switch (shieldType)
            {
                case ShieldType.Buckler:
                    if (defenseBonus == 0) defenseBonus = 1;
                    if (blockChance < 0.1f) blockChance = 0.18f; // Higher block chance for agility
                    blocksRangedAttacks = false; // Too small for ranged protection
                    break;
                
                case ShieldType.Round:
                    if (defenseBonus == 0) defenseBonus = 2;
                    if (blockChance < 0.1f) blockChance = 0.15f; // Standard block chance
                    if (!blocksRangedAttacks) blocksRangedAttacks = true; // Can block some ranged
                    break;
                
                case ShieldType.Tower:
                    if (defenseBonus == 0) defenseBonus = 4;
                    if (blockChance < 0.1f) blockChance = 0.12f; // Lower block chance due to size
                    blocksRangedAttacks = true; // Excellent ranged protection
                    break;
                
                case ShieldType.Magic:
                    if (defenseBonus == 0) defenseBonus = 3;
                    if (blockChance < 0.1f) blockChance = 0.20f; // Enhanced magical effectiveness
                    blocksRangedAttacks = true; // Magic can deflect projectiles
                    break;
            }
            
            // Validate resistances
            if (resistances != null)
            {
                for (int i = 0; i < resistances.Length; i++)
                {
                    resistances[i] = new ElementalResistance(
                        resistances[i].elementType,
                        Mathf.Clamp01(resistances[i].resistancePercentage)
                    );
                }
            }
        }
        #endif
    }
}
