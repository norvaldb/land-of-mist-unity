using UnityEngine;
using LandOfMist.Core;

namespace LandOfMist.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject containing character class data and progression information
    /// Represents the four main character classes: Theron (Warrior), Sylvain (Ranger), 
    /// Valdris (Mage), and Caelum (Cleric)
    /// </summary>
    [CreateAssetMenu(fileName = "New Character", menuName = "Land of Mist/Character")]
    public class CharacterData : ScriptableObject
    {
        [Header("Character Identity")]
        [SerializeField] private string characterName;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private string description;
        [SerializeField] private Sprite portrait;
        
        [Header("Base Attributes")]
        [SerializeField] private AttributeData baseAttributes;
        [SerializeField] private AttributeData attributeGrowth; // Per level growth
        
        [Header("Starting Stats")]
        [SerializeField] private int baseHP;
        [SerializeField] private int baseMP;
        [SerializeField] private int hpPerLevel = 8;
        [SerializeField] private int mpPerLevel = 4;
        
        [Header("Starting Equipment")]
        [SerializeField] private WeaponData startingWeapon;
        [SerializeField] private ArmorData startingArmor;
        [SerializeField] private ShieldData startingShield; // Optional
        [SerializeField] private Currency startingCurrency;
        
        [Header("Class Abilities")]
        [SerializeField] private SpellData[] availableSpells;
        [SerializeField] private string[] specialAbilities;
        
        // Public properties
        public string CharacterName => characterName;
        public CharacterClass Class => characterClass;
        public string Description => description;
        public Sprite Portrait => portrait;
        public AttributeData BaseAttributes => baseAttributes;
        public AttributeData AttributeGrowth => attributeGrowth;
        public int BaseHP => baseHP;
        public int BaseMP => baseMP;
        public int HPPerLevel => hpPerLevel;
        public int MPPerLevel => mpPerLevel;
        public WeaponData StartingWeapon => startingWeapon;
        public ArmorData StartingArmor => startingArmor;
        public ShieldData StartingShield => startingShield;
        public Currency StartingCurrency => startingCurrency;
        public SpellData[] AvailableSpells => availableSpells;
        public string[] SpecialAbilities => specialAbilities;
        
        /// <summary>
        /// Calculate attributes at a specific level
        /// </summary>
        public AttributeData GetAttributesAtLevel(int level)
        {
            level = Mathf.Max(1, level); // Minimum level 1
            int levelBonus = level - 1;
            
            return new AttributeData(
                baseAttributes.Strength + (attributeGrowth.Strength * levelBonus),
                baseAttributes.Dexterity + (attributeGrowth.Dexterity * levelBonus),
                baseAttributes.Constitution + (attributeGrowth.Constitution * levelBonus),
                baseAttributes.Intelligence + (attributeGrowth.Intelligence * levelBonus),
                baseAttributes.Wisdom + (attributeGrowth.Wisdom * levelBonus),
                baseAttributes.Charisma + (attributeGrowth.Charisma * levelBonus)
            );
        }
        
        /// <summary>
        /// Calculate maximum HP at a specific level
        /// </summary>
        public int GetMaxHPAtLevel(int level)
        {
            level = Mathf.Max(1, level);
            var attributes = GetAttributesAtLevel(level);
            return baseHP + (hpPerLevel * (level - 1)) + attributes.ConstitutionModifier;
        }
        
        /// <summary>
        /// Calculate maximum MP at a specific level
        /// </summary>
        public int GetMaxMPAtLevel(int level)
        {
            level = Mathf.Max(1, level);
            var attributes = GetAttributesAtLevel(level);
            
            // MP scaling depends on class
            int intelligenceBonus = characterClass == CharacterClass.Mage ? attributes.IntelligenceModifier * 2 : attributes.IntelligenceModifier;
            int wisdomBonus = characterClass == CharacterClass.Cleric ? attributes.WisdomModifier * 2 : attributes.WisdomModifier;
            
            return baseMP + (mpPerLevel * (level - 1)) + intelligenceBonus + wisdomBonus;
        }
        
        /// <summary>
        /// Check if character can equip a specific weapon type
        /// </summary>
        public bool CanEquipWeapon(WeaponType weaponType)
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    // Warriors can use all melee weapons
                    return weaponType != WeaponType.Bow && weaponType != WeaponType.Crossbow && weaponType != WeaponType.Staff;
                
                case CharacterClass.Ranger:
                    // Rangers can use ranged weapons, light melee weapons
                    return weaponType == WeaponType.Bow || weaponType == WeaponType.Crossbow || 
                           weaponType == WeaponType.Knife || weaponType == WeaponType.Sword || weaponType == WeaponType.Spear;
                
                case CharacterClass.Mage:
                    // Mages can only use staffs and knives
                    return weaponType == WeaponType.Staff || weaponType == WeaponType.Knife;
                
                case CharacterClass.Cleric:
                    // Clerics can use one-handed weapons, shields, and staffs
                    return weaponType == WeaponType.Mace || weaponType == WeaponType.Sword || 
                           weaponType == WeaponType.Staff || weaponType == WeaponType.Knife;
                
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Check if character can wear specific armor type
        /// </summary>
        public bool CanEquipArmor(ArmorType armorType)
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    return true; // Warriors can wear any armor
                
                case CharacterClass.Ranger:
                case CharacterClass.Cleric:
                    return armorType != ArmorType.Heavy; // Light and medium only
                
                case CharacterClass.Mage:
                    return armorType == ArmorType.Light; // Light armor only
                
                default:
                    return false;
            }
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Validate character data in the Unity Editor
        /// </summary>
        private void OnValidate()
        {
            // Ensure base stats are reasonable
            baseHP = Mathf.Max(1, baseHP);
            baseMP = Mathf.Max(0, baseMP);
            hpPerLevel = Mathf.Max(1, hpPerLevel);
            mpPerLevel = Mathf.Max(0, mpPerLevel);
            
            // Ensure character name is not empty
            if (string.IsNullOrEmpty(characterName))
            {
                characterName = characterClass.ToString();
            }
        }
        #endif
    }
}
