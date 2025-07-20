using UnityEngine;

namespace LandOfMist.RPG.Characters
{
    /// <summary>
    /// Base class for all characters in the RPG system
    /// </summary>
    public abstract class BaseCharacter : MonoBehaviour
    {
        [Header("Character Info")]
        [SerializeField] protected string characterName;
        [SerializeField] protected int level = 1;
        
        [Header("Core Stats")]
        [SerializeField] protected int maxHealth = 100;
        [SerializeField] protected int currentHealth;
        [SerializeField] protected int maxMana = 50;
        [SerializeField] protected int currentMana;
        
        [Header("Primary Attributes")]
        [SerializeField] protected int strength = 10;
        [SerializeField] protected int dexterity = 10;
        [SerializeField] protected int intelligence = 10;
        [SerializeField] protected int constitution = 10;
        [SerializeField] protected int wisdom = 10;
        [SerializeField] protected int charisma = 10;
        
        // Events
        public System.Action<int, int> OnHealthChanged; // current, max
        public System.Action<int, int> OnManaChanged;   // current, max
        public System.Action OnCharacterDeath;
        
        // Properties
        public string CharacterName => characterName;
        public int Level => level;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public int CurrentMana => currentMana;
        public int MaxMana => maxMana;
        public bool IsAlive => currentHealth > 0;
        
        protected virtual void Awake()
        {
            InitializeCharacter();
        }
        
        protected virtual void InitializeCharacter()
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
        
        public virtual void TakeDamage(int damage)
        {
            if (!IsAlive) return;
            
            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public virtual void Heal(int healAmount)
        {
            if (!IsAlive) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public virtual bool UseMana(int manaCost)
        {
            if (currentMana < manaCost) return false;
            
            currentMana -= manaCost;
            OnManaChanged?.Invoke(currentMana, maxMana);
            return true;
        }
        
        public virtual void RestoreMana(int manaAmount)
        {
            currentMana = Mathf.Min(maxMana, currentMana + manaAmount);
            OnManaChanged?.Invoke(currentMana, maxMana);
        }
        
        protected virtual void Die()
        {
            OnCharacterDeath?.Invoke();
            // Override in derived classes for specific death behavior
        }
        
        // Stat calculations
        public int GetAttackPower()
        {
            return strength + (level * 2);
        }
        
        public int GetDefense()
        {
            return constitution + (level);
        }
        
        public int GetSpellPower()
        {
            return intelligence + (level * 2);
        }
        
        public int GetCriticalChance()
        {
            return Mathf.Min(50, dexterity / 2); // Max 50% crit chance
        }
    }
}
