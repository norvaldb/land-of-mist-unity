# Land of Mist Unity RPG

A text-based, turn-based RPG built in Unity featuring Tolkien-inspired fantasy world, party-based combat, and tactical decision-making.

## 🎮 Game Overview

**Genre**: Text-based Turn-based RPG
**Platform**: PC (Windows/Mac/Linux), with mobile adaptation potential
**Engine**: Unity 2023.3 LTS
**Development Status**: Planning & Early Development

### Core Features

- **Party-Based Combat**: Manage a party of 4 unique characters (Warrior, Ranger, Mage, Cleric)
- **Turn-Based Strategy**: Tactical combat with initiative-based turn order
- **Rich Magic System**: Three schools of magic (Fire, Frost, Poison) with distinct effects
- **Weapon Variety**: Swords, axes, spears, and bows with unique mechanics
- **Currency System**: Three-tier economy (Copper/Silver/Gold) with auto-conversion
- **Character Progression**: Experience-based leveling with attribute growth and skill specialization
- **Immersive Text Interface**: Medieval fantasy aesthetic with rich narrative descriptions

## 🏗️ Project Architecture

This project follows **SOLID principles** and clean architecture patterns optimized for Unity development:

```text
Unity Project Structure:
├── Assets/
│   ├── Scripts/
│   │   ├── Core/              # Game domain logic
│   │   ├── Systems/           # Game systems (singletons)
│   │   ├── RPG/              # RPG-specific mechanics
│   │   ├── UI/               # User interface components
│   │   └── Utils/            # Utility classes
│   ├── Prefabs/              # Reusable game objects
│   ├── Scenes/               # Game scenes
│   └── Data/                 # ScriptableObject data assets
├── Planning/                 # Design documents and roadmap
└── Tests/                    # Unit and integration tests
```

## 📋 Planning Documents

Comprehensive planning documents are available in the [`Planning/`](Planning/) directory:

- **[Core Mechanics](Planning/GameDesign/core-mechanics.md)**: Combat, progression, and economic systems
- **[Narrative Design](Planning/GameDesign/narrative-design.md)**: Story framework and world building
- **[System Architecture](Planning/Technical/system-architecture.md)**: Technical implementation details
- **[Development Roadmap](Planning/Production/development-roadmap.md)**: 6-month development timeline

## 🚀 Getting Started

### Prerequisites

- **Unity 2023.3 LTS** or later
- **Visual Studio Code** (recommended) with Unity extensions
- **Git** for version control

### Setup Instructions

1. **Clone the repository**:

   ```bash
   git clone https://github.com/norvaldb/land-of-mist-unity.git
   cd land-of-mist-unity
   ```

2. **Open in Unity**:
   - Launch Unity Hub
   - Click "Add" and select the project folder
   - Open with Unity 2023.3 LTS

3. **VS Code Setup** (Optional but recommended):
   - Open the project folder in VS Code
   - Install recommended extensions (see `.vscode/extensions.json`)
   - Unity integration will be configured automatically

### Development Workflow

1. **Follow SOLID Principles**: All code must adhere to clean architecture
2. **Test-Driven Development**: Write tests before implementation
3. **Data-Driven Design**: Use ScriptableObjects for game balance
4. **Event-Driven Architecture**: Loose coupling between systems

## 🎯 Development Phases

### Phase 1: Foundation (Weeks 1-4) ⏳

- [ ] Project setup and architecture
- [ ] Core data systems (save/load, currency)
- [ ] Character data framework

### Phase 2: Core Gameplay (Weeks 5-12) 📋

- [ ] Character system (classes, attributes, progression)
- [ ] Turn-based combat mechanics
- [ ] Magic system with three schools
- [ ] Equipment and inventory

### Phase 3: UI and Polish (Weeks 13-18) 🎨

- [ ] Text-based interface design
- [ ] Combat UI and feedback
- [ ] Menu systems and navigation

### Phase 4: Content and Balancing (Weeks 19-24) ⚖️

- [ ] World building and lore
- [ ] Game balance and testing
- [ ] Performance optimization

### Phase 5: Release Preparation (Weeks 25-26) 🚢

- [ ] Final polish and bug fixes
- [ ] Platform compatibility
- [ ] Release preparation

## 🧪 Testing Strategy

- **Unit Tests**: Pure C# logic testing in EditMode
- **Integration Tests**: System interactions in PlayMode
- **Manual Testing**: Gameplay balance and user experience
- **Performance Testing**: Frame rate and memory optimization

## 🤝 Contributing

This project follows professional development standards:

1. **Branch Strategy**: Feature branches with pull requests
2. **Code Review**: All changes require review
3. **Documentation**: Update planning docs with major changes
4. **Testing**: Maintain test coverage for new features

### Code Style

- Follow C# naming conventions
- Use SOLID principles consistently
- Comment complex business logic
- Keep methods focused and small

## 📊 Success Metrics

- **Technical**: 90%+ test coverage, 60 FPS performance
- **Gameplay**: Tactical combat, meaningful progression
- **User Experience**: Intuitive text interface, immersive narrative

## 🔮 Future Plans

- **Post-Launch**: Bug fixes, balance adjustments, QoL improvements
- **Content Updates**: New classes, spells, and world content
- **Long-term**: Multiplayer, advanced quest systems, mod support

## 📄 License

This project is developed as a personal learning project. See [LICENSE](LICENSE) for details.

## 🙏 Acknowledgments

- Inspired by classic text-based RPGs and Tolkien's fantasy works
- Built with Unity Engine and following clean architecture principles
- Community feedback and playtesting contributions welcome

---

**Current Status**: 🔧 Early Development - Planning Complete, Implementation Starting

For detailed technical information, see the [Planning Documents](Planning/) or check the [Development Roadmap](Planning/Production/development-roadmap.md) for current progress.
