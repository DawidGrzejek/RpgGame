using RpgGame.Domain.ValueObjects;
using System;
using Xunit;

namespace RpgGame.UnitTests.Domain.ValueObjects
{
    public class CharacterStatsTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesCharacterStats()
        {
            // Arrange
            var level = 5;
            var maxHealth = 150;
            var strength = 20;
            var defense = 15;
            var speed = 18;
            var magic = 12;

            // Act
            var stats = new CharacterStats(level, maxHealth, strength, defense, speed, magic);

            // Assert
            Assert.Equal(level, stats.Level);
            Assert.Equal(maxHealth, stats.MaxHealth);
            Assert.Equal(maxHealth, stats.CurrentHealth); // Should start at max
            Assert.Equal(strength, stats.Strength);
            Assert.Equal(defense, stats.Defense);
            Assert.Equal(speed, stats.Speed);
            Assert.Equal(magic, stats.Magic);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Constructor_InvalidLevel_ThrowsArgumentException(int invalidLevel)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new CharacterStats(invalidLevel, 100, 10, 8, 12, 5));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Constructor_InvalidMaxHealth_ThrowsArgumentException(int invalidMaxHealth)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new CharacterStats(1, invalidMaxHealth, 10, 8, 12, 5));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Constructor_NegativeStats_ThrowsArgumentException(int negativeStat)
        {
            // Act & Assert for Strength
            var statsStrength = new CharacterStats(1, 100, negativeStat, 8, 12, 5);
            Assert.Equal(1, statsStrength.Strength);

            // Act & Assert for Defense
            var statsDefense = new CharacterStats(1, 100, 10, negativeStat, 12, 5);
            Assert.Equal(0, statsDefense.Defense);

            // Act & Assert for Speed
            var statsSpeed = new CharacterStats(1, 100, 10, 8, negativeStat, 5);
            Assert.Equal(1, statsSpeed.Speed);

            // Act & Assert for Magic
            var statsMagic = new CharacterStats(1, 100, 10, 8, 12, negativeStat);
            Assert.Equal(0, statsMagic.Magic);
        }

        [Fact]
        public void WithHealth_ValidHealth_ReturnsNewStatsWithUpdatedHealth()
        {
            // Arrange
            var originalStats = new CharacterStats(1, 100, 10, 8, 12, 5);
            var newHealth = 75;

            // Act
            var updatedStats = originalStats.WithHealth(newHealth);

            // Assert
            Assert.Equal(newHealth, updatedStats.CurrentHealth);
            Assert.Equal(originalStats.MaxHealth, updatedStats.MaxHealth);
            Assert.Equal(originalStats.Level, updatedStats.Level);
            Assert.Equal(originalStats.Strength, updatedStats.Strength);
            
            // Original should be unchanged (immutable)
            Assert.Equal(100, originalStats.CurrentHealth);
        }

        [Fact]
        public void WithHealth_HealthAboveMax_CapsAtMaxHealth()
        {
            // Arrange
            var stats = new CharacterStats(1, 100, 10, 8, 12, 5);
            var excessiveHealth = 150;

            // Act
            var updatedStats = stats.WithHealth(excessiveHealth);

            // Assert
            Assert.Equal(100, updatedStats.CurrentHealth); // Capped at max
            Assert.Equal(100, updatedStats.MaxHealth);
        }

        [Fact]
        public void WithHealth_NegativeHealth_SetsToZero()
        {
            // Arrange
            var stats = new CharacterStats(1, 100, 10, 8, 12, 5);
            var negativeHealth = -10;

            // Act
            var updatedStats = stats.WithHealth(negativeHealth);

            // Assert
            Assert.Equal(0, updatedStats.CurrentHealth);
        }

        [Fact]
        public void LevelUp_IncreasesLevelAndStats()
        {
            // Arrange
            var stats = new CharacterStats(1, 100, 10, 8, 12, 5);

            // Act
            var leveledStats = stats.LevelUp();

            // Assert
            Assert.Equal(2, leveledStats.Level);
            Assert.True(leveledStats.MaxHealth > stats.MaxHealth);
            Assert.True(leveledStats.Strength > stats.Strength);
            Assert.True(leveledStats.Defense > stats.Defense);
            Assert.True(leveledStats.Speed > stats.Speed);
            Assert.True(leveledStats.Magic > stats.Magic);
            
            // Health should be full after level up
            Assert.Equal(leveledStats.MaxHealth, leveledStats.CurrentHealth);
        }

        [Fact]
        public void LevelUp_MultipleTimesIncreasesCorrectly()
        {
            // Arrange
            var stats = new CharacterStats(1, 100, 10, 8, 12, 5);

            // Act
            var level2Stats = stats.LevelUp();
            var level3Stats = level2Stats.LevelUp();

            // Assert
            Assert.Equal(3, level3Stats.Level);
            Assert.True(level3Stats.MaxHealth > level2Stats.MaxHealth);
            Assert.True(level3Stats.Strength > level2Stats.Strength);
        }

        // NOTE: TakeDamage and Heal are business actions tested in Character entity tests
        // CharacterStats (value object) should only contain pure calculations per DDD principles

        [Fact]
        public void IsAlive_HealthAboveZero_ReturnsTrue()
        {
            // Arrange
            var stats = new CharacterStats(1, 100, 10, 8, 12, 5)
                .WithHealth(1); // Minimum alive health

            // Act & Assert
            Assert.True(stats.IsAlive);
        }

        [Fact]
        public void IsAlive_HealthZero_ReturnsFalse()
        {
            // Arrange
            var stats = new CharacterStats(1, 100, 10, 8, 12, 5)
                .WithHealth(0);

            // Act & Assert
            Assert.False(stats.IsAlive);
        }

        [Fact]
        public void Equality_SameValues_AreEqual()
        {
            // Arrange
            var stats1 = new CharacterStats(5, 150, 20, 15, 18, 12);
            var stats2 = new CharacterStats(5, 150, 20, 15, 18, 12);

            // Act & Assert
            Assert.Equal(stats1, stats2);
            Assert.True(stats1 == stats2);
            Assert.False(stats1 != stats2);
            Assert.Equal(stats1.GetHashCode(), stats2.GetHashCode());
        }

        [Fact]
        public void Equality_DifferentValues_AreNotEqual()
        {
            // Arrange
            var stats1 = new CharacterStats(5, 150, 20, 15, 18, 12);
            var stats2 = new CharacterStats(5, 150, 21, 15, 18, 12); // Different strength

            // Act & Assert
            Assert.NotEqual(stats1, stats2);
            Assert.False(stats1 == stats2);
            Assert.True(stats1 != stats2);
        }

        [Fact]
        public void Immutability_ModifyingReturnsNewInstance()
        {
            // Arrange
            var originalStats = new CharacterStats(1, 100, 10, 8, 12, 5);

            // Act
            var modifiedStats = originalStats.LevelUp();

            // Assert
            Assert.NotSame(originalStats, modifiedStats);
            Assert.Equal(1, originalStats.Level); // Original unchanged
            Assert.Equal(2, modifiedStats.Level); // New instance changed
        }

        [Fact]
        public void ToString_ReturnsReadableFormat()
        {
            // Arrange
            var stats = new CharacterStats(5, 150, 20, 15, 18, 12)
                .WithHealth(120);

            // Act
            var result = stats.ToString();

            // Assert
            Assert.Contains("Level: 5", result);
            Assert.Contains("Health: 120/150", result);
            Assert.Contains("Strength: 20", result);
            Assert.Contains("Defense: 15", result);
            Assert.Contains("Speed: 18", result);
            Assert.Contains("Magic: 12", result);
        }
    }
}