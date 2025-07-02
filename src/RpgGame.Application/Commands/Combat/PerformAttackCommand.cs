using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Commands.Results;
using RpgGame.Application.Events;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.NPC.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Combat
{
    public class PerformAttackCommand : ICommand<OperationResult<CombatResult>>
    {
        public Guid AttackerId { get; set; }
        public Guid DefenderId { get; set; }
    }

    public class PerformAttackCommandValidator : AbstractValidator<PerformAttackCommand>
    {
        public PerformAttackCommandValidator()
        {
            RuleFor(x => x.AttackerId)
                .NotEmpty()
                .WithMessage("Attacker ID is required");

            RuleFor(x => x.DefenderId)
                .NotEmpty()
                .WithMessage("Defender ID is required");

            RuleFor(x => x.AttackerId)
                .NotEqual(x => x.DefenderId)
                .WithMessage("Attacker and Defender cannot be the same character");
        }
    }
    public class PerformAttackCommandHandler : IRequestHandler<PerformAttackCommand, OperationResult<CombatResult>>
    {
        private readonly IEventSourcingService _eventSourcingService;
        private readonly ILogger<PerformAttackCommandHandler> _logger;

        public PerformAttackCommandHandler(
            IEventSourcingService eventSourcingService,
            ILogger<PerformAttackCommandHandler> logger)
        {
            _eventSourcingService = eventSourcingService;
            _logger = logger;
        }

        public async Task<OperationResult<CombatResult>> Handle(PerformAttackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get attacker and defender
                var attacker = await _eventSourcingService.GetByIdAsync<Character>(request.AttackerId);
                var defender = await _eventSourcingService.GetByIdAsync<Character>(request.DefenderId);

                if (attacker == null)
                    return OperationResult<CombatResult>.NotFound("Attacker", request.AttackerId.ToString());

                if (defender == null)
                    return OperationResult<CombatResult>.NotFound("Defender", request.DefenderId.ToString());

                // Validate combat state
                if (!attacker.IsAlive)
                    return OperationResult<CombatResult>.BusinessRuleViolation("DeadAttacker", "Dead characters cannot attack");

                if (!defender.IsAlive)
                    return OperationResult<CombatResult>.BusinessRuleViolation("DeadDefender", "Cannot attack dead characters");

                // Perform the attack
                var attackerHealthBefore = attacker.Health;
                var defenderHealthBefore = defender.Health;

                // Calculate damage (this would be more complex in a real game)
                var attackerDamage = CalculateDamage(attacker, defender);

                // Apply damage to defender
                defender.TakeDamage(attackerDamage);

                // Check if defender died
                bool defenderDefeated = !defender.IsAlive;
                int experienceGained = 0;
                var itemsDropped = new List<string>();

                if (defenderDefeated && defender is Enemy enemy)
                {
                    experienceGained = enemy.ExperienceReward;

                    // Award experience to attacker if it's a player
                    if (attacker is PlayerCharacter playerAttacker)
                    {
                        playerAttacker.GainExperience(experienceGained);
                    }

                    // Handle loot drops
                    var droppedItem = enemy.DropLoot();
                    if (droppedItem != null)
                    {
                        itemsDropped.Add(droppedItem.Name);
                    }
                }

                // Defender counter-attacks if still alive and is not a player
                int defenderDamage = 0;
                bool attackerDefeated = false;

                if (!defenderDefeated && defender is Enemy enemyDefender)
                {
                    defenderDamage = CalculateDamage(defender, attacker);
                    attacker.TakeDamage(defenderDamage);
                    attackerDefeated = !attacker.IsAlive;
                }

                // Save changes
                await _eventSourcingService.SaveAsync(attacker);
                await _eventSourcingService.SaveAsync(defender);

                // Create result
                var result = new CombatResult
                {
                    PlayerDamage = attackerDamage,
                    EnemyDamage = defenderDamage,
                    PlayerHealth = attacker.Health,
                    EnemyHealth = defender.Health,
                    EnemyDefeated = defenderDefeated,
                    PlayerDefeated = attackerDefeated,
                    ExperienceGained = experienceGained,
                    ItemsDropped = itemsDropped
                };

                _logger.LogInformation("Combat completed: {Attacker} vs {Defender}, Attacker Damage: {AttackerDamage}, Defender Damage: {DefenderDamage}",
                    attacker.Name, defender.Name, attackerDamage, defenderDamage);

                return OperationResult<CombatResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing attack between {AttackerId} and {DefenderId}",
                    request.AttackerId, request.DefenderId);
                return OperationResult<CombatResult>.Failure("Combat.AttackFailed", ex.Message);
            }
        }

        private int CalculateDamage(Character attacker, Character defender)
        {
            // Simple damage calculation - you can make this more complex
            var baseDamage = attacker.Strength;
            var defense = defender.Defense;
            var randomFactor = new Random().Next(1, 6); // 1-5 random damage

            var damage = Math.Max(1, baseDamage + randomFactor - defense);
            return damage;
        }
    }
}
