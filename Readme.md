# RPG Game Domain

This project defines the core domain logic for an RPG game, including characters, items, quests, and game mechanics. It is built using C# 12.0 and targets .NET 8. The architecture is modular and extensible, leveraging interfaces, abstract classes, and enums to represent various game elements.

---

## **Project Structure**

The project is organized into the following key components:

### **1. Interfaces**
Interfaces define the contracts for various entities in the game, ensuring consistent behavior across implementations.

#### **Characters**
- **`ICharacter`**: Base interface for all characters.
  - Properties: `Name`, `Health`, `MaxHealth`, `Level`, `IsAlive`.
  - Methods: `Attack`, `TakeDamage`, `Heal`, `LevelUp`.

- **`INonPlayerCharacter`**: Extends `ICharacter` for NPC-specific behavior.
  - Properties: `IsFriendly`, `Dialogue`.
  - Methods: `Interact`.

- **`IPlayerCharacter`**: Extends `ICharacter` for player-specific behavior.
  - Properties: `Experience`, `ExperienceToNextLevel`, `Inventory`.
  - Methods: `GainExperience`, `EquipItem`, `UseItem`, `UseSpecialAbility`.

#### **Items**
- **`IItem`**: Base interface for all items.
  - Properties: `Name`, `Description`, `Value`, `Type`.

- **`IEquipment`**: Extends `IItem` for equippable items.
  - Properties: `Slot`, `BonusValue`.
  - Methods: `OnEquip`, `OnUnequip`.

- **`IConsumable`**: Extends `IItem` for consumable items.
  - Methods: `Consume`.

#### **Inventory**
- **`IInventory`**: Manages a character's inventory.
  - Properties: `Items`, `Capacity`, `Gold`.
  - Methods: `AddItem`, `RemoveItem`, `AddGold`, `SpendGold`.

#### **Quests**
- **`IQuest`**: Represents a quest in the game.
  - Properties: `Name`, `Description`, `IsCompleted`.
  - Methods: `Complete`.

---

### **2. Enums**
Enums categorize various game elements for better readability and maintainability.

- **`CharacterType`**: Defines types of characters (e.g., Warrior, Mage).
- **`DamageType`**: Defines types of damage (e.g., Physical, Magical).
- **`EnemyType`**: Defines types of enemies (e.g., Goblin, Dragon).
- **`EquipmentSlot`**: Defines equipment slots (e.g., Head, Chest).
- **`ItemType`**: Defines types of items (e.g., Weapon, Potion).
- **`LocationType`**: Defines types of locations (e.g., Town, Dungeon).

---

### **3. Entities**
Entities provide concrete implementations of the interfaces and define the behavior of game elements.

#### **Characters**
- **`Character`**: Abstract base class implementing `ICharacter`.
  - Handles common character logic like attacking, taking damage, healing, and leveling up.
  - Includes protected methods for customization (e.g., `OnBeforeAttack`, `OnDeath`).

- **`NonPlayerCharacter`**: Abstract class extending `Character` for NPCs.
  - Adds properties like `IsFriendly` and `Dialogue`.
  - Defines interaction behavior with players.

- **`PlayerCharacter`**: Concrete class for player-controlled characters.
  - Implements inventory and experience management.

- **`Enemy`**: Abstract class extending `NonPlayerCharacter` for enemies.
  - Adds properties like `ExperienceReward` and `LootTable`.
  - Includes methods for dropping loot and initiating combat.

---

## **Key Features**
- **Modular Design**: Interfaces and abstract classes allow for easy extension and customization.
- **Encapsulation**: Fields are protected, with controlled access through properties.
- **Polymorphism**: Enables treating different character types uniformly via the `ICharacter` interface.
- **Extensibility**: New character types, items, or quests can be added with minimal changes to existing code.

---

## **Getting Started**
1. Clone the repository.
2. Open the solution in Visual Studio 2022.
3. Build the project to ensure all dependencies are resolved.
4. Explore the `Entities`, `Interfaces`, and `Enums` folders to understand the domain logic.

---

## **Future Enhancements**
- Add more character types (e.g., BossEnemy, Merchant).
- Implement advanced combat mechanics (e.g., special abilities, status effects).
- Expand the quest system with branching storylines.
- Introduce multiplayer support.

---

## **Contributing**
Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a feature branch.
3. Commit your changes with clear messages.
4. Submit a pull request for review.

---

## **License**
This project is licensed under the MIT License. See the `LICENSE` file for details.
