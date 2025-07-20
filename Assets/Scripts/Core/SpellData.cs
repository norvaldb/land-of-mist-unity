using UnityEngine;
using LandOfMist.Core;

namespace LandOfMist.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject for spell data implementing the ISpell interface
    /// Supports Fire, Water, and Earth magic schools with various targeting and effect types
    /// </summary>
    [CreateAssetMenu(fileName = "New Spell", menuName = "Land of Mist/Magic/Spell")]
    public class SpellData : ScriptableObject, ISpell
    {
        [Header("Spell Identity")]
        [SerializeField] private string spellName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        
        [Header("Spell Properties")]
        [SerializeField] private SpellSchool school;
        [SerializeField] private int manaCost;
        [SerializeField] private TargetType targetType;
        [SerializeField] private int spellLevel = 1; // 1-5 for spell progression
        
        [Header("Spell Effects")]
        [SerializeField] private EffectData[] effects;
        
        [Header("Requirements")]
        [SerializeField] private AttributeRequirements requirements;
        [SerializeField] private int minimumCharacterLevel = 1;
        
        [Header("Casting")]
        [SerializeField] private float castTime = 1.0f;          // Time to cast in combat turns
        [SerializeField] private float cooldownTurns = 0f;       // Turns before spell can be cast again
        [SerializeField] private bool requiresLineOfSight = true;
        [SerializeField] private float range = 10f;              // Spell range in meters
        
        [Header("Visual & Audio")]
        [SerializeField] private string castingAnimation;
        [SerializeField] private string spellEffectPrefab;
        [SerializeField] private AudioClip castingSound;
        
        // Interface implementations
        public SpellSchool School => school;
        public int ManaCost => manaCost;
        public TargetType Target => targetType;
        public EffectData[] Effects => effects;
        public AttributeRequirements Requirements => requirements;
        
        // Additional properties
        public string SpellName => spellName;
        public string Description => description;
        public Sprite Icon => icon;
        public int SpellLevel => spellLevel;
        public int MinimumCharacterLevel => minimumCharacterLevel;
        public float CastTime => castTime;
        public float CooldownTurns => cooldownTurns;
        public bool RequiresLineOfSight => requiresLineOfSight;
        public float Range => range;
        public string CastingAnimation => castingAnimation;
        public string SpellEffectPrefab => spellEffectPrefab;
        public AudioClip CastingSound => castingSound;
        
        /// <summary>
        /// Check if caster can cast this spell
        /// </summary>
        public bool CanCast(ICombatant caster)
        {
            // Check mana cost
            if (caster.CurrentMP < manaCost)
                return false;
            
            // Check if caster is alive and can act
            if (!caster.IsAlive || !caster.CanAct)
                return false;
            
            // Check attribute requirements
            if (!requirements.MeetsRequirements(caster.Attributes, GetRequiredClass()))
                return false;
            
            return true;
        }
        
        /// <summary>
        /// Cast the spell on specified targets
        /// </summary>
        public void Cast(ICombatant caster, ICombatant[] targets)
        {
            if (!CanCast(caster))
                return;
            
            // Consume mana
            int actualManaCost = GetEffectiveManaCost(caster.Attributes);
            caster.RestoreMP(-actualManaCost);
            
            // Apply effects to each target
            foreach (var target in targets)
            {
                if (target != null && ShouldApplyEffectToTarget(target, caster))
                {
                    ApplyEffectsToTarget(target, caster);
                }
            }
        }
        
        /// <summary>
        /// Calculate effective mana cost based on caster's attributes
        /// </summary>
        public int GetEffectiveManaCost(AttributeData casterAttributes)
        {
            float cost = manaCost;
            
            // Intelligence reduces mana cost for all spells
            float intelligenceReduction = casterAttributes.IntelligenceModifier * 0.05f; // 5% per point
            
            // Wisdom reduces mana cost for specific schools
            float wisdomReduction = 0f;
            if (school == SpellSchool.Water || school == SpellSchool.Earth)
            {
                wisdomReduction = casterAttributes.WisdomModifier * 0.03f; // 3% per point for nature-based magic
            }
            
            cost *= (1f - intelligenceReduction - wisdomReduction);
            return Mathf.Max(1, Mathf.RoundToInt(cost));
        }
        
        /// <summary>
        /// Calculate spell effectiveness based on caster's attributes
        /// </summary>
        public float GetSpellPower(AttributeData casterAttributes)
        {
            float basePower = 1.0f;
            
            // Primary attribute for spell power
            switch (school)
            {
                case SpellSchool.Fire:
                    basePower += casterAttributes.IntelligenceModifier * 0.1f; // 10% per intelligence modifier
                    break;
                case SpellSchool.Water:
                    basePower += (casterAttributes.IntelligenceModifier + casterAttributes.WisdomModifier) * 0.05f; // 5% per modifier
                    break;
                case SpellSchool.Earth:
                    basePower += casterAttributes.WisdomModifier * 0.1f; // 10% per wisdom modifier
                    basePower += casterAttributes.ConstitutionModifier * 0.05f; // 5% per constitution modifier
                    break;
            }
            
            // Spell level multiplier
            basePower *= (1f + (spellLevel - 1) * 0.2f); // 20% increase per spell level above 1
            
            return Mathf.Max(0.1f, basePower);
        }
        
        /// <summary>
        /// Get spell critical hit chance
        /// </summary>
        public float GetCriticalChance(AttributeData casterAttributes)
        {
            float baseCritChance = 0.05f; // 5% base
            
            // Intelligence improves critical chance for all schools
            baseCritChance += casterAttributes.IntelligenceModifier * 0.01f; // 1% per modifier
            
            // School-specific bonuses
            switch (school)
            {
                case SpellSchool.Fire:
                    baseCritChance += 0.02f; // Fire magic has higher crit chance
                    break;
                case SpellSchool.Water:
                    // Water magic focuses on consistency over crits
                    break;
                case SpellSchool.Earth:
                    baseCritChance += casterAttributes.WisdomModifier * 0.005f; // 0.5% per wisdom modifier
                    break;
            }
            
            return Mathf.Clamp01(baseCritChance);
        }
        
        /// <summary>
        /// Apply spell effects to a target
        /// </summary>
        private void ApplyEffectsToTarget(ICombatant target, ICombatant caster)
        {
            if (effects == null) return;
            
            float spellPower = GetSpellPower(caster.Attributes);
            bool isCritical = Random.value < GetCriticalChance(caster.Attributes);
            float criticalMultiplier = isCritical ? 1.5f : 1.0f;
            
            foreach (var effect in effects)
            {
                ApplyEffect(effect, target, caster, spellPower * criticalMultiplier);
            }
        }
        
        /// <summary>
        /// Apply a single effect to a target
        /// </summary>
        private void ApplyEffect(EffectData effect, ICombatant target, ICombatant caster, float powerMultiplier)
        {
            int effectValue = Mathf.RoundToInt(effect.baseValue * powerMultiplier);
            
            switch (effect.effectType)
            {
                case EffectData.EffectType.Damage:
                    target.TakeDamage(effectValue);
                    break;
                
                case EffectData.EffectType.Healing:
                    target.RestoreHP(effectValue);
                    break;
                
                case EffectData.EffectType.DamageOverTime:
                    // TODO: Implement DoT system
                    Debug.Log($"Applied {effectValue} {effect.elementType} DoT for {effect.duration} turns");
                    break;
                
                case EffectData.EffectType.HealingOverTime:
                    // TODO: Implement HoT system
                    Debug.Log($"Applied {effectValue} healing over {effect.duration} turns");
                    break;
                
                case EffectData.EffectType.Buff:
                case EffectData.EffectType.Debuff:
                    // TODO: Implement buff/debuff system
                    Debug.Log($"Applied {effect.effectType} for {effect.duration} turns");
                    break;
            }
        }
        
        /// <summary>
        /// Check if effect should be applied to target based on spell targeting
        /// </summary>
        private bool ShouldApplyEffectToTarget(ICombatant target, ICombatant caster)
        {
            switch (targetType)
            {
                case TargetType.Self:
                    return target == caster;
                
                case TargetType.SingleAlly:
                case TargetType.AllAllies:
                    return target != caster && target.IsAlive; // Assuming party members
                
                case TargetType.SingleEnemy:
                case TargetType.AllEnemies:
                    return target != caster; // Assuming enemies
                
                case TargetType.Area:
                    return target.IsAlive; // Area spells affect all valid targets
                
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Get required character class for this spell
        /// </summary>
        private CharacterClass GetRequiredClass()
        {
            return school switch
            {
                SpellSchool.Fire => CharacterClass.Mage,
                SpellSchool.Water => CharacterClass.Cleric, // Water spells often include healing
                SpellSchool.Earth => CharacterClass.Cleric, // Earth spells for protection and nature
                _ => CharacterClass.Mage
            };
        }
        
        /// <summary>
        /// Get spell description with current stats
        /// </summary>
        public string GetDetailedDescription(AttributeData casterAttributes)
        {
            string details = description + "\n\n";
            details += $"School: {school}\n";
            details += $"Mana Cost: {GetEffectiveManaCost(casterAttributes)}\n";
            details += $"Cast Time: {castTime} turns\n";
            details += $"Range: {range}m\n";
            details += $"Spell Power: {GetSpellPower(casterAttributes):P0}\n";
            details += $"Critical Chance: {GetCriticalChance(casterAttributes):P1}\n";
            
            return details;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Validate spell data in the Unity Editor
        /// </summary>
        private void OnValidate()
        {
            // Ensure reasonable values
            manaCost = Mathf.Max(1, manaCost);
            spellLevel = Mathf.Clamp(spellLevel, 1, 5);
            minimumCharacterLevel = Mathf.Max(1, minimumCharacterLevel);
            castTime = Mathf.Max(0.1f, castTime);
            cooldownTurns = Mathf.Max(0f, cooldownTurns);
            range = Mathf.Max(0f, range);
            
            // Ensure spell name
            if (string.IsNullOrEmpty(spellName))
            {
                spellName = school.ToString() + " Spell";
            }
            
            // Validate effects
            if (effects != null)
            {
                for (int i = 0; i < effects.Length; i++)
                {
                    var effect = effects[i];
                    effect.baseValue = Mathf.Max(0, effect.baseValue);
                    effect.duration = Mathf.Max(0, effect.duration);
                    effect.attributeScaling = Mathf.Max(0f, effect.attributeScaling);
                    effects[i] = effect;
                }
            }
        }
        #endif
    }
}
