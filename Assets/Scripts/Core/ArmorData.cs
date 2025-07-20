using UnityEngine;
using LandOfMist.Core;

namespace LandOfMist.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject for armor data implementing the IArmor interface
    /// Supports light, medium, and heavy armor types with different penalties and bonuses
    /// </summary>
    [CreateAssetMenu(fileName = "New Armor", menuName = "Land of Mist/Equipment/Armor")]
    public class ArmorData : ScriptableObject, IArmor
    {
        [Header("Armor Identity")]
        [SerializeField] private string armorName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        
        [Header("Armor Properties")]
        [SerializeField] private ArmorType armorType;
        [SerializeField] private int defenseBonus;
        [SerializeField] private float movementPenalty = 0f;   // 0.0 to 1.0 (0% to 100% speed reduction)
        [SerializeField] private float stealthPenalty = 0f;    // 0.0 to 1.0 (0% to 100% stealth reduction)
        
        [Header("Elemental Protection")]
        [SerializeField] private ElementalResistance[] resistances;
        
        [Header("Requirements")]
        [SerializeField] private AttributeRequirements requirements;
        
        [Header("Economic")]
        [SerializeField] private Currency value;
        
        // Interface implementations
        public ArmorType Type => armorType;
        public int DefenseBonus => defenseBonus;
        public float MovementPenalty => movementPenalty;
        public float StealthPenalty => stealthPenalty;
        public ElementalResistance[] Resistances => resistances;
        public AttributeRequirements Requirements => requirements;
        
        // Additional properties
        public string ArmorName => armorName;
        public string Description => description;
        public Sprite Icon => icon;
        public Currency Value => value;
        
        /// <summary>
        /// Check if character can equip this armor
        /// </summary>
        public bool CanEquip(AttributeData characterAttributes)
        {
            // Check attribute requirements
            if (characterAttributes.Strength < requirements.minimumStrength) return false;
            if (characterAttributes.Constitution < requirements.minimumConstitution) return false;
            
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
        /// Calculate effective defense considering character's constitution
        /// </summary>
        public int GetEffectiveDefense(AttributeData characterAttributes)
        {
            int baseDefense = defenseBonus;
            
            // Constitution bonus applies to all armor types but is more effective with heavier armor
            int constitutionBonus = characterAttributes.ConstitutionModifier;
            
            float constitutionMultiplier = armorType switch
            {
                ArmorType.Light => 0.5f,   // Light armor gets half constitution bonus
                ArmorType.Medium => 0.75f, // Medium armor gets 75% constitution bonus
                ArmorType.Heavy => 1.0f,   // Heavy armor gets full constitution bonus
                _ => 1.0f
            };
            
            int totalDefense = baseDefense + Mathf.RoundToInt(constitutionBonus * constitutionMultiplier);
            return Mathf.Max(0, totalDefense);
        }
        
        /// <summary>
        /// Calculate movement speed after armor penalty
        /// </summary>
        public float GetEffectiveMovementSpeed(float baseSpeed, AttributeData characterAttributes)
        {
            float penalty = movementPenalty;
            
            // High strength can partially offset heavy armor penalties
            if (armorType == ArmorType.Heavy)
            {
                float strengthReduction = characterAttributes.StrengthModifier * 0.02f; // 2% per strength modifier
                penalty = Mathf.Max(0f, penalty - strengthReduction);
            }
            
            return baseSpeed * (1f - penalty);
        }
        
        /// <summary>
        /// Calculate stealth effectiveness after armor penalty
        /// </summary>
        public float GetEffectiveStealthModifier(AttributeData characterAttributes)
        {
            float penalty = stealthPenalty;
            
            // High dexterity can partially offset armor stealth penalties
            float dexterityReduction = characterAttributes.DexterityModifier * 0.02f; // 2% per dexterity modifier
            penalty = Mathf.Max(0f, penalty - dexterityReduction);
            
            return 1f - penalty;
        }
        
        /// <summary>
        /// Get armor weight category for gameplay effects
        /// </summary>
        public float GetArmorWeight()
        {
            return armorType switch
            {
                ArmorType.Light => 1.0f,
                ArmorType.Medium => 2.5f,
                ArmorType.Heavy => 5.0f,
                _ => 1.0f
            };
        }
        
        /// <summary>
        /// Check if this armor is compatible with specific character classes
        /// </summary>
        public bool IsClassCompatible(CharacterClass characterClass)
        {
            return armorType switch
            {
                ArmorType.Light => true, // All classes can wear light armor
                ArmorType.Medium => characterClass != CharacterClass.Mage, // All except Mage
                ArmorType.Heavy => characterClass == CharacterClass.Warrior, // Only Warrior
                _ => true
            };
        }
        
        /// <summary>
        /// Get armor durability multiplier based on type
        /// </summary>
        public float GetDurabilityMultiplier()
        {
            return armorType switch
            {
                ArmorType.Light => 0.8f,   // Less durable
                ArmorType.Medium => 1.0f,  // Standard durability
                ArmorType.Heavy => 1.3f,   // More durable
                _ => 1.0f
            };
        }
        
        /// <summary>
        /// Calculate initiative penalty from armor weight
        /// </summary>
        public float GetInitiativePenalty()
        {
            return armorType switch
            {
                ArmorType.Light => 0f,     // No penalty
                ArmorType.Medium => 0.1f,  // -10% initiative
                ArmorType.Heavy => 0.25f,  // -25% initiative
                _ => 0f
            };
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Validate armor data in the Unity Editor
        /// </summary>
        private void OnValidate()
        {
            // Ensure reasonable values
            defenseBonus = Mathf.Max(0, defenseBonus);
            movementPenalty = Mathf.Clamp01(movementPenalty);
            stealthPenalty = Mathf.Clamp01(stealthPenalty);
            
            // Ensure armor name
            if (string.IsNullOrEmpty(armorName))
            {
                armorName = armorType.ToString() + " Armor";
            }
            
            // Set reasonable defaults based on armor type
            switch (armorType)
            {
                case ArmorType.Light:
                    if (defenseBonus == 0) defenseBonus = 2;
                    if (movementPenalty > 0.1f) movementPenalty = 0f;
                    if (stealthPenalty > 0.1f) stealthPenalty = 0f;
                    break;
                
                case ArmorType.Medium:
                    if (defenseBonus == 0) defenseBonus = 5;
                    if (movementPenalty == 0f) movementPenalty = 0.1f;
                    if (stealthPenalty == 0f) stealthPenalty = 0.15f;
                    break;
                
                case ArmorType.Heavy:
                    if (defenseBonus == 0) defenseBonus = 8;
                    if (movementPenalty < 0.15f) movementPenalty = 0.2f;
                    if (stealthPenalty < 0.2f) stealthPenalty = 0.3f;
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
