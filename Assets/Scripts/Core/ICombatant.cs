using UnityEngine;

namespace LandOfMist.Core
{
    /// <summary>
    /// Base interface for all combatants (characters and enemies)
    /// </summary>
    public interface ICombatant
    {
        string Name { get; }
        int CurrentHP { get; }
        int MaxHP { get; }
        int CurrentMP { get; }
        int MaxMP { get; }
        AttributeData Attributes { get; }
        
        bool IsAlive { get; }
        bool CanAct { get; }
        
        void TakeDamage(int damage);
        void RestoreHP(int amount);
        void RestoreMP(int amount);
    }
}
