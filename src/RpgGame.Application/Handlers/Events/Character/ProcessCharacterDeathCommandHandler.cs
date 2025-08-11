using MediatR;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Commands.Characters;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Items;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Handlers.Events.Character
{
    /// <summary>
     /// Handler that processes all the complex logic when a character dies
     /// </summary>
    public class ProcessCharacterDeathCommandHandler : IRequestHandler<ProcessCharacterDeathCommand, OperationResult<CharacterDeathResult>>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IGameSaveService _gameSaveService;
        private readonly IQuestService _questService;
        private readonly IEventSourcingService _eventSourcingService;
        private readonly IGameWorld _gameWorld;
        private readonly ILogger<ProcessCharacterDeathCommandHandler> _logger;

        public ProcessCharacterDeathCommandHandler(
            ICharacterRepository characterRepository,
            IGameSaveService gameSaveService,
            IQuestService questService,
            IEventSourcingService eventSourcingService,
            IGameWorld gameWorld,
            ILogger<ProcessCharacterDeathCommandHandler> logger)
        {
            _characterRepository = characterRepository;
            _gameSaveService = gameSaveService;
            _questService = questService;
            _eventSourcingService = eventSourcingService;
            _gameWorld = gameWorld;
            _logger = logger;
        }

        public async Task<OperationResult<CharacterDeathResult>> Handle(
            ProcessCharacterDeathCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing death for character {CharacterId} at {Location}",
                    request.CharacterId, request.LocationName);

                // 1. Get the character
                var character = await _eventSourcingService.GetByIdAsync<RpgGame.Domain.Entities.Characters.Base.Character>(request.CharacterId);
                if (character == null)
                {
                    return OperationResult<CharacterDeathResult>.NotFound("Character", request.CharacterId.ToString());
                }

                // 2. Verify character is actually dead
                if (character.IsAlive)
                {
                    return OperationResult<CharacterDeathResult>.BusinessRuleViolation(
                        "CharacterNotDead",
                        "Cannot process death for a living character");
                }

                // 3. Create result object
                var result = new CharacterDeathResult
                {
                    CharacterId = character.Id,
                    CharacterName = character.Name
                };

                // 4. Handle player character death differently than NPC death
                if (request.IsPlayerCharacter && character.Type == CharacterType.Player)
                {
                    await ProcessPlayerDeath(character, request, result, cancellationToken);
                }
                else
                {
                    await ProcessNpcDeath(character, request, result, cancellationToken);
                }

                // 5. Update statistics and analytics
                await UpdateDeathStatistics(character, request.CauseOfDeath, cancellationToken);

                // 6. Check for achievements related to death
                await CheckDeathAchievements(character, request, result, cancellationToken);

                // 7. Auto-save the game state
                if (request.IsPlayerCharacter)
                {
                    var currentLocation = _gameWorld.GetLocation(request.LocationName);
                    if (currentLocation != null)
                    {
                        await _gameSaveService.SaveGameAsync("AutoSave_Death", character, currentLocation, cancellationToken);
                    }
                }

                _logger.LogInformation("Successfully processed death for character {CharacterName}", character.Name);
                return OperationResult<CharacterDeathResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing character death for {CharacterId}", request.CharacterId);
                return OperationResult<CharacterDeathResult>.Failure("CharacterDeath.ProcessingFailed", ex.Message);
            }
        }

        private async Task ProcessPlayerDeath(
            RpgGame.Domain.Entities.Characters.Base.Character player,
            ProcessCharacterDeathCommand request,
            CharacterDeathResult result,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing player death for {PlayerName}", player.Name);

            // 1. Determine if this is game over or if player can respawn
            result.GameOver = DetermineGameOver(player, request);

            if (!result.GameOver)
            {
                // 2. Calculate respawn details
                result.RespawnLocation = DetermineRespawnLocation(request.LocationName);
                result.RespawnCooldown = CalculateRespawnCooldown(player.Level);

                // 3. Apply death penalties
                await ApplyDeathPenalties(player, result, cancellationToken);

                // 4. Handle inventory drops (maybe drop some items but not all)
                await HandlePlayerInventoryOnDeath(player, result, cancellationToken);
            }
            else
            {
                // Game over - handle permadeath scenario
                result.ExperienceDropped = player.Experience;
                result.GoldDropped = player.Inventory.Gold;

                // Drop all items for other players to find
                result.ItemsDropped = player.Inventory.Items.Select(i => i.Name).ToList();
            }

            // 5. Fail any time-sensitive quests
            //result.FailedQuests = await _questService.FailQuestsOnPlayerDeath(player.Id, cancellationToken);
        }

        private async Task ProcessNpcDeath(
            RpgGame.Domain.Entities.Characters.Base.Character npc,
            ProcessCharacterDeathCommand request,
            CharacterDeathResult result,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing NPC death for {NpcName}", npc.Name);

            // NPCs might drop experience and loot for the killer
            if (request.KillerId.HasValue && npc.Type == CharacterType.NPC)
            {
                // Get experience reward from template or custom data
                var experienceReward = GetExperienceReward(npc);
                result.ExperienceDropped = experienceReward;

                // Determine loot drops from template
                var droppedItems = GetLootDrops(npc);
                result.ItemsDropped.AddRange(droppedItems);

                // Award experience to killer if it's a player
                await AwardExperienceToKiller(request.KillerId.Value, result.ExperienceDropped, cancellationToken);
            }

            // Remove NPC from location's active enemies list
            // This would require updating the location state
        }

        private bool DetermineGameOver(RpgGame.Domain.Entities.Characters.Base.Character player, ProcessCharacterDeathCommand request)
        {
            // In hardcore mode, death is permanent
            // In normal mode, players can respawn
            // This could be configurable per game mode

            return false; // For now, always allow respawn
        }

        private string DetermineRespawnLocation(string deathLocation)
        {
            // Players typically respawn at the nearest safe location
            // This could be a town, checkpoint, or spawn point

            return deathLocation switch
            {
                "Forest" => "Town",
                "Cave" => "Town",
                "Mountain" => "Town",
                _ => "Town" // Default safe location
            };
        }

        private TimeSpan CalculateRespawnCooldown(int playerLevel)
        {
            // Higher level players might have shorter cooldowns
            // Or longer cooldowns as penalty for being careless

            var baseSeconds = Math.Max(30, 120 - (playerLevel * 5));
            return TimeSpan.FromSeconds(baseSeconds);
        }

        private async Task ApplyDeathPenalties(
            RpgGame.Domain.Entities.Characters.Base.Character player,
            CharacterDeathResult result,
            CancellationToken cancellationToken)
        {
            // Common death penalties in RPGs:
            // 1. Experience loss
            // 2. Equipment durability damage
            // 3. Temporary stat debuffs
            // 4. Gold loss

            var experienceLoss = (int)(player.Experience * 0.1); // 10% experience loss
            if (experienceLoss > 0)
            {
                // This would trigger another command to modify experience
                result.ExperienceDropped = experienceLoss;
            }

            var goldLoss = (int)(player.Inventory.Gold * 0.05); // 5% gold loss
            if (goldLoss > 0)
            {
                result.GoldDropped = goldLoss;
            }
        }

        private async Task HandlePlayerInventoryOnDeath(
            RpgGame.Domain.Entities.Characters.Base.Character player,
            CharacterDeathResult result,
            CancellationToken cancellationToken)
        {
            // In most RPGs, players don't lose all items on death
            // Maybe drop a few random items or items of certain types

            var droppableItems = player.Inventory.Items
                .Where(item => IsItemDroppableOnDeath(item))
                .Take(3) // Drop max 3 items
                .ToList();

            result.ItemsDropped = droppableItems.Select(i => i.Name).ToList();
        }

        private bool IsItemDroppableOnDeath(IItem item)
        {
            // Quest items and bound items shouldn't drop
            // Consumables and common equipment might drop

            return item.Type switch
            {
                ItemType.QuestItem => false,
                ItemType.Potion => true,
                ItemType.Weapon => true,
                ItemType.Armor => true,
                _ => false
            };
        }

        private async Task AwardExperienceToKiller(
            Guid killerId,
            int experience,
            CancellationToken cancellationToken)
        {
            if (experience > 0)
            {
                // This would trigger another command
                var awardExpCommand = new AwardExperienceCommand
                {
                    CharacterId = killerId,
                    ExperienceAmount = experience,
                    Source = "Enemy Kill"
                };

                // We could use MediatR to send this command
                // await _mediator.Send(awardExpCommand, cancellationToken);
            }
        }

        private async Task UpdateDeathStatistics(
            RpgGame.Domain.Entities.Characters.Base.Character character,
            string causeOfDeath,
            CancellationToken cancellationToken)
        {
            // Update game analytics
            // Track common causes of death
            // Update leaderboards

            _logger.LogInformation("Updating death statistics for {Character} - Cause: {Cause}",
                character.Name, causeOfDeath);
        }

        private async Task CheckDeathAchievements(
            RpgGame.Domain.Entities.Characters.Base.Character character,
            ProcessCharacterDeathCommand request,
            CharacterDeathResult result,
            CancellationToken cancellationToken)
        {
            // Check for death-related achievements
            // "Die 100 times", "Die to a specific enemy", etc.

            var achievements = new List<string>();

            if (character.Level >= 50)
            {
                achievements.Add("High Level Death - Died at level 50+");
            }

            if (request.CauseOfDeath.Contains("Dragon"))
            {
                achievements.Add("Dragon Slayer's End - Killed by a dragon");
            }

            result.CompletedAchievements = achievements;
        }

        private int GetExperienceReward(RpgGame.Domain.Entities.Characters.Base.Character npc)
        {
            // Template-driven approach: Get experience from template or custom data
            if (npc.CustomData.TryGetValue("ExperienceReward", out var expReward))
            {
                return Convert.ToInt32(expReward);
            }
            
            // Default experience based on level
            return npc.Stats.Level * 10;
        }

        private List<string> GetLootDrops(RpgGame.Domain.Entities.Characters.Base.Character npc)
        {
            var droppedItems = new List<string>();
            
            // Template-driven approach: Get loot from template or custom data
            if (npc.CustomData.TryGetValue("LootTable", out var lootData))
            {
                // This would typically involve a more complex loot generation system
                // For now, return simple loot based on custom data
                if (lootData is List<string> lootList)
                {
                    // Simple random drop logic
                    var random = new Random();
                    foreach (var item in lootList)
                    {
                        if (random.NextDouble() < 0.3) // 30% chance to drop each item
                        {
                            droppedItems.Add(item);
                        }
                    }
                }
            }
            
            return droppedItems;
        }
    }
}
