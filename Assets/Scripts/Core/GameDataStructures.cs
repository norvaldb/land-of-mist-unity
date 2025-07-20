using UnityEngine;
using System;

namespace LandOfMist.Core
{
    /// <summary>
    /// Character attributes following D&D-style system
    /// </summary>
    [System.Serializable]
    public struct AttributeData
    {
        [SerializeField] private int strength;      // Melee damage, carrying capacity
        [SerializeField] private int dexterity;     // Ranged accuracy, stealth, initiative
        [SerializeField] private int constitution;  // Health points, poison resistance
        [SerializeField] private int intelligence;  // Mana points, spell effectiveness
        [SerializeField] private int wisdom;        // Perception, divine magic
        [SerializeField] private int charisma;      // Social interactions, leadership
        
        public int Strength => strength;
        public int Dexterity => dexterity;
        public int Constitution => constitution;
        public int Intelligence => intelligence;
        public int Wisdom => wisdom;
        public int Charisma => charisma;
        
        public AttributeData(int str, int dex, int con, int intel, int wis, int cha)
        {
            strength = str;
            dexterity = dex;
            constitution = con;
            intelligence = intel;
            wisdom = wis;
            charisma = cha;
        }
        
        /// <summary>
        /// Calculate attribute modifier (D&D style: (score - 10) / 2)
        /// </summary>
        public int GetModifier(int attributeValue)
        {
            return (attributeValue - 10) / 2;
        }
        
        public int StrengthModifier => GetModifier(strength);
        public int DexterityModifier => GetModifier(dexterity);
        public int ConstitutionModifier => GetModifier(constitution);
        public int IntelligenceModifier => GetModifier(intelligence);
        public int WisdomModifier => GetModifier(wisdom);
        public int CharismaModifier => GetModifier(charisma);
    }
    
    /// <summary>
    /// Equipment requirements for class and attribute restrictions
    /// </summary>
    [System.Serializable]
    public struct AttributeRequirements
    {
        public int minimumStrength;
        public int minimumDexterity;
        public int minimumConstitution;
        public int minimumIntelligence;
        public int minimumWisdom;
        public int minimumCharisma;
        
        public CharacterClass[] allowedClasses;
        
        /// <summary>
        /// Check if character meets all requirements
        /// </summary>
        public bool MeetsRequirements(AttributeData attributes, CharacterClass characterClass)
        {
            // Check attribute requirements
            if (attributes.Strength < minimumStrength) return false;
            if (attributes.Dexterity < minimumDexterity) return false;
            if (attributes.Constitution < minimumConstitution) return false;
            if (attributes.Intelligence < minimumIntelligence) return false;
            if (attributes.Wisdom < minimumWisdom) return false;
            if (attributes.Charisma < minimumCharisma) return false;
            
            // Check class restrictions
            if (allowedClasses != null && allowedClasses.Length > 0)
            {
                bool classAllowed = false;
                foreach (var allowedClass in allowedClasses)
                {
                    if (allowedClass == characterClass)
                    {
                        classAllowed = true;
                        break;
                    }
                }
                if (!classAllowed) return false;
            }
            
            return true;
        }
    }
    
    /// <summary>
    /// Elemental resistance data
    /// </summary>
    [System.Serializable]
    public struct ElementalResistance
    {
        public ElementType elementType;
        public float resistancePercentage; // 0.0 to 1.0 (0% to 100%)
        
        public ElementalResistance(ElementType element, float resistance)
        {
            elementType = element;
            resistancePercentage = Mathf.Clamp01(resistance);
        }
    }
    
    /// <summary>
    /// Spell effect data for damage, healing, buffs, etc.
    /// </summary>
    [System.Serializable]
    public struct EffectData
    {
        public EffectType effectType;
        public ElementType elementType;
        public int baseValue;           // Base damage/healing amount
        public float attributeScaling;  // Scaling factor with caster attributes
        public int duration;            // Duration in turns (0 = instant)
        public bool stackable;          // Can multiple instances stack
        
        public enum EffectType
        {
            Damage,
            Healing,
            Buff,
            Debuff,
            DamageOverTime,
            HealingOverTime
        }
    }
    
    /// <summary>
    /// Three-tier currency system with automatic conversion
    /// </summary>
    [System.Serializable]
    public struct Currency
    {
        [SerializeField] private int totalCopper;
        
        public int Gold => totalCopper / 10000;
        public int Silver => (totalCopper % 10000) / 100;
        public int Copper => totalCopper % 100;
        public int TotalCopper => totalCopper;
        
        public Currency(int gold = 0, int silver = 0, int copper = 0)
        {
            totalCopper = gold * 10000 + silver * 100 + copper;
        }
        
        public static Currency FromCopper(int copper) => new Currency { totalCopper = copper };
        
        public static Currency operator +(Currency a, Currency b) => FromCopper(a.totalCopper + b.totalCopper);
        public static Currency operator -(Currency a, Currency b) => FromCopper(Mathf.Max(0, a.totalCopper - b.totalCopper));
        
        public static bool operator >=(Currency a, Currency b) => a.totalCopper >= b.totalCopper;
        public static bool operator <=(Currency a, Currency b) => a.totalCopper <= b.totalCopper;
        public static bool operator >(Currency a, Currency b) => a.totalCopper > b.totalCopper;
        public static bool operator <(Currency a, Currency b) => a.totalCopper < b.totalCopper;
        
        public override string ToString()
        {
            if (Gold > 0)
                return $"{Gold}g {Silver}s {Copper}c";
            else if (Silver > 0)
                return $"{Silver}s {Copper}c";
            else
                return $"{Copper}c";
        }
    }
}
