using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Repositories;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Concrete implementation of CharacterTemplate repository using Entity Framework
    /// </summary>
    public class CharacterTemplateRepository : Repository<CharacterTemplate>, ICharacterTemplateRepository
    {
        public CharacterTemplateRepository(GameDbContext context) : base(context)
        {
        }

        #region Read Operations

        public async Task<OperationResult<IEnumerable<CharacterTemplate>>> GetByCharacterTypeAsync(CharacterType characterType)
        {
            try
            {
                var templates = await _dbSet
                    .Where(ct => ct.CharacterType == characterType)
                    .OrderBy(ct => ct.Name)
                    .ToListAsync();

                return OperationResult<IEnumerable<CharacterTemplate>>.Success(templates);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<CharacterTemplate>>.Failure(
                    new OperationError("CharacterTemplate.GetByType.Failed",
                        $"Failed to get character templates by type '{characterType}'.",
                        ex.Message));
            }
        }

        public async Task<OperationResult<CharacterTemplate>> GetByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return OperationResult<CharacterTemplate>.Failure(
                        OperationError.ValidationFailed(nameof(name), "Name cannot be null, empty, or whitespace."));
                }

                var template = await _dbSet
                    .FirstOrDefaultAsync(ct => ct.Name.ToLower() == name.ToLower());

                if (template == null)
                {
                    return OperationResult<CharacterTemplate>.NotFound("CharacterTemplate", name);
                }

                return OperationResult<CharacterTemplate>.Success(template);
            }
            catch (Exception ex)
            {
                return OperationResult<CharacterTemplate>.Failure(
                    new OperationError("CharacterTemplate.GetByName.Failed",
                        $"Failed to get character template by name '{name}'.",
                        ex.Message));
            }
        }

        public async Task<OperationResult<IEnumerable<CharacterTemplate>>> GetByNPCBehaviorAsync(NPCBehavior behavior)
        {
            try
            {
                var templates = await _dbSet
                    .Where(ct => ct.NPCBehavior == behavior)
                    .OrderBy(ct => ct.Name)
                    .ToListAsync();

                return OperationResult<IEnumerable<CharacterTemplate>>.Success(templates);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<CharacterTemplate>>.Failure(
                    new OperationError("CharacterTemplate.GetByNPCBehavior.Failed",
                        $"Failed to get character templates by NPC behavior '{behavior}'.",
                        ex.Message));
            }
        }

        public async Task<OperationResult<IEnumerable<CharacterTemplate>>> GetByPlayerClassAsync(PlayerClass playerClass)
        {
            try
            {
                var templates = await _dbSet
                    .Where(ct => ct.PlayerClass == playerClass)
                    .OrderBy(ct => ct.Name)
                    .ToListAsync();

                if (!templates.Any())
                {
                    return OperationResult<IEnumerable<CharacterTemplate>>.NotFound("PlayerClass", playerClass.ToString());
                }

                return OperationResult<IEnumerable<CharacterTemplate>>.Success(templates);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<CharacterTemplate>>.Failure(
                    new OperationError("CharacterTemplate.GetByPlayerClass.Failed",
                        $"Failed to get character templates by player class '{playerClass}'.",
                        ex.Message));
            }
        }

        #endregion

        #region Write Operations

        public new async Task<OperationResult<CharacterTemplate>> AddAsync(CharacterTemplate characterTemplate)
        {
            try
            {
                if (characterTemplate == null)
                {
                    return OperationResult<CharacterTemplate>.Failure(
                        OperationError.ValidationFailed(nameof(characterTemplate), "Character template cannot be null."));
                }

                // Check if a template with the same name already exists
                var existingTemplate = await GetByNameAsync(characterTemplate.Name);

                if (existingTemplate.Succeeded)
                    return OperationResult<CharacterTemplate>.Failure(
                        OperationError.Conflict($"A character template with the name '{characterTemplate.Name}' already exists."));
                
                var result = await base.AddAsync(characterTemplate);

                if (result.Succeeded)
                    return OperationResult<CharacterTemplate>.Success(characterTemplate);

                return OperationResult<CharacterTemplate>.Failure(result.Errors.ToArray());
            }
            catch (Exception ex)
            {
                return OperationResult<CharacterTemplate>.Failure(
                    new OperationError("CharacterTemplate.Add.Failed",
                        "Failed to add character template.",
                        ex.Message));
            }
        }

        public new async Task<OperationResult<CharacterTemplate>> UpdateAsync(CharacterTemplate characterTemplate)
        {
            try
            {
                if (characterTemplate == null)
                    return OperationResult<CharacterTemplate>.Failure(
                        OperationError.ValidationFailed(nameof(characterTemplate), "Character template cannot be null."));                

                var existingTemplate = await GetByIdAsync(characterTemplate.Id);
                if (!existingTemplate.Succeeded || existingTemplate.Data == null)
                {
                    return OperationResult<CharacterTemplate>.NotFound("CharacterTemplate", characterTemplate.Id.ToString());
                }

                // Check if another template with the same name exists (excluding current template)
                var duplicateTemplate = await GetByNameAsync(characterTemplate.Name);
                if (duplicateTemplate.Succeeded && duplicateTemplate.Data.Id != characterTemplate.Id)
                {
                    return OperationResult<CharacterTemplate>.Failure(
                        OperationError.Conflict($"Another character template with the name '{characterTemplate.Name}' already exists."));
                }

                var result = await base.UpdateAsync(characterTemplate);
                if (!result.Succeeded)
                    return OperationResult<CharacterTemplate>.Failure(result.Errors.ToArray());

                return OperationResult<CharacterTemplate>.Success(characterTemplate);
            }
            catch (Exception ex)
            {
                return OperationResult<CharacterTemplate>.Failure(
                    new OperationError("CharacterTemplate.Update.Failed",
                        "Failed to update character template.",
                        ex.Message));
            }
        }

        public new async Task<OperationResult> DeleteAsync(CharacterTemplate characterTemplate)
        {
            try
            {
                if (characterTemplate == null)
                {
                    return OperationResult.Failure(
                        OperationError.ValidationFailed(nameof(characterTemplate), "Character template cannot be null."));
                }

                var existingTemplate = await GetByIdAsync(characterTemplate.Id);
                if (!existingTemplate.Succeeded || existingTemplate.Data == null)
                {
                    return OperationResult.NotFound("CharacterTemplate", characterTemplate.Id.ToString());
                }

                var result = await base.DeleteAsync(existingTemplate.Data);
                if (!result.Succeeded)
                    return OperationResult.Failure(result.Errors.ToArray());

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(
                    new OperationError("CharacterTemplate.Delete.Failed",
                        "Failed to delete character template.",
                        ex.Message));
            }
        }

        /// <summary>
        /// Soft delete by Id
        /// </summary>
        /// <param name="id">The Id of the character template to delete.</param>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> DeleteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return OperationResult.Failure(
                        OperationError.ValidationFailed(nameof(id), "Id cannot be empty."));
                }

                var template = await GetByIdAsync(id);
                if (!template.Succeeded || template.Data == null)
                    return OperationResult.NotFound("CharacterTemplate", id.ToString());

                var result = await base.DeleteAsync(template.Data);
                if (!result.Succeeded)
                    return OperationResult.Failure(result.Errors.ToArray());

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(
                    new OperationError("CharacterTemplate.DeleteById.Failed",
                        $"Failed to delete character template with id '{id}'.",
                        ex.Message));
            }
        }

        #endregion
    }
}