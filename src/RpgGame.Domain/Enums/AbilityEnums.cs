namespace RpgGame.Domain.Enums
{
    public enum AbilityType
    {
        Active,
        Passive,
        Toggle,
        Channeled
    }

    public enum TargetType
    {
        Self,
        SingleEnemy,
        SingleAlly,
        AllEnemies,
        AllAllies,
        Area,
        None
    }

    public enum EffectType
    {
        // Damage effects
        PhysicalDamage,
        MagicalDamage,
        FireDamage,
        IceDamage,
        PoisonDamage,
        
        // Healing effects
        InstantHeal,
        HealOverTime,
        
        // Stat modifications
        StrengthBoost,
        DefenseBoost,
        SpeedBoost,
        MagicBoost,
        
        // Status effects
        Stun,
        Poison,
        Burn,
        Freeze,
        Regeneration,
        
        // Special effects
        Teleport,
        Invisibility,
        Shield,
        Resurrection
    }

    public enum NPCBehavior
    {
        Aggressive,
        Defensive,
        Passive,
        Friendly,
        Vendor,
        QuestGiver,
        Guard,
        Patrol
    }

    public enum CharacterType
    {
        Player,
        NPC
    }

    public enum PlayerClass
    {
        Warrior,
        Mage,
        Rogue,
        Archer,
        Paladin,
        Necromancer
    }
}