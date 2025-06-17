using FluentValidation;
using RpgGame.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Characters
{
    /// <summary>
    /// Command to process everything that happens when a character dies
    /// </summary>
    public class ProcessCharacterDeathCommand : ICommand<OperationResult<CharacterDeathResult>>
    {
        public Guid CharacterId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string CauseOfDeath { get; set; } = "Unknown";
        public Guid? KillerId { get; set; } = null;
        public bool IsPlayerCharacter { get; set; }
    }

    /// <summary>
    /// Result object for character death processing
    /// </summary>
    public class CharacterDeathResult
    {
        public Guid CharacterId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public int ExperienceDropped { get; set; }
        public List<string> ItemsDropped { get; set; } = new();
        public int GoldDropped { get; set; }
        public string RespawnLocation { get; set; } = string.Empty;
        public TimeSpan RespawnCooldown { get; set; }
        public bool GameOver { get; set; }
        public List<string> CompletedAchievements { get; set; } = new();
        public List<string> FailedQuests { get; set; } = new();
    }

    /// <summary>
    /// Validator for the death command
    /// </summary>
    public class ProcessCharacterDeathCommandValidator : AbstractValidator<ProcessCharacterDeathCommand>
    {
        public ProcessCharacterDeathCommandValidator()
        {
            RuleFor(x => x.CharacterId)
                .NotEmpty()
                .WithMessage("Character ID is required");

            RuleFor(x => x.LocationName)
                .NotEmpty()
                .WithMessage("Location name is required");

            RuleFor(x => x.CauseOfDeath)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Cause of death must be specified and under 100 characters");
        }
    }
}
