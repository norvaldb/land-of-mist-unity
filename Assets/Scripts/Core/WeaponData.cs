using UnityEngine;
using LandOfMist.Core;

namespace LandOfMist.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject for weapon data implementing the IWeapon interface
    /// Supports all weapon types including melee, ranged, and magical weapons
    /// Includes poison enhancement system for tactical combat
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Land of Mist/Equipment/Weapon")]
    public class WeaponData : ScriptableObject, IWeapon
    {
        [Header("Weapon Identity")]
        [SerializeField] private string weaponName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        
        [Header("Weapon Properties")]
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private WeaponHandedness handedness;
        [SerializeField] private int baseDamage;
        [SerializeField] private float criticalChance = 0.05f; // 5% default
        [SerializeField] private bool canBeEnhanced = true;
        [SerializeField] private bool hasArmorPenetration = false;
        [SerializeField] private float poisonEffectiveness = 1.0f; // Multiplier for poison effects
        
        [Header("Requirements")]
        [SerializeField] private AttributeRequirements requirements;
        
        [Header("Economic")]
        [SerializeField] private Currency value;
        
        [Header("Runtime Poison State")]
        [SerializeField] private PoisonType appliedPoison = PoisonType.None;
        [SerializeField] private int poisonCharges = 0;
        
        // Interface implementations
        public WeaponType Type => weaponType;
        public WeaponHandedness Handedness => handedness;
        public int BaseDamage => baseDamage;
        public float CriticalChance => criticalChance;
        public bool CanBeEnhanced => canBeEnhanced;
        public bool HasArmorPenetration => hasArmorPenetration;
        public float PoisonEffectiveness => poisonEffectiveness;
        public AttributeRequirements Requirements => requirements;
        public PoisonType AppliedPoison => appliedPoison;
        public int PoisonCharges => poisonCharges;
        
        // Additional properties
        public string WeaponName => weaponName;
        public string Description => description;
        public Sprite Icon => icon;
        public Currency Value => value;
        
        /// <summary>
        /// Check if character can equip this weapon
        /// </summary>
        public bool CanEquip(AttributeData characterAttributes)
        {
            return requirements.MeetsRequirements(characterAttributes, GetCharacterClassFromWeapon());
        }
        
        /// <summary>
        /// Calculate damage output based on attacker's attributes
        /// </summary>
        public int CalculateDamage(AttributeData attackerAttributes)
        {
            int damage = baseDamage;
            
            // Add attribute bonuses based on weapon type
            switch (weaponType)
            {
                case WeaponType.Bow:
                case WeaponType.Crossbow:
                    // Ranged weapons use Dexterity
                    damage += attackerAttributes.DexterityModifier;
                    break;
                
                case WeaponType.Staff:
                    // Magical weapons use Intelligence
                    damage += attackerAttributes.IntelligenceModifier;
                    break;
                
                default:
                    // Melee weapons use Strength
                    damage += attackerAttributes.StrengthModifier;
                    break;
            }
            
            return Mathf.Max(1, damage); // Minimum 1 damage
        }
        
        /// <summary>
        /// Apply poison to the weapon
        /// </summary>
        public void ApplyPoison(PoisonType poison, int charges)
        {
            if (!canBeEnhanced) return;
            
            appliedPoison = poison;
            poisonCharges = charges;
        }
        
        /// <summary>
        /// Remove poison from the weapon
        /// </summary>
        public void RemovePoison()
        {
            appliedPoison = PoisonType.None;
            poisonCharges = 0;
        }
        
        /// <summary>
        /// Use one poison charge (called when attacking)
        /// </summary>
        public bool ConsumePoisonCharge()
        {
            if (appliedPoison == PoisonType.None || poisonCharges <= 0)
                return false;
            
            poisonCharges--;
            if (poisonCharges <= 0)
            {
                appliedPoison = PoisonType.None;
            }
            
            return true;
        }
        
        /// <summary>
        /// Get poison damage based on weapon and poison type
        /// </summary>
        public int GetPoisonDamage()
        {
            if (appliedPoison == PoisonType.None) return 0;
            
            int basePoisonDamage = appliedPoison switch
            {
                PoisonType.Weak => 2,
                PoisonType.Strong => 5,
                PoisonType.Paralysis => 1, // Low damage but has special effect
                PoisonType.Weakness => 1,  // Low damage but reduces enemy damage
                _ => 0
            };
            
            return Mathf.RoundToInt(basePoisonDamage * poisonEffectiveness);
        }
        
        /// <summary>
        /// Determine character class based on weapon type (for requirement checking)
        /// </summary>
        private CharacterClass GetCharacterClassFromWeapon()
        {
            // This is a simplified check - in practice, multiple classes might use the same weapon
            return weaponType switch
            {
                WeaponType.Bow or WeaponType.Crossbow => CharacterClass.Ranger,
                WeaponType.Staff => CharacterClass.Mage,
                WeaponType.Mace => CharacterClass.Cleric,
                _ => CharacterClass.Warrior
            };
        }
        
        /// <summary>
        /// Get weapon range category for combat positioning
        /// </summary>
        public bool IsRangedWeapon()
        {
            return weaponType == WeaponType.Bow || weaponType == WeaponType.Crossbow;
        }
        
        /// <summary>
        /// Get weapon speed modifier for initiative
        /// </summary>
        public float GetSpeedModifier()
        {
            return weaponType switch
            {
                WeaponType.Knife => 1.2f,          // Fast
                WeaponType.Sword => 1.0f,          // Normal
                WeaponType.Bow => 1.1f,            // Fast
                WeaponType.Crossbow => 0.9f,       // Slow
                WeaponType.GreatSword => 0.8f,     // Slow
                WeaponType.GreatAxe => 0.7f,       // Very slow
                WeaponType.Staff => 1.0f,          // Normal
                WeaponType.Mace => 0.9f,           // Slow
                WeaponType.Spear => 1.0f,          // Normal
                WeaponType.Axe => 0.9f,            // Slow
                _ => 1.0f
            };
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Validate weapon data in the Unity Editor
        /// </summary>
        private void OnValidate()
        {
            // Ensure reasonable values
            baseDamage = Mathf.Max(1, baseDamage);
            criticalChance = Mathf.Clamp01(criticalChance);
            poisonEffectiveness = Mathf.Max(0f, poisonEffectiveness);
            poisonCharges = Mathf.Max(0, poisonCharges);
            
            // Ensure weapon name
            if (string.IsNullOrEmpty(weaponName))
            {
                weaponName = weaponType.ToString();
            }
            
            // Set handedness based on weapon type
            switch (weaponType)
            {
                case WeaponType.GreatSword:
                case WeaponType.GreatAxe:
                case WeaponType.Bow:
                case WeaponType.Crossbow:
                case WeaponType.Staff:
                    handedness = WeaponHandedness.TwoHanded;
                    break;
                default:
                    handedness = WeaponHandedness.OneHanded;
                    break;
            }
        }
        #endif
    }
}
