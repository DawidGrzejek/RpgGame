using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using System;
using System.Collections.Generic;
using Xunit;

namespace RpgGame.UnitTests.Domain.Entities
{
    public class ItemTemplateTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesTemplate()
        {
            // Arrange
            var name = "Iron Sword";
            var description = "A sturdy iron sword";
            var itemType = ItemType.Weapon;
            var value = 100;

            // Act
            var template = new ItemTemplate(name, description, itemType, value, false, true, EquipmentSlot.MainHand);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(itemType, template.ItemType);
            Assert.Equal(value, template.Value);
            Assert.False(template.IsConsumable);
            Assert.True(template.IsEquippable);
            Assert.Equal(EquipmentSlot.MainHand, template.EquipmentSlot);
            Assert.NotNull(template.StatModifiers);
            Assert.Empty(template.StatModifiers);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new ItemTemplate(invalidName, "Description", ItemType.Weapon, 100));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_InvalidDescription_ThrowsArgumentException(string invalidDescription)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new ItemTemplate("Name", invalidDescription, ItemType.Weapon, 100));
        }

        [Fact]
        public void Constructor_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new ItemTemplate("Name", "Description", ItemType.Weapon, -1));
        }

        [Fact]
        public void CreateWeaponTemplate_ValidParameters_CreatesWeaponTemplate()
        {
            // Arrange
            var name = "Steel Sword";
            var description = "A sharp steel sword";
            var value = 200;
            var slot = EquipmentSlot.MainHand;
            var statModifiers = new Dictionary<string, int> 
            { 
                { "Strength", 10 }, 
                { "Attack", 15 } 
            };

            // Act
            var template = ItemTemplate.CreateWeaponTemplate(name, description, value, slot, statModifiers);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.Equal(value, template.Value);
            Assert.False(template.IsConsumable);
            Assert.True(template.IsEquippable);
            Assert.Equal(slot, template.EquipmentSlot);
            Assert.Equal(2, template.StatModifiers.Count);
            Assert.Equal(10, template.GetStatModifier("Strength"));
            Assert.Equal(15, template.GetStatModifier("Attack"));
        }

        [Fact]
        public void CreateArmorTemplate_ValidParameters_CreatesArmorTemplate()
        {
            // Arrange
            var name = "Leather Armor";
            var description = "Basic leather protection";
            var value = 50;
            var slot = EquipmentSlot.Chest;
            var statModifiers = new Dictionary<string, int> 
            { 
                { "Defense", 8 } 
            };

            // Act
            var template = ItemTemplate.CreateArmorTemplate(name, description, value, slot, statModifiers);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(ItemType.Armor, template.ItemType);
            Assert.Equal(value, template.Value);
            Assert.False(template.IsConsumable);
            Assert.True(template.IsEquippable);
            Assert.Equal(slot, template.EquipmentSlot);
            Assert.Single(template.StatModifiers);
            Assert.Equal(8, template.GetStatModifier("Defense"));
        }

        [Fact]
        public void CreatePotionTemplate_ValidParameters_CreatesPotionTemplate()
        {
            // Arrange
            var name = "Health Potion";
            var description = "Restores health";
            var value = 25;
            var effects = new Dictionary<string, int> 
            { 
                { "Health", 50 } 
            };

            // Act
            var template = ItemTemplate.CreatePotionTemplate(name, description, value, effects);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(ItemType.Potion, template.ItemType);
            Assert.Equal(value, template.Value);
            Assert.True(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
            Assert.Single(template.StatModifiers);
            Assert.Equal(50, template.GetStatModifier("Health"));
        }

        [Fact]
        public void CreateQuestItemTemplate_ValidParameters_CreatesQuestItemTemplate()
        {
            // Arrange
            var name = "Ancient Key";
            var description = "A mysterious ancient key";
            var value = 0;

            // Act
            var template = ItemTemplate.CreateQuestItemTemplate(name, description, value);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(ItemType.QuestItem, template.ItemType);
            Assert.Equal(value, template.Value);
            Assert.False(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
            Assert.Empty(template.StatModifiers);
        }

        [Fact]
        public void CreateMiscellaneousTemplate_ValidParameters_CreatesMiscellaneousTemplate()
        {
            // Arrange
            var name = "Gold Coin";
            var description = "Shiny gold currency";
            var value = 1;
            var isConsumable = false;

            // Act
            var template = ItemTemplate.CreateMiscellaneousTemplate(name, description, value, isConsumable);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(ItemType.Miscellaneous, template.ItemType);
            Assert.Equal(value, template.Value);
            Assert.Equal(isConsumable, template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
        }

        [Fact]
        public void AddStatModifier_ValidStatName_AddsModifier()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            var statName = "Strength";
            var modifier = 10;

            // Act
            template.AddStatModifier(statName, modifier);

            // Assert
            Assert.True(template.HasStatModifier(statName));
            Assert.Equal(modifier, template.GetStatModifier(statName));
            Assert.Equal(1, template.GetStatModifierCount());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void AddStatModifier_InvalidStatName_ThrowsArgumentException(string invalidStatName)
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                template.AddStatModifier(invalidStatName, 10));
        }

        [Fact]
        public void AddStatModifier_DuplicateStatName_UpdatesModifier()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            var statName = "Strength";
            template.AddStatModifier(statName, 10);

            // Act
            template.AddStatModifier(statName, 15);

            // Assert
            Assert.Equal(15, template.GetStatModifier(statName));
            Assert.Equal(1, template.GetStatModifierCount());
        }

        [Fact]
        public void RemoveStatModifier_ExistingStatName_RemovesModifier()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            var statName = "Strength";
            template.AddStatModifier(statName, 10);

            // Act
            var result = template.RemoveStatModifier(statName);

            // Assert
            Assert.True(result);
            Assert.False(template.HasStatModifier(statName));
            Assert.Equal(0, template.GetStatModifierCount());
        }

        [Fact]
        public void RemoveStatModifier_NonExistentStatName_ReturnsFalse()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act
            var result = template.RemoveStatModifier("NonExistent");

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void RemoveStatModifier_InvalidStatName_ThrowsArgumentException(string invalidStatName)
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                template.RemoveStatModifier(invalidStatName));
        }

        [Fact]
        public void GetStatModifier_ExistingStatName_ReturnsModifier()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            var statName = "Strength";
            var modifier = 10;
            template.AddStatModifier(statName, modifier);

            // Act
            var result = template.GetStatModifier(statName);

            // Assert
            Assert.Equal(modifier, result);
        }

        [Fact]
        public void GetStatModifier_NonExistentStatName_ReturnsDefaultValue()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            var defaultValue = 5;

            // Act
            var result = template.GetStatModifier("NonExistent", defaultValue);

            // Assert
            Assert.Equal(defaultValue, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GetStatModifier_InvalidStatName_ThrowsArgumentException(string invalidStatName)
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                template.GetStatModifier(invalidStatName));
        }

        [Fact]
        public void UpdateDetails_ValidParameters_UpdatesAllProperties()
        {
            // Arrange
            var template = new ItemTemplate("Old Name", "Old Description", ItemType.Weapon, 100);
            var newName = "New Name";
            var newDescription = "New Description";
            var newItemType = ItemType.Armor;
            var newValue = 200;
            var newIsConsumable = true;
            var newIsEquippable = false;
            var newEquipmentSlot = EquipmentSlot.Chest;

            // Act
            template.UpdateDetails(newName, newDescription, newItemType, newValue, newIsConsumable, newIsEquippable, newEquipmentSlot);

            // Assert
            Assert.Equal(newName, template.Name);
            Assert.Equal(newDescription, template.Description);
            Assert.Equal(newItemType, template.ItemType);
            Assert.Equal(newValue, template.Value);
            Assert.Equal(newIsConsumable, template.IsConsumable);
            Assert.Equal(newIsEquippable, template.IsEquippable);
            Assert.Equal(newEquipmentSlot, template.EquipmentSlot);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateDetails_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Arrange
            var template = new ItemTemplate("Name", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                template.UpdateDetails(invalidName, "New Description", ItemType.Armor, 200, false, true, null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateDetails_InvalidDescription_ThrowsArgumentException(string invalidDescription)
        {
            // Arrange
            var template = new ItemTemplate("Name", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                template.UpdateDetails("New Name", invalidDescription, ItemType.Armor, 200, false, true, null));
        }

        [Fact]
        public void UpdateDetails_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var template = new ItemTemplate("Name", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                template.UpdateDetails("New Name", "New Description", ItemType.Armor, -1, false, true, null));
        }

        [Fact]
        public void UpdateBasicDetails_ValidParameters_UpdatesBasicProperties()
        {
            // Arrange
            var template = new ItemTemplate("Old Name", "Old Description", ItemType.Weapon, 100);
            var newName = "New Name";
            var newDescription = "New Description";
            var newValue = 200;

            // Act
            template.UpdateBasicDetails(newName, newDescription, newValue);

            // Assert
            Assert.Equal(newName, template.Name);
            Assert.Equal(newDescription, template.Description);
            Assert.Equal(newValue, template.Value);
            // Other properties should remain unchanged
            Assert.Equal(ItemType.Weapon, template.ItemType);
        }

        [Fact]
        public void UpdateValue_ValidValue_UpdatesValue()
        {
            // Arrange
            var template = new ItemTemplate("Name", "Description", ItemType.Weapon, 100);
            var newValue = 250;

            // Act
            template.UpdateValue(newValue);

            // Assert
            Assert.Equal(newValue, template.Value);
        }

        [Fact]
        public void UpdateValue_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var template = new ItemTemplate("Name", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => template.UpdateValue(-1));
        }

        [Fact]
        public void ClearStatModifiers_WithModifiers_ClearsAllAndReturnsTrue()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            template.AddStatModifier("Strength", 10);
            template.AddStatModifier("Dexterity", 5);

            // Act
            var result = template.ClearStatModifiers();

            // Assert
            Assert.True(result);
            Assert.Empty(template.StatModifiers);
            Assert.Equal(0, template.GetStatModifierCount());
        }

        [Fact]
        public void ClearStatModifiers_WithoutModifiers_ReturnsFalse()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act
            var result = template.ClearStatModifiers();

            // Assert
            Assert.False(result);
            Assert.Equal(0, template.GetStatModifierCount());
        }

        [Fact]
        public void HasStatModifier_ExistingModifier_ReturnsTrue()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            template.AddStatModifier("Strength", 10);

            // Act & Assert
            Assert.True(template.HasStatModifier("Strength"));
        }

        [Fact]
        public void HasStatModifier_NonExistentModifier_ReturnsFalse()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.False(template.HasStatModifier("Strength"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void HasStatModifier_InvalidStatName_ThrowsArgumentException(string invalidStatName)
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                template.HasStatModifier(invalidStatName));
        }

        [Fact]
        public void GetStatModifierCount_WithMultipleModifiers_ReturnsCorrectCount()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            template.AddStatModifier("Strength", 10);
            template.AddStatModifier("Dexterity", 5);
            template.AddStatModifier("Intelligence", 8);

            // Act
            var count = template.GetStatModifierCount();

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public void ComplexScenario_WeaponTemplateWithFullConfiguration()
        {
            // Arrange & Act - Build complex weapon template
            var template = ItemTemplate.CreateWeaponTemplate(
                "Legendary Excalibur", 
                "The legendary sword of King Arthur", 
                1000, 
                EquipmentSlot.MainHand);

            template.AddStatModifier("Strength", 25);
            template.AddStatModifier("Attack", 50);
            template.AddStatModifier("CriticalChance", 15);
            template.AddStatModifier("Holy", 10);

            // Assert
            Assert.Equal("Legendary Excalibur", template.Name);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.Equal(1000, template.Value);
            Assert.True(template.IsEquippable);
            Assert.False(template.IsConsumable);
            Assert.Equal(EquipmentSlot.MainHand, template.EquipmentSlot);
            Assert.Equal(4, template.GetStatModifierCount());
            Assert.Equal(25, template.GetStatModifier("Strength"));
            Assert.Equal(50, template.GetStatModifier("Attack"));
            Assert.Equal(15, template.GetStatModifier("CriticalChance"));
            Assert.Equal(10, template.GetStatModifier("Holy"));
        }

        [Fact]
        public void ComplexScenario_ConsumablePotionWithEffects()
        {
            // Arrange & Act - Build complex potion template
            var template = ItemTemplate.CreatePotionTemplate(
                "Greater Healing Elixir", 
                "A powerful potion that restores health and mana", 
                75);

            template.AddStatModifier("HealthRestore", 100);
            template.AddStatModifier("ManaRestore", 50);
            template.AddStatModifier("Duration", 10);

            // Assert
            Assert.Equal("Greater Healing Elixir", template.Name);
            Assert.Equal(ItemType.Potion, template.ItemType);
            Assert.Equal(75, template.Value);
            Assert.True(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
            Assert.Equal(3, template.GetStatModifierCount());
            Assert.Equal(100, template.GetStatModifier("HealthRestore"));
            Assert.Equal(50, template.GetStatModifier("ManaRestore"));
            Assert.Equal(10, template.GetStatModifier("Duration"));
        }

        #region Enhanced Factory Method Tests

        // Test all ItemType enum values with factory methods
        [Theory]
        [InlineData(ItemType.Weapon)]
        [InlineData(ItemType.Armor)]
        [InlineData(ItemType.Potion)]
        [InlineData(ItemType.Scroll)]
        [InlineData(ItemType.QuestItem)]
        [InlineData(ItemType.Miscellaneous)]
        [InlineData(ItemType.Currency)]
        public void Constructor_AllItemTypeValues_CreatesTemplateWithCorrectType(ItemType itemType)
        {
            // Act
            var template = new ItemTemplate("Test Item", "Test Description", itemType, 100);

            // Assert
            Assert.Equal(itemType, template.ItemType);
            Assert.Equal("Test Item", template.Name);
            Assert.Equal("Test Description", template.Description);
            Assert.Equal(100, template.Value);
        }

        // Test all EquipmentSlot enum values
        [Theory]
        [InlineData(EquipmentSlot.None)]
        [InlineData(EquipmentSlot.Head)]
        [InlineData(EquipmentSlot.Chest)]
        [InlineData(EquipmentSlot.Legs)]
        [InlineData(EquipmentSlot.Feet)]
        [InlineData(EquipmentSlot.Hands)]
        [InlineData(EquipmentSlot.MainHand)]
        [InlineData(EquipmentSlot.OffHand)]
        [InlineData(EquipmentSlot.Neck)]
        [InlineData(EquipmentSlot.Ring)]
        public void CreateWeaponTemplate_AllEquipmentSlots_CreatesWithCorrectSlot(EquipmentSlot slot)
        {
            // Act
            var template = ItemTemplate.CreateWeaponTemplate("Test Weapon", "Description", 100, slot);

            // Assert
            Assert.Equal(slot, template.EquipmentSlot);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.True(template.IsEquippable);
            Assert.False(template.IsConsumable);
        }

        [Theory]
        [InlineData(EquipmentSlot.Head)]
        [InlineData(EquipmentSlot.Chest)]
        [InlineData(EquipmentSlot.Legs)]
        [InlineData(EquipmentSlot.Feet)]
        [InlineData(EquipmentSlot.Hands)]
        [InlineData(EquipmentSlot.Neck)]
        [InlineData(EquipmentSlot.Ring)]
        public void CreateArmorTemplate_AllArmorSlots_CreatesWithCorrectSlot(EquipmentSlot slot)
        {
            // Act
            var template = ItemTemplate.CreateArmorTemplate("Test Armor", "Description", 100, slot);

            // Assert
            Assert.Equal(slot, template.EquipmentSlot);
            Assert.Equal(ItemType.Armor, template.ItemType);
            Assert.True(template.IsEquippable);
            Assert.False(template.IsConsumable);
        }

        [Fact]
        public void CreateWeaponTemplate_WithNullStatModifiers_CreatesValidTemplate()
        {
            // Act
            var template = ItemTemplate.CreateWeaponTemplate("Null Mods Weapon", "No modifiers", 50, 
                EquipmentSlot.MainHand, null);

            // Assert
            Assert.Equal("Null Mods Weapon", template.Name);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.Empty(template.StatModifiers);
            Assert.Equal(0, template.GetStatModifierCount());
        }

        [Fact]
        public void CreateArmorTemplate_WithNullStatModifiers_CreatesValidTemplate()
        {
            // Act
            var template = ItemTemplate.CreateArmorTemplate("Null Mods Armor", "No modifiers", 50, 
                EquipmentSlot.Chest, null);

            // Assert
            Assert.Equal("Null Mods Armor", template.Name);
            Assert.Equal(ItemType.Armor, template.ItemType);
            Assert.Empty(template.StatModifiers);
            Assert.Equal(0, template.GetStatModifierCount());
        }

        [Fact]
        public void CreatePotionTemplate_WithNullEffects_CreatesValidTemplate()
        {
            // Act
            var template = ItemTemplate.CreatePotionTemplate("Null Effects Potion", "No effects", 25, null);

            // Assert
            Assert.Equal("Null Effects Potion", template.Name);
            Assert.Equal(ItemType.Potion, template.ItemType);
            Assert.True(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
            Assert.Empty(template.StatModifiers);
        }

        [Fact]
        public void CreateWeaponTemplate_WithEmptyStatModifiers_CreatesValidTemplate()
        {
            // Arrange
            var emptyModifiers = new Dictionary<string, int>();

            // Act
            var template = ItemTemplate.CreateWeaponTemplate("Empty Mods Weapon", "Empty modifiers", 75, 
                EquipmentSlot.OffHand, emptyModifiers);

            // Assert
            Assert.Equal("Empty Mods Weapon", template.Name);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.Empty(template.StatModifiers);
        }

        [Fact]
        public void CreateArmorTemplate_WithEmptyStatModifiers_CreatesValidTemplate()
        {
            // Arrange
            var emptyModifiers = new Dictionary<string, int>();

            // Act
            var template = ItemTemplate.CreateArmorTemplate("Empty Mods Armor", "Empty modifiers", 75, 
                EquipmentSlot.Legs, emptyModifiers);

            // Assert
            Assert.Equal("Empty Mods Armor", template.Name);
            Assert.Equal(ItemType.Armor, template.ItemType);
            Assert.Empty(template.StatModifiers);
        }

        [Fact]
        public void CreatePotionTemplate_WithEmptyEffects_CreatesValidTemplate()
        {
            // Arrange
            var emptyEffects = new Dictionary<string, int>();

            // Act
            var template = ItemTemplate.CreatePotionTemplate("Empty Effects Potion", "Empty effects", 30, emptyEffects);

            // Assert
            Assert.Equal("Empty Effects Potion", template.Name);
            Assert.Equal(ItemType.Potion, template.ItemType);
            Assert.Empty(template.StatModifiers);
        }

        // Test missing factory methods for Scroll and Currency types
        [Fact]
        public void Constructor_CreateScrollItemManually_CreatesScrollTemplate()
        {
            // Act
            var template = new ItemTemplate("Fireball Scroll", "Casts fireball spell", ItemType.Scroll, 50, 
                true, false, null);

            // Assert
            Assert.Equal("Fireball Scroll", template.Name);
            Assert.Equal(ItemType.Scroll, template.ItemType);
            Assert.True(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
        }

        [Fact]
        public void Constructor_CreateCurrencyItemManually_CreatesCurrencyTemplate()
        {
            // Act
            var template = new ItemTemplate("Gold Piece", "Standard currency", ItemType.Currency, 1, 
                false, false, null);

            // Assert
            Assert.Equal("Gold Piece", template.Name);
            Assert.Equal(ItemType.Currency, template.ItemType);
            Assert.False(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
        }

        #endregion

        #region Advanced Stat Modifier System Tests

        [Theory]
        [InlineData(-100)]
        [InlineData(-50)]
        [InlineData(-1)]
        public void AddStatModifier_NegativeModifiers_AddsNegativeValues(int negativeModifier)
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act
            template.AddStatModifier("Strength", negativeModifier);

            // Assert
            Assert.Equal(negativeModifier, template.GetStatModifier("Strength"));
            Assert.True(template.HasStatModifier("Strength"));
        }

        [Fact]
        public void AddStatModifier_ZeroModifier_AddsZeroValue()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act
            template.AddStatModifier("Strength", 0);

            // Assert
            Assert.Equal(0, template.GetStatModifier("Strength"));
            Assert.True(template.HasStatModifier("Strength"));
        }

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void AddStatModifier_ExtremeValues_HandlesMaximumIntegers(int extremeValue)
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act
            template.AddStatModifier("Extreme", extremeValue);

            // Assert
            Assert.Equal(extremeValue, template.GetStatModifier("Extreme"));
        }

        [Fact]
        public void StatModifiers_SequentialOperations_MaintainsCorrectState()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act - Add multiple modifiers
            template.AddStatModifier("Strength", 10);
            template.AddStatModifier("Dexterity", 15);
            template.AddStatModifier("Intelligence", 20);

            // Remove one
            var removed = template.RemoveStatModifier("Dexterity");

            // Update one
            template.AddStatModifier("Strength", 25);

            // Assert
            Assert.True(removed);
            Assert.Equal(2, template.GetStatModifierCount());
            Assert.Equal(25, template.GetStatModifier("Strength"));
            Assert.Equal(20, template.GetStatModifier("Intelligence"));
            Assert.False(template.HasStatModifier("Dexterity"));
        }

        [Theory]
        [InlineData("STRENGTH")] // All caps
        [InlineData("strength")] // All lowercase
        [InlineData("StReNgTh")] // Mixed case
        public void StatModifiers_CaseSensitivity_TreatsAsDistinctStats(string statName)
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            template.AddStatModifier("Strength", 10);

            // Act
            template.AddStatModifier(statName, 20);

            // Assert
            if (statName == "Strength")
            {
                Assert.Equal(1, template.GetStatModifierCount());
                Assert.Equal(20, template.GetStatModifier("Strength"));
            }
            else
            {
                Assert.Equal(2, template.GetStatModifierCount());
                Assert.Equal(10, template.GetStatModifier("Strength"));
                Assert.Equal(20, template.GetStatModifier(statName));
            }
        }

        [Theory]
        [InlineData("Stat With Spaces")]
        [InlineData("Stat123")]
        [InlineData("Stat_With_Underscores")]
        [InlineData("Stat-With-Hyphens")]
        [InlineData("Stat.With.Dots")]
        [InlineData("Stat@#$%")]
        public void AddStatModifier_SpecialCharacters_AcceptsVariousStatNames(string statName)
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act
            template.AddStatModifier(statName, 100);

            // Assert
            Assert.True(template.HasStatModifier(statName));
            Assert.Equal(100, template.GetStatModifier(statName));
        }

        [Fact]
        public void AddStatModifier_BulkOperations_HandlesLargeNumberOfModifiers()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            const int modifierCount = 100;

            // Act
            for (int i = 0; i < modifierCount; i++)
            {
                template.AddStatModifier($"Stat{i}", i * 10);
            }

            // Assert
            Assert.Equal(modifierCount, template.GetStatModifierCount());
            for (int i = 0; i < modifierCount; i++)
            {
                Assert.Equal(i * 10, template.GetStatModifier($"Stat{i}"));
            }
        }

        [Fact]
        public void RemoveStatModifier_BulkOperations_RemovesMultipleModifiers()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            template.AddStatModifier("Str", 10);
            template.AddStatModifier("Dex", 15);
            template.AddStatModifier("Int", 20);
            template.AddStatModifier("Wis", 25);
            template.AddStatModifier("Con", 30);

            // Act - Remove multiple
            var results = new[]
            {
                template.RemoveStatModifier("Str"),
                template.RemoveStatModifier("Int"),
                template.RemoveStatModifier("Con")
            };

            // Assert
            Assert.All(results, result => Assert.True(result));
            Assert.Equal(2, template.GetStatModifierCount());
            Assert.True(template.HasStatModifier("Dex"));
            Assert.True(template.HasStatModifier("Wis"));
            Assert.False(template.HasStatModifier("Str"));
            Assert.False(template.HasStatModifier("Int"));
            Assert.False(template.HasStatModifier("Con"));
        }

        #endregion

        #region Enhanced Constructor Testing

        [Theory]
        [InlineData(0)] // Minimum valid value
        [InlineData(1)] // Small positive
        [InlineData(999999)] // Large value
        [InlineData(int.MaxValue)] // Maximum valid value
        public void Constructor_BoundaryValues_AcceptsValidValues(int value)
        {
            // Act
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, value);

            // Assert
            Assert.Equal(value, template.Value);
            Assert.NotEqual(Guid.Empty, template.Id);
        }

        [Fact]
        public void Constructor_AllOptionalParameters_UsesCorrectDefaults()
        {
            // Act
            var template = new ItemTemplate("Test Item", "Test Description", ItemType.Miscellaneous, 50);

            // Assert
            Assert.False(template.IsConsumable); // Default false
            Assert.False(template.IsEquippable); // Default false
            Assert.Null(template.EquipmentSlot); // Default null
            Assert.Empty(template.StatModifiers);
        }

        [Fact]
        public void Constructor_EquippableWithSlot_LogicalCombination()
        {
            // Act
            var template = new ItemTemplate("Sword", "Sharp blade", ItemType.Weapon, 100, 
                false, true, EquipmentSlot.MainHand);

            // Assert
            Assert.False(template.IsConsumable);
            Assert.True(template.IsEquippable);
            Assert.Equal(EquipmentSlot.MainHand, template.EquipmentSlot);
        }

        [Fact]
        public void Constructor_ConsumableWithoutSlot_LogicalCombination()
        {
            // Act
            var template = new ItemTemplate("Potion", "Health restore", ItemType.Potion, 25, 
                true, false, null);

            // Assert
            Assert.True(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
        }

        // Test all parameter combinations for edge cases
        [Theory]
        [InlineData(true, true)] // Both consumable and equippable
        [InlineData(true, false)] // Only consumable
        [InlineData(false, true)] // Only equippable
        [InlineData(false, false)] // Neither
        public void Constructor_AllConsumableEquippableCombinations_AcceptsAllCombinations(
            bool isConsumable, bool isEquippable)
        {
            // Act
            var template = new ItemTemplate("Test", "Description", ItemType.Miscellaneous, 50, 
                isConsumable, isEquippable, isEquippable ? EquipmentSlot.MainHand : null);

            // Assert
            Assert.Equal(isConsumable, template.IsConsumable);
            Assert.Equal(isEquippable, template.IsEquippable);
        }

        #endregion

        #region Update Methods - Comprehensive Coverage

        [Fact]
        public void UpdateDetails_SameValues_DoesNotBreakState()
        {
            // Arrange
            var template = new ItemTemplate("Original", "Original Description", ItemType.Weapon, 100, 
                false, true, EquipmentSlot.MainHand);
            var originalId = template.Id;

            // Act - Update with same values
            template.UpdateDetails("Original", "Original Description", ItemType.Weapon, 100, 
                false, true, EquipmentSlot.MainHand);

            // Assert
            Assert.Equal(originalId, template.Id); // ID should remain unchanged
            Assert.Equal("Original", template.Name);
            Assert.Equal("Original Description", template.Description);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.Equal(100, template.Value);
        }

        [Fact]
        public void UpdateDetails_DifferentValues_UpdatesAllProperties()
        {
            // Arrange
            var template = new ItemTemplate("Old", "Old Description", ItemType.Weapon, 100);
            template.AddStatModifier("TestStat", 50); // Add stat to verify it's preserved

            // Act
            template.UpdateDetails("New", "New Description", ItemType.Armor, 200, 
                true, false, null);

            // Assert
            Assert.Equal("New", template.Name);
            Assert.Equal("New Description", template.Description);
            Assert.Equal(ItemType.Armor, template.ItemType);
            Assert.Equal(200, template.Value);
            Assert.True(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
            // Stat modifiers should be preserved
            Assert.Equal(50, template.GetStatModifier("TestStat"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void UpdateValue_BoundaryConditions_UpdatesCorrectly(int newValue)
        {
            // Arrange
            var template = new ItemTemplate("Test", "Description", ItemType.Weapon, 100);

            // Act
            template.UpdateValue(newValue);

            // Assert
            Assert.Equal(newValue, template.Value);
        }

        [Fact]
        public void UpdateBasicDetails_PreservesOtherProperties()
        {
            // Arrange
            var template = new ItemTemplate("Original", "Original Desc", ItemType.Armor, 100, 
                true, true, EquipmentSlot.Chest);
            template.AddStatModifier("Defense", 25);

            // Act
            template.UpdateBasicDetails("Updated", "Updated Desc", 250);

            // Assert
            Assert.Equal("Updated", template.Name);
            Assert.Equal("Updated Desc", template.Description);
            Assert.Equal(250, template.Value);
            // These should remain unchanged
            Assert.Equal(ItemType.Armor, template.ItemType);
            Assert.True(template.IsConsumable);
            Assert.True(template.IsEquippable);
            Assert.Equal(EquipmentSlot.Chest, template.EquipmentSlot);
            Assert.Equal(25, template.GetStatModifier("Defense"));
        }

        [Fact]
        public void ChainedUpdates_MultipleUpdatesInSequence_MaintainsCorrectState()
        {
            // Arrange
            var template = new ItemTemplate("Original", "Original Desc", ItemType.Weapon, 100);

            // Act - Chain multiple updates
            template.UpdateBasicDetails("Step1", "Step1 Desc", 150);
            template.UpdateValue(200);
            template.UpdateDetails("Final", "Final Desc", ItemType.Armor, 300, true, false, null);

            // Assert
            Assert.Equal("Final", template.Name);
            Assert.Equal("Final Desc", template.Description);
            Assert.Equal(ItemType.Armor, template.ItemType);
            Assert.Equal(300, template.Value);
            Assert.True(template.IsConsumable);
            Assert.False(template.IsEquippable);
            Assert.Null(template.EquipmentSlot);
        }

        #endregion

        #region Integration and Real-World Scenarios

        [Fact]
        public void ComplexWeaponBuild_LegendaryWeaponWithMultipleModifiers_CreatesCompleteWeapon()
        {
            // Arrange & Act
            var template = ItemTemplate.CreateWeaponTemplate(
                "Dragonslayer Greatsword",
                "A massive two-handed sword forged from dragon bone and blessed by ancient magic. The blade glows with inner fire and thirsts for draconic blood.",
                5000,
                EquipmentSlot.MainHand);

            // Add complex stat modifications
            template.AddStatModifier("PhysicalDamage", 85);
            template.AddStatModifier("FireDamage", 45);
            template.AddStatModifier("CriticalChance", 12);
            template.AddStatModifier("CriticalMultiplier", 250);
            template.AddStatModifier("DragonSlaying", 300);
            template.AddStatModifier("Strength", 15);
            template.AddStatModifier("RequiredLevel", 45);
            template.AddStatModifier("Durability", 500);

            // Assert
            Assert.Equal("Dragonslayer Greatsword", template.Name);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.Equal(5000, template.Value);
            Assert.True(template.IsEquippable);
            Assert.False(template.IsConsumable);
            Assert.Equal(EquipmentSlot.MainHand, template.EquipmentSlot);
            Assert.Equal(8, template.GetStatModifierCount());
            Assert.Equal(85, template.GetStatModifier("PhysicalDamage"));
            Assert.Equal(300, template.GetStatModifier("DragonSlaying"));
        }

        [Fact]
        public void ArmorSet_CompleteArmorPiecesWithDifferentSlots_CreatesFullSet()
        {
            // Arrange & Act - Create complete armor set
            var helmet = ItemTemplate.CreateArmorTemplate("Dragon Scale Helm", "Protective headgear", 
                500, EquipmentSlot.Head, new Dictionary<string, int> { { "Defense", 25 }, { "FireResistance", 15 } });

            var chestplate = ItemTemplate.CreateArmorTemplate("Dragon Scale Mail", "Heavy chest protection", 
                800, EquipmentSlot.Chest, new Dictionary<string, int> { { "Defense", 45 }, { "FireResistance", 25 } });

            var gauntlets = ItemTemplate.CreateArmorTemplate("Dragon Scale Gauntlets", "Protective gloves", 
                300, EquipmentSlot.Hands, new Dictionary<string, int> { { "Defense", 15 }, { "FireResistance", 10 } });

            var boots = ItemTemplate.CreateArmorTemplate("Dragon Scale Boots", "Protective footwear", 
                350, EquipmentSlot.Feet, new Dictionary<string, int> { { "Defense", 18 }, { "FireResistance", 12 } });

            // Assert set properties
            var armorPieces = new[] { helmet, chestplate, gauntlets, boots };
            Assert.All(armorPieces, piece => Assert.Equal(ItemType.Armor, piece.ItemType));
            Assert.All(armorPieces, piece => Assert.True(piece.IsEquippable));
            Assert.All(armorPieces, piece => Assert.False(piece.IsConsumable));

            // Verify total set bonuses
            var totalDefense = armorPieces.Sum(p => p.GetStatModifier("Defense"));
            var totalFireResistance = armorPieces.Sum(p => p.GetStatModifier("FireResistance"));
            Assert.Equal(103, totalDefense);
            Assert.Equal(62, totalFireResistance);
        }

        [Fact]
        public void ConsumableStack_VariousPotionTypes_CreatesEffectiveConsumables()
        {
            // Arrange & Act - Create various consumables
            var healingPotion = ItemTemplate.CreatePotionTemplate("Major Healing Potion", 
                "Restores significant health", 50, 
                new Dictionary<string, int> { { "HealthRestore", 150 } });

            var manaPotion = ItemTemplate.CreatePotionTemplate("Greater Mana Elixir", 
                "Restores mana and increases regeneration", 75, 
                new Dictionary<string, int> { { "ManaRestore", 200 }, { "ManaRegen", 25 } });

            var buffPotion = ItemTemplate.CreatePotionTemplate("Elixir of Giant Strength", 
                "Temporarily increases physical power", 100, 
                new Dictionary<string, int> { { "StrengthBonus", 10 }, { "Duration", 300 } });

            // Assert consumable properties
            var potions = new[] { healingPotion, manaPotion, buffPotion };
            Assert.All(potions, potion => Assert.Equal(ItemType.Potion, potion.ItemType));
            Assert.All(potions, potion => Assert.True(potion.IsConsumable));
            Assert.All(potions, potion => Assert.False(potion.IsEquippable));
            Assert.All(potions, potion => Assert.Null(potion.EquipmentSlot));

            // Verify specific effects
            Assert.Equal(150, healingPotion.GetStatModifier("HealthRestore"));
            Assert.Equal(200, manaPotion.GetStatModifier("ManaRestore"));
            Assert.Equal(10, buffPotion.GetStatModifier("StrengthBonus"));
        }

        [Fact]
        public void QuestItemWorkflow_ItemPropertiesChangeOverTime_HandlesQuestProgression()
        {
            // Arrange - Create quest item
            var template = ItemTemplate.CreateQuestItemTemplate("Mysterious Orb", "A glowing orb of unknown origin");

            // Act - Simulate quest progression
            // Step 1: Initial discovery
            Assert.Equal(0, template.Value); // Initially worthless
            Assert.False(template.IsEquippable);

            // Step 2: Research reveals magical properties
            template.UpdateBasicDetails("Orb of Ancient Power", 
                "An orb containing immense magical energy, revealed through careful study", 1000);
            template.AddStatModifier("MagicPower", 25);
            template.AddStatModifier("WisdomRequired", 20);

            // Step 3: Quest completion transforms it into usable item
            template.UpdateDetails("Staff of the Archmage", 
                "The orb has been mounted on an ancient staff, creating a powerful artifact", 
                ItemType.Weapon, 5000, false, true, EquipmentSlot.MainHand);
            template.AddStatModifier("SpellDamage", 50);

            // Assert final state
            Assert.Equal("Staff of the Archmage", template.Name);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.Equal(5000, template.Value);
            Assert.True(template.IsEquippable);
            Assert.Equal(EquipmentSlot.MainHand, template.EquipmentSlot);
            Assert.Equal(3, template.GetStatModifierCount());
            Assert.Equal(50, template.GetStatModifier("SpellDamage"));
        }

        [Fact]
        public void EconomicScenarios_CurrencyAndValuableItems_HandlesTrading()
        {
            // Arrange & Act - Create economic items
            var goldCoin = new ItemTemplate("Gold Coin", "Standard currency", ItemType.Currency, 1);
            var silverCoin = new ItemTemplate("Silver Coin", "Lesser currency", ItemType.Currency, 1);
            var gemstone = ItemTemplate.CreateMiscellaneousTemplate("Ruby Gemstone", 
                "A precious red gem", 500, false);
            var artItem = ItemTemplate.CreateMiscellaneousTemplate("Ancient Vase", 
                "A valuable historical artifact", 2000, false);

            // Assert economic properties
            Assert.Equal(ItemType.Currency, goldCoin.ItemType);
            Assert.Equal(ItemType.Currency, silverCoin.ItemType);
            Assert.Equal(ItemType.Miscellaneous, gemstone.ItemType);
            Assert.Equal(ItemType.Miscellaneous, artItem.ItemType);

            // Verify value relationships
            Assert.True(artItem.Value > gemstone.Value);
            Assert.True(gemstone.Value > goldCoin.Value);
            Assert.Equal(goldCoin.Value, silverCoin.Value); // Same base currency value
        }

        [Fact]
        public void EquipmentCombinations_RealisticLoadout_CreatesValidEquipmentSet()
        {
            // Arrange & Act - Create realistic character loadout
            var weapon = ItemTemplate.CreateWeaponTemplate("Iron Longsword", "Standard warrior weapon", 
                150, EquipmentSlot.MainHand, new Dictionary<string, int> { { "Attack", 25 } });

            var shield = ItemTemplate.CreateArmorTemplate("Oak Shield", "Wooden defensive shield", 
                75, EquipmentSlot.OffHand, new Dictionary<string, int> { { "Defense", 15 }, { "BlockChance", 10 } });

            var armor = ItemTemplate.CreateArmorTemplate("Chainmail Hauberk", "Flexible metal armor", 
                300, EquipmentSlot.Chest, new Dictionary<string, int> { { "Defense", 30 } });

            var amulet = ItemTemplate.CreateMiscellaneousTemplate("Amulet of Vigor", 
                "Increases health regeneration", 200, false);
            // Make amulet equippable after creation
            amulet.UpdateDetails(amulet.Name, amulet.Description, ItemType.Miscellaneous, 
                amulet.Value, false, true, EquipmentSlot.Neck);
            amulet.AddStatModifier("HealthRegen", 5);

            var ring = ItemTemplate.CreateMiscellaneousTemplate("Ring of Power", 
                "Increases magical abilities", 150, false);
            ring.UpdateDetails(ring.Name, ring.Description, ItemType.Miscellaneous, 
                ring.Value, false, true, EquipmentSlot.Ring);
            ring.AddStatModifier("MagicPower", 8);

            // Assert equipment set validity
            var equipment = new[] { weapon, shield, armor, amulet, ring };
            Assert.All(equipment, item => Assert.True(item.IsEquippable));

            // Verify no slot conflicts (all different slots)
            var slots = equipment.Select(e => e.EquipmentSlot).ToList();
            Assert.Equal(5, slots.Distinct().Count()); // All unique slots

            // Calculate total bonuses
            var totalDefense = equipment.Sum(e => e.GetStatModifier("Defense"));
            Assert.Equal(45, totalDefense); // Shield + Armor
        }

        #endregion

        #region Edge Cases and Error Conditions

        [Fact]
        public void StatModifiers_MaximumLoadTesting_Handles1000Modifiers()
        {
            // Arrange
            var template = new ItemTemplate("Overloaded Item", "Too many stats", ItemType.Miscellaneous, 1);
            const int maxModifiers = 1000;

            // Act - Add 1000 stat modifiers
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < maxModifiers; i++)
            {
                template.AddStatModifier($"Stat{i:D4}", i);
            }
            stopwatch.Stop();

            // Assert
            Assert.Equal(maxModifiers, template.GetStatModifierCount());
            Assert.True(stopwatch.ElapsedMilliseconds < 1000); // Should be fast

            // Verify random sampling of modifiers
            Assert.Equal(0, template.GetStatModifier("Stat0000"));
            Assert.Equal(500, template.GetStatModifier("Stat0500"));
            Assert.Equal(999, template.GetStatModifier("Stat0999"));
        }

        [Fact]
        public void StatModifiers_ConcurrentModifications_MaintainsConsistency()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);

            // Act - Simulate concurrent operations
            template.AddStatModifier("Stat1", 10);
            template.AddStatModifier("Stat2", 20);
            template.RemoveStatModifier("Stat1");
            template.AddStatModifier("Stat3", 30);
            template.AddStatModifier("Stat2", 25); // Update existing
            template.RemoveStatModifier("NonExistent"); // Remove non-existent

            // Assert
            Assert.Equal(2, template.GetStatModifierCount());
            Assert.False(template.HasStatModifier("Stat1"));
            Assert.Equal(25, template.GetStatModifier("Stat2"));
            Assert.Equal(30, template.GetStatModifier("Stat3"));
        }

        [Theory]
        [InlineData(10000)] // Very long string
        [InlineData(100000)] // Extremely long string
        public void StringHandling_VeryLongStrings_HandlesSafely(int length)
        {
            // Arrange
            var longName = new string('A', Math.Min(length, 50000)); // Cap for sanity
            var longDescription = new string('B', Math.Min(length, 50000));
            var longStatName = new string('C', Math.Min(length, 10000));

            // Act
            var template = new ItemTemplate(longName, longDescription, ItemType.Miscellaneous, 1);
            template.AddStatModifier(longStatName, 100);

            // Assert
            Assert.Equal(longName, template.Name);
            Assert.Equal(longDescription, template.Description);
            Assert.True(template.HasStatModifier(longStatName));
            Assert.Equal(100, template.GetStatModifier(longStatName));
        }

        [Fact]
        public void StateConsistency_AfterFailedOperations_MaintainsValidState()
        {
            // Arrange
            var template = new ItemTemplate("Test Item", "Description", ItemType.Weapon, 100);
            template.AddStatModifier("OriginalStat", 50);
            var originalCount = template.GetStatModifierCount();

            // Act - Attempt operations that should fail
            try { template.UpdateValue(-1); } catch { /* Expected */ }
            try { template.UpdateBasicDetails("", "desc", 100); } catch { /* Expected */ }
            try { template.AddStatModifier("", 10); } catch { /* Expected */ }

            // Assert - State should be unchanged
            Assert.Equal("Test Item", template.Name);
            Assert.Equal(100, template.Value);
            Assert.Equal(originalCount, template.GetStatModifierCount());
            Assert.Equal(50, template.GetStatModifier("OriginalStat"));
        }

        [Theory]
        [InlineData(ItemType.Weapon, EquipmentSlot.MainHand, true)] // Logical: weapon in main hand
        [InlineData(ItemType.Weapon, EquipmentSlot.OffHand, true)] // Logical: weapon in off hand
        [InlineData(ItemType.Armor, EquipmentSlot.Chest, true)] // Logical: armor on chest
        [InlineData(ItemType.Potion, EquipmentSlot.MainHand, false)] // Illogical: potion equipped
        [InlineData(ItemType.QuestItem, EquipmentSlot.Ring, false)] // Illogical: quest item as ring
        [InlineData(ItemType.Currency, EquipmentSlot.Head, false)] // Illogical: currency as helmet
        public void LogicalCombinations_ItemTypeAndEquipmentSlot_ValidatesExpectedBehavior(
            ItemType itemType, EquipmentSlot slot, bool shouldBeLogical)
        {
            // Act - Create item with potentially illogical combination
            var template = new ItemTemplate("Test", "Description", itemType, 100, 
                false, true, slot);

            // Assert - All combinations are technically allowed by the system
            // The logic validation would be handled at a higher application layer
            Assert.Equal(itemType, template.ItemType);
            Assert.Equal(slot, template.EquipmentSlot);
            Assert.True(template.IsEquippable); // All were created as equippable

            // Note: This test documents current behavior - 
            // business logic validation would be in application services
        }

        #endregion

        #region Performance and Scale Testing

        [Fact]
        public void Performance_BulkStatModifierOperations_CompletesWithinReasonableTime()
        {
            // Arrange
            var template = new ItemTemplate("Performance Test", "Description", ItemType.Miscellaneous, 1);
            const int operationCount = 10000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act - Perform bulk operations
            for (int i = 0; i < operationCount; i++)
            {
                template.AddStatModifier($"BulkStat{i}", i);
            }

            for (int i = 0; i < operationCount / 2; i++)
            {
                template.RemoveStatModifier($"BulkStat{i}");
            }

            stopwatch.Stop();

            // Assert
            Assert.Equal(operationCount / 2, template.GetStatModifierCount());
            Assert.True(stopwatch.ElapsedMilliseconds < 5000); // Should complete in under 5 seconds
        }

        [Fact]
        public void MemoryUsage_LargeDictionaries_HandlesEfficientlyWithoutLeak()
        {
            // Arrange
            var templates = new List<ItemTemplate>();
            const int templateCount = 100;
            const int modifiersPerTemplate = 100;

            // Act - Create many templates with many modifiers
            for (int t = 0; t < templateCount; t++)
            {
                var template = new ItemTemplate($"Template{t}", "Description", ItemType.Miscellaneous, 1);
                for (int m = 0; m < modifiersPerTemplate; m++)
                {
                    template.AddStatModifier($"T{t}M{m}", m);
                }
                templates.Add(template);
            }

            // Assert
            Assert.Equal(templateCount, templates.Count);
            Assert.All(templates, t => Assert.Equal(modifiersPerTemplate, t.GetStatModifierCount()));

            // Cleanup test - ensure clearing works
            foreach (var template in templates)
            {
                template.ClearStatModifiers();
                Assert.Equal(0, template.GetStatModifierCount());
            }
        }

        #endregion

        #region Complex Interaction Tests

        [Fact]
        public void FullItemLifecycle_CreateModifyUpdateClearValidate_CompletesSuccessfully()
        {
            // Arrange & Act - Complete lifecycle
            // Step 1: Create via factory
            var template = ItemTemplate.CreateWeaponTemplate("Lifecycle Weapon", "Test weapon", 100, 
                EquipmentSlot.MainHand, new Dictionary<string, int> { { "InitialStat", 10 } });

            // Step 2: Add more modifiers
            template.AddStatModifier("AddedStat", 20);
            template.AddStatModifier("AnotherStat", 30);

            // Step 3: Update properties
            template.UpdateBasicDetails("Updated Lifecycle Weapon", "Updated description", 200);

            // Step 4: Modify existing modifiers
            template.AddStatModifier("InitialStat", 15); // Update existing
            template.RemoveStatModifier("AddedStat"); // Remove one

            // Step 5: Full update
            template.UpdateDetails("Final Weapon", "Final description", ItemType.Weapon, 300, 
                false, true, EquipmentSlot.OffHand);

            // Step 6: Add final modifiers
            template.AddStatModifier("FinalStat", 40);

            // Assert final state
            Assert.Equal("Final Weapon", template.Name);
            Assert.Equal("Final description", template.Description);
            Assert.Equal(ItemType.Weapon, template.ItemType);
            Assert.Equal(300, template.Value);
            Assert.Equal(EquipmentSlot.OffHand, template.EquipmentSlot);
            Assert.Equal(3, template.GetStatModifierCount()); // InitialStat, AnotherStat, FinalStat
            Assert.Equal(15, template.GetStatModifier("InitialStat"));
            Assert.Equal(30, template.GetStatModifier("AnotherStat"));
            Assert.Equal(40, template.GetStatModifier("FinalStat"));
            Assert.False(template.HasStatModifier("AddedStat")); // Removed

            // Step 7: Clear all modifiers
            var cleared = template.ClearStatModifiers();
            Assert.True(cleared);
            Assert.Equal(0, template.GetStatModifierCount());
        }

        [Fact]
        public void MixedOperations_FactoryCreationPlusManualModifications_WorksTogether()
        {
            // Arrange - Create via factory with initial stats
            var initialStats = new Dictionary<string, int>
            {
                { "FactoryStat1", 10 },
                { "FactoryStat2", 20 }
            };

            // Act - Factory creation
            var template = ItemTemplate.CreateArmorTemplate("Mixed Operations Armor", "Test armor", 
                150, EquipmentSlot.Chest, initialStats);

            // Manual modifications
            template.AddStatModifier("ManualStat1", 30);
            template.AddStatModifier("ManualStat2", 40);
            template.AddStatModifier("FactoryStat1", 15); // Update factory stat

            // Assert
            Assert.Equal(ItemType.Armor, template.ItemType);
            Assert.Equal(4, template.GetStatModifierCount());
            Assert.Equal(15, template.GetStatModifier("FactoryStat1")); // Updated
            Assert.Equal(20, template.GetStatModifier("FactoryStat2")); // Original
            Assert.Equal(30, template.GetStatModifier("ManualStat1")); // Added manually
            Assert.Equal(40, template.GetStatModifier("ManualStat2")); // Added manually
        }

        [Fact]
        public void PropertyInterdependencies_EquipmentAndConsumableLogic_ValidatesCorrectly()
        {
            // Arrange & Act - Test various logical combinations
            var equippableConsumable = new ItemTemplate("Magic Scroll", "Consumable spell scroll", 
                ItemType.Scroll, 50, true, true, EquipmentSlot.OffHand);

            var nonEquippableWeapon = new ItemTemplate("Broken Sword", "Unusable weapon", 
                ItemType.Weapon, 1, false, false, null);

            var valuelessQuestItem = ItemTemplate.CreateQuestItemTemplate("Worthless Key", 
                "Has no monetary value but important for quest", 0);

            // Assert - All combinations are allowed (business rules would be enforced elsewhere)
            Assert.Equal(ItemType.Scroll, equippableConsumable.ItemType);
            Assert.True(equippableConsumable.IsConsumable && equippableConsumable.IsEquippable);

            Assert.Equal(ItemType.Weapon, nonEquippableWeapon.ItemType);
            Assert.False(nonEquippableWeapon.IsEquippable);

            Assert.Equal(0, valuelessQuestItem.Value);
            Assert.Equal(ItemType.QuestItem, valuelessQuestItem.ItemType);
        }

        #endregion
    }
}