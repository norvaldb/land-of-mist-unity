# Core Mechanics - Land of Mist RPG

## Purpose

Define the fundamental gameplay systems that drive the Land of Mist RPG experience, including combat, character progression, economy, and party management.

## Game Overview

A text-based, turn-based RPG set in a Tolkien-inspired fantasy world featuring party-based combat, tactical decision-making, and character progression.

## Core Systems

### 1. Party System

#### Requirements

- Manage a party of exactly 4 characters
- Each character has distinct classes and abilities
- Party formation affects combat tactics
- Character synergies and combinations

#### Design

- **Party Composition**: 4 distinct character slots
- **Character Classes**:
  - **Warrior**: Melee combat specialist, high defense, sword/axe mastery
  - **Ranger**: Ranged combat, bow expertise, stealth abilities
  - **Mage**: Spell casting, elemental magic, low physical defense
  - **Cleric**: Healing, support magic, moderate combat ability
- **Party Roles**:
  - Tank (front-line defense)
  - DPS (damage dealers)
  - Support (healing/buffs)
  - Utility (special abilities)

#### Dependencies

- Character creation system
- Class progression system
- Equipment management

### 2. Currency System

#### Requirements

- Three-tier currency system with automatic conversion
- Realistic economic progression
- Clear value representation in UI

#### Design

- **Currency Tiers**:
  - **Copper Coins**: Base currency (1-99 copper)
  - **Silver Coins**: Mid-tier (1 silver = 100 copper)
  - **Gold Coins**: High-tier (1 gold = 100 silver = 10,000 copper)
- **Auto-Conversion**: Automatic promotion (100 copper → 1 silver)
- **Display Format**: "2g 15s 47c" (gold/silver/copper)
- **Economic Balance**:
  - Starting equipment: 5-50 copper
  - Basic weapons/armor: 1-10 silver
  - Rare equipment: 1-5 gold
  - Epic items: 10+ gold

#### Dependencies

- Inventory system
- Shop/trading system
- Loot generation

### 3. Combat System

#### Requirements

- Turn-based tactical combat
- Weapon type advantages/disadvantages
- Magic system integration
- Status effects and conditions

#### Design

- **Turn Order**: Initiative-based (Dexterity + dice roll)
- **Combat Actions**:
  - Attack (weapon-based)
  - Cast Spell (magic-based)
  - Use Item (consumables)
  - Defend (damage reduction)
  - Special Abilities (class-specific)
- **Weapon Types**:
  - **Swords**: Balanced damage, versatile, parry bonus
  - **Axes**: High damage, slower speed, armor penetration
  - **Spears**: Medium damage, reach advantage, formation bonus
  - **Bows**: Ranged damage, requires ammunition, critical hit bonus
- **Damage Calculation**:

  ```
  Base Damage = Weapon Damage + Attribute Modifier
  Final Damage = Base Damage - Armor Defense + Critical Multiplier
  ```

#### Dependencies

- Character attribute system
- Equipment system
- Magic system

### 4. Magic System

#### Requirements

- Elemental magic types with distinct effects
- Mana/resource management
- Spell progression and learning

#### Design

- **Magic Schools**:
  - **Fire Magic**:
    - Fireball: Single target, high damage, burn effect
    - Flame Burst: Area damage, lower per-target damage
  - **Frost Magic**:
    - Frost Projectile: Single target, slow effect
    - Ice Shield: Defensive spell, damage absorption
  - **Poison Magic**:
    - Poison Dart: Damage over time effect
    - Toxic Cloud: Area poison, vision reduction
- **Mana System**:
  - Each character has Mana Points (MP)
  - Spells consume MP based on power level
  - MP regenerates slowly per turn or via items/rest
- **Spell Components**:
  - Verbal (speaking) - can be silenced
  - Material (components) - can be consumed
  - Somatic (gestures) - can be restricted

#### Dependencies

- Character magic attributes
- Status effect system
- Resource management

### 5. Character Progression

#### Requirements

- Experience-based leveling system
- Attribute growth and customization
- Skill specialization paths

#### Design

- **Core Attributes**:
  - **Strength**: Melee damage, carrying capacity
  - **Dexterity**: Initiative, ranged accuracy, dodge
  - **Intelligence**: Mana pool, spell damage, magic resistance
  - **Constitution**: Health points, poison resistance
  - **Wisdom**: Mana regeneration, spell accuracy
  - **Charisma**: Trade prices, party leadership
- **Leveling System**:
  - Experience points from combat and quest completion
  - Level cap: 20 (allows for focused progression)
  - Each level: +1 to primary attribute, +health/mana
- **Skill Trees**: Class-specific advancement paths
  - Combat specializations
  - Magic school mastery
  - Utility abilities

#### Dependencies

- Experience point system
- Class definition system
- Equipment requirements

## UI/UX Requirements

### Text-Based Interface Design

- **Font**: Medieval/fantasy serif font (e.g., Cinzel, Trajan Pro)
- **Color Scheme**:
  - Background: Dark brown/black parchment texture
  - Text: Cream/gold for readability
  - Highlights: Green for positive, red for negative
- **Layout Sections**:
  - Party status (health, mana, conditions)
  - Current scene description
  - Available actions
  - Inventory/equipment quick view
  - Message log

### Information Display

- **Party Status Panel**:

  ```
  [Warrior] Aragorn    HP: 45/50  MP: 5/10   [Poisoned]
  [Ranger]  Legolas    HP: 35/40  MP: 15/20  [Ready]
  [Mage]    Gandalf    HP: 20/25  MP: 40/50  [Casting]
  [Cleric]  Elrond     HP: 30/35  MP: 25/30  [Ready]
  ```

- **Currency Display**: Gold: 2 | Silver: 15 | Copper: 47
- **Combat Log**: Scrolling text of recent actions and results

## Technical Considerations

### Data Storage

- Character data in ScriptableObjects
- Save/load system for game state
- Modular design for easy balancing

### Performance

- Text-based interface for fast rendering
- Minimal graphics processing requirements
- Efficient state management

### Extensibility

- Plugin architecture for new classes/spells
- Data-driven equipment and magic systems
- Mod support considerations

## Success Metrics

- Combat encounters feel tactical and engaging
- Character progression provides meaningful choices
- Party composition creates strategic depth
- Economic system feels balanced and rewarding
