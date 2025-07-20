using UnityEngine;

namespace LandOfMist.Core
{
    /// <summary>
    /// Interface for all weapon types in the game
    /// </summary>
    public interface IWeapon
    {
        WeaponType Type { get; }
        WeaponHandedness Handedness { get; }
        int BaseDamage { get; }
        float CriticalChance { get; }
        bool CanBeEnhanced { get; }
        bool HasArmorPenetration { get; }
        float PoisonEffectiveness { get; }
        AttributeRequirements Requirements { get; }
        
        // Poison enhancement system
        PoisonType AppliedPoison { get; }
        int PoisonCharges { get; }
        
        bool CanEquip(AttributeData characterAttributes);
        int CalculateDamage(AttributeData attackerAttributes);
    }
    
    /// <summary>
    /// Interface for all armor types in the game
    /// </summary>
    public interface IArmor
    {
        ArmorType Type { get; }
        int DefenseBonus { get; }
        float MovementPenalty { get; }
        float StealthPenalty { get; }
        ElementalResistance[] Resistances { get; }
        AttributeRequirements Requirements { get; }
        
        bool CanEquip(AttributeData characterAttributes);
    }
    
    /// <summary>
    /// Interface for all shield types in the game
    /// </summary>
    public interface IShield
    {
        ShieldType Type { get; }
        int DefenseBonus { get; }
        float BlockChance { get; }
        bool BlocksRangedAttacks { get; }
        ElementalResistance[] Resistances { get; }
        AttributeRequirements Requirements { get; }
        
        bool CanEquip(AttributeData characterAttributes);
    }
    
    /// <summary>
    /// Interface for all spells in the game
    /// </summary>
    public interface ISpell
    {
        SpellSchool School { get; }
        int ManaCost { get; }
        TargetType Target { get; }
        EffectData[] Effects { get; }
        AttributeRequirements Requirements { get; }
        
        bool CanCast(ICombatant caster);
        void Cast(ICombatant caster, ICombatant[] targets);
    }
}
