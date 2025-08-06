using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Commands.Results;
using RpgGame.Application.Events;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Combat
{
    public class FleeCombatCommand : ICommand<OperationResult<FleeResult>>
    {
        public Guid CharacterId { get; set; }
        public Guid EnemyId { get; set; }
    }

    public class FleeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CombatResult? EnemyCounterAttack { get; set; }
    }

    public class FleeCombatCommandValidator : AbstractValidator<FleeCombatCommand>
    {
        public FleeCombatCommandValidator()
        {
            RuleFor(x => x.CharacterId)
                .NotEmpty()
                .WithMessage("Character ID is required");

            RuleFor(x => x.EnemyId)
                .NotEmpty()
                .WithMessage("Enemy ID is required");
        }
    }

    public class FleeCombatCommandHandler : IRequestHandler<FleeCombatCommand, OperationResult<FleeResult>>
    {
        private readonly IEventSourcingService _eventSourcingService;
        private readonly ILogger<FleeCombatCommandHandler> _logger;

        public FleeCombatCommandHandler(
            IEventSourcingService eventSourcingService,
            ILogger<FleeCombatCommandHandler> logger)
        {
            _eventSourcingService = eventSourcingService;
            _logger = logger;
        }

        public async Task<OperationResult<FleeResult>> Handle(FleeCombatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var character = await _eventSourcingService.GetByIdAsync<Character>(request.CharacterId);
                var enemy = await _eventSourcingService.GetByIdAsync<Character>(request.EnemyId);

                if (character == null)
                    return OperationResult<FleeResult>.NotFound("Character", request.CharacterId.ToString());

                if (enemy == null)
                    return OperationResult<FleeResult>.NotFound("Enemy", request.EnemyId.ToString());

                if (!character.IsAlive)
                    return OperationResult<FleeResult>.BusinessRuleViolation("DeadCharacter", "Dead characters cannot flee");

                // Calculate flee chance (could be based on character stats, enemy stats, etc.)
                var fleeChance = CalculateFleeChance(character, enemy);
                var random = new Random();
                var fleeRoll = random.NextDouble();

                var result = new FleeResult();

                if (fleeRoll <= fleeChance)
                {
                    // Successful flee
                    result.Success = true;
                    result.Message = $"{character.Name} successfully escaped from {enemy.Name}!";

                    _logger.LogInformation("Character {CharacterName} successfully fled from {EnemyName}",
                        character.Name, enemy.Name);
                }
                else
                {
                    // Failed flee - enemy gets a free attack
                    result.Success = false;
                    result.Message = $"{character.Name} failed to escape! {enemy.Name} attacks!";

                    var damage = CalculateEnemyDamage(enemy, character);
                    character.TakeDamage(damage);

                    result.EnemyCounterAttack = new CombatResult
                    {
                        PlayerDamage = 0,
                        EnemyDamage = damage,
                        PlayerHealth = character.Stats.CurrentHealth,
                        EnemyHealth = enemy.Stats.CurrentHealth,
                        EnemyDefeated = false,
                        PlayerDefeated = !character.IsAlive,
                        ExperienceGained = 0,
                        ItemsDropped = new List<string>()
                    };

                    await _eventSourcingService.SaveAsync(character);

                    _logger.LogInformation("Character {CharacterName} failed to flee from {EnemyName} and took {Damage} damage",
                        character.Name, enemy.Name, damage);
                }

                return OperationResult<FleeResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing flee attempt for character {CharacterId} from enemy {EnemyId}",
                    request.CharacterId, request.EnemyId);
                return OperationResult<FleeResult>.Failure("Combat.FleeFailed", ex.Message);
            }
        }

        private double CalculateFleeChance(Character character, Character enemy)
        {
            // Base flee chance
            double baseChance = 0.7; // 70% base chance

            // Modify based on level difference
            int levelDifference = character.Stats.Level - enemy.Stats.Level;
            double levelModifier = levelDifference * 0.05; // +/-5% per level difference

            // Modify based on health percentage
            double healthPercent = (double)character.Stats.CurrentHealth / character.Stats.MaxHealth;
            double healthModifier = (1 - healthPercent) * 0.2; // Up to +20% when near death

            // Calculate final chance
            double finalChance = baseChance + levelModifier + healthModifier;

            // Clamp between 10% and 95%
            return Math.Max(0.1, Math.Min(0.95, finalChance));
        }

        private int CalculateEnemyDamage(Character enemy, Character target)
        {
            // Simple damage calculation for enemy counter-attack
            var baseDamage = enemy.Stats.Strength;
            var defense = target.Stats.Defense;
            var randomFactor = new Random().Next(1, 4); // Slightly less random damage for counter-attack

            var damage = Math.Max(1, baseDamage + randomFactor - defense);
            return damage;
        }
    }
}