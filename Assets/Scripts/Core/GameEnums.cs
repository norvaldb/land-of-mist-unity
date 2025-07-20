using UnityEngine;

namespace LandOfMist.Core
{
    /// <summary>
    /// Character classes available in the game
    /// </summary>
    public enum CharacterClass
    {
        Warrior,    // Tank, melee specialist
        Ranger,     // Ranged DPS, stealth and utility
        Mage,       // Magic DPS, elemental spells
        Cleric      // Healer, support magic
    }
    
    /// <summary>
    /// Weapon categories and types
    /// </summary>
    public enum WeaponType
    {
        // Melee Weapons
        Sword,
        GreatSword,
        Axe,
        GreatAxe,
        Mace,
        Spear,
        Knife,
        
        // Ranged Weapons
        Bow,
        Crossbow,
        
        // Magic Weapons
        Staff
    }
    
    /// <summary>
    /// Weapon handedness for equipment restrictions
    /// </summary>
    public enum WeaponHandedness
    {
        OneHanded,
        TwoHanded
    }
    
    /// <summary>
    /// Armor types with different protection levels
    /// </summary>
    public enum ArmorType
    {
        Light,      // Cloth, leather - low defense, no penalties
        Medium,     // Chain, scale - moderate defense, small penalties
        Heavy       // Plate - high defense, significant penalties
    }
    
    /// <summary>
    /// Shield types for different defensive strategies
    /// </summary>
    public enum ShieldType
    {
        Buckler,    // Small, light shield
        Round,      // Medium shield
        Tower,      // Large, heavy shield
        Magic       // Enchanted shield
    }
    
    /// <summary>
    /// Magic schools for spell organization
    /// </summary>
    public enum SpellSchool
    {
        Fire,       // Damage, burning effects
        Water,      // Healing, ice damage
        Earth       // Protection, poison effects
    }
    
    /// <summary>
    /// Spell targeting types
    /// </summary>
    public enum TargetType
    {
        Self,           // Caster only
        SingleAlly,     // One party member
        SingleEnemy,    // One enemy
        AllAllies,      // All party members
        AllEnemies,     // All enemies
        Area            // Area of effect
    }
    
    /// <summary>
    /// Poison types for weapon enhancement
    /// </summary>
    public enum PoisonType
    {
        None,
        Weak,       // Minor damage over time
        Strong,     // Significant damage over time
        Paralysis,  // Chance to skip turns
        Weakness    // Reduced damage output
    }
    
    /// <summary>
    /// Elemental damage and resistance types
    /// </summary>
    public enum ElementType
    {
        Physical,
        Fire,
        Water,
        Earth,
        Poison
    }
}
