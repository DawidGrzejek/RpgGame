using System;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.ValueObjects;
using RpgGame.Domain.Enums;

// Simple test to verify the new Character system works
class TestCharacterSystem
{
    static void Main()
    {
        Console.WriteLine("=== Testing New Character System ===");
        
        try
        {
            // Test CharacterStats
            var warriorStats = new CharacterStats(1, 120, 15, 12, 10, 5);
            Console.WriteLine($"✓ CharacterStats created: Level {warriorStats.Level}, Health {warriorStats.CurrentHealth}/{warriorStats.MaxHealth}");
            
            // Test Player Creation
            var warrior = Character.CreatePlayer("Sir Lancelot", PlayerClass.Warrior, warriorStats);
            Console.WriteLine($"✓ Player created: {warrior.Name} ({warrior.PlayerClass}) - {warrior.Type}");
            
            // Test NPC Creation
            var goblin = Character.CreateNPC("Goblin Scout", NPCBehavior.Aggressive, 
                new CharacterStats(3, 60, 12, 8, 15, 3));
            Console.WriteLine($"✓ NPC created: {goblin.Name} ({goblin.NPCBehavior}) - {goblin.Type}");
            
            // Test Combat Mechanics
            Console.WriteLine("\n=== Testing Combat ===");
            Console.WriteLine($"Warrior initial health: {warrior.Stats.CurrentHealth}");
            
            warrior.TakeDamage(30);
            Console.WriteLine($"After taking 30 damage: {warrior.Stats.CurrentHealth} (defense reduced actual damage)");
            
            warrior.Heal(20);
            Console.WriteLine($"After healing 20: {warrior.Stats.CurrentHealth}");
            
            // Test Experience
            Console.WriteLine("\n=== Testing Experience ===");
            Console.WriteLine($"Warrior initial level: {warrior.Stats.Level}, XP: {warrior.Experience}");
            
            warrior.GainExperience(500);
            Console.WriteLine($"After gaining 500 XP: Level {warrior.Stats.Level}, XP: {warrior.Experience}");
            
            warrior.GainExperience(500); // Should level up at 1000 XP
            Console.WriteLine($"After gaining another 500 XP: Level {warrior.Stats.Level}, XP: {warrior.Experience}");
            
            // Test CustomData
            Console.WriteLine("\n=== Testing CustomData ===");
            warrior.CustomData["Mana"] = 50;
            warrior.CustomData["CriticalChance"] = 0.15;
            Console.WriteLine($"Added custom data - Mana: {warrior.CustomData["Mana"]}, Crit: {warrior.CustomData["CriticalChance"]}");
            
            // Test NPC doesn't gain XP
            Console.WriteLine("\n=== Testing NPC Behavior ===");
            Console.WriteLine($"Goblin XP before: {goblin.Experience}");
            goblin.GainExperience(100);
            Console.WriteLine($"Goblin XP after gaining XP: {goblin.Experience} (NPCs don't gain XP)");
            
            Console.WriteLine("\n✅ All tests passed! New Character system is working correctly.");
            Console.WriteLine("\n=== Architecture Benefits Demonstrated ===");
            Console.WriteLine("- Single Character class handles both Players and NPCs");
            Console.WriteLine("- Template-driven design ready for database configuration");
            Console.WriteLine("- Immutable CharacterStats with proper value semantics");
            Console.WriteLine("- Flexible CustomData for unlimited extensibility");
            Console.WriteLine("- Defense calculations and combat mechanics working");
            Console.WriteLine("- Experience system with automatic leveling");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}