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
    }
}