export interface EnemyTemplate {
  id: string; // Unique identifier for the enemy template
  name: string; // Name of the enemy
  description: string; // Description of the enemy
  baseHealth: number; // Maximum health of the enemy
  baseStrength: number; // Strength attribute of the enemy
  baseDefense: number; // Defense attribute of the enemy
  enemyType: EnemyType; // Type of the enemy (e.g., Beast, Undead, Dragon, Humanoid)
  experienceReward: number; // Experience points rewarded for defeating this enemy
  possibleLoot?: string[]; // Optional list of loot items dropped by the enemy
  attackPattern: string; // Attack pattern or behavior of the enemy
  specialAbilities: { [key: string]: any }; // Special abilities of the enemy, keyed by ability name
}

export interface ItemTemplate {
  id: string; // Unique identifier for the item template
  name: string; // Name of the item
  description: string; // Description of the item
  itemType: ItemType; // Type of the item (e.g., weapon, armor, potion)
  value: number; // Value of the item in the game currency
  statModifiers: { [key: string]: number }; // Stat modifiers provided by the item, keyed by stat name
  isConsumable: boolean; // Indicates if the item is consumable (e.g., potions)
  isEquippable: boolean; // Indicates if the item can be equipped
  equipmentSlot?: EquipmentSlot; // Optional slot for equippable items (e.g., head, chest, weapon)
}

export enum EnemyType {
  Beast = 'Beast',
  Undead = 'Undead',
  Dragon = 'Dragon',
  Humanoid = 'Humanoid'
}

export enum ItemType {
  Weapon = 'Weapon',
  Armor = 'Armor',
  Consumable = 'Consumable',
  Misc = 'Misc'
}

export enum EquipmentSlot {
  Weapon = 'Weapon',
  Armor = 'Armor',
  Helmet = 'Helmet',
  Boots = 'Boots'
}
