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
    /// Repository for managing AbilityTemplate entities
    /// </summary>
    public class AbilityTemplateRepository : Repository<AbilityTemplate>, IAbilityTemplateRepository
    {
        public AbilityTemplateRepository(GameDbContext context) : base(context)
        {
        }

        #region Read Operations

        public async Task<OperationResult<IEnumerable<AbilityTemplate>>> GetByAbilityTypeAsync(AbilityType abilityType)
        {
            try
            {
                var abilities = await _dbSet.Where(a => a.AbilityType == abilityType).ToListAsync();
                return OperationResult<IEnumerable<AbilityTemplate>>.Success(abilities);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<AbilityTemplate>>.Failure(new OperationError("An error occurred while retrieving abilities by type.", ex.Message));
            }
        }

        public async Task<OperationResult<IEnumerable<AbilityTemplate>>> GetByTargetTypeAsync(TargetType targetType)
        {
            try
            {
                var abilities = await _dbSet.Where(a => a.TargetType == targetType).ToListAsync();
                return OperationResult<IEnumerable<AbilityTemplate>>.Success(abilities);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<AbilityTemplate>>.Failure(new OperationError("An error occurred while retrieving abilities by target type.", ex.Message));
            }
        }

        public async Task<OperationResult<AbilityTemplate>> GetByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return OperationResult<AbilityTemplate>.Failure(
                        OperationError.ValidationFailed(nameof(name), "Name cannot be null, empty, or whitespace."));

                var ability = await _dbSet.FirstOrDefaultAsync(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                return ability != null
                    ? OperationResult<AbilityTemplate>.Success(ability)
                    : OperationResult<AbilityTemplate>.Failure(new OperationError("Ability not found.", $"No ability with name '{name}' exists."));
            }
            catch (Exception ex)
            {
                return OperationResult<AbilityTemplate>.Failure(new OperationError("An error occurred while retrieving ability by name.", ex.Message));
            }
        }

        public async Task<OperationResult<IEnumerable<AbilityTemplate>>> GetAvailableForCharacterAsync(Dictionary<string, object> characterData)
        {
            try
            {
                if (characterData == null || !characterData.Any())
                    return OperationResult<IEnumerable<AbilityTemplate>>.Success(Enumerable.Empty<AbilityTemplate>());

                var query = _dbSet.AsQueryable();

                foreach (var kvp in characterData)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;

                    // Assuming AbilityTemplate has properties that match the keys in characterData
                    // and that they are of comparable types.
                    query = query.Where(a => EF.Property<object>(a, key).Equals(value));
                }

                var abilities = await query.ToListAsync();
                return OperationResult<IEnumerable<AbilityTemplate>>.Success(abilities);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<AbilityTemplate>>.Failure(new OperationError("An error occurred while retrieving available abilities for character.", ex.Message));
            }
        }

        #endregion

        #region Write Operations

        public new async Task<OperationResult<AbilityTemplate>> AddAsync(AbilityTemplate abilityTemplate)
        {
            try
            {
                if (abilityTemplate == null)
                    throw new ArgumentNullException(nameof(abilityTemplate));

                var existingTemplate = await GetByNameAsync(abilityTemplate.Name);
                if (existingTemplate.Succeeded)
                    return OperationResult<AbilityTemplate>.Failure(OperationError.ValidationFailed("Name", $"An ability with the name '{abilityTemplate.Name}' already exists."));

                var result = await base.AddAsync(abilityTemplate);
                if (!result.Succeeded)
                    return OperationResult<AbilityTemplate>.Failure(result.Errors.ToArray());

                return OperationResult<AbilityTemplate>.Success(abilityTemplate);
            }
            catch (Exception ex)
            {
                return OperationResult<AbilityTemplate>.Failure(new OperationError("AbilityTemplate.Add.Failed", ex.Message));
            }
        }

        public new async Task<OperationResult<AbilityTemplate>> UpdateAsync(AbilityTemplate abilityTemplate)
        {
            try
            {
                if (abilityTemplate == null)
                    throw new ArgumentNullException(nameof(abilityTemplate));

                var existingTemplate = await GetByIdAsync(abilityTemplate.Id);
                if (!existingTemplate.Succeeded || existingTemplate.Data == null)
                    return OperationResult<AbilityTemplate>.NotFound("AbilityTemplate", abilityTemplate.Id.ToString());

                var duplicateTemplate = await GetByNameAsync(abilityTemplate.Name);

                if (duplicateTemplate.Succeeded && duplicateTemplate.Data.Id != abilityTemplate.Id)
                    return OperationResult<AbilityTemplate>.Failure(OperationError.ValidationFailed("Name", $"An ability with the name '{abilityTemplate.Name}' already exists."));

                var result = await base.UpdateAsync(abilityTemplate);
                if (!result.Succeeded)
                    return OperationResult<AbilityTemplate>.Failure(result.Errors.ToArray());

                return OperationResult<AbilityTemplate>.Success(abilityTemplate);
            }
            catch (Exception ex)
            {
                return OperationResult<AbilityTemplate>.Failure(new OperationError("AbilityTemplate.Update.Failed", ex.Message));
            }
        }

        public new async Task<OperationResult> DeleteAsync(AbilityTemplate abilityTemplate)
        {
            try
            {
                if (abilityTemplate == null)
                {
                    return OperationResult.Failure(
                        OperationError.ValidationFailed(nameof(abilityTemplate), "Ability template cannot be null."));
                }

                var existingTemplate = await GetByIdAsync(abilityTemplate.Id);
                if (!existingTemplate.Succeeded || existingTemplate.Data == null)
                {
                    return OperationResult.NotFound("AbilityTemplate", abilityTemplate.Id.ToString());
                }

                var result = await base.DeleteAsync(existingTemplate.Data);
                if (!result.Succeeded)
                    return OperationResult.Failure(result.Errors.ToArray());

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(
                    new OperationError("AbilityTemplate.Delete.Failed",
                        "Failed to delete ability template.",
                        ex.Message));
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return OperationResult.Failure(
                        OperationError.ValidationFailed(nameof(id), "ID cannot be empty."));
                }

                var entityResult = await GetByIdAsync(id);
                if (!entityResult.Succeeded || entityResult.Data == null)
                    return OperationResult.NotFound("AbilityTemplate", id.ToString());

                var result = await base.DeleteAsync(entityResult.Data);
                if (!result.Succeeded)
                    return OperationResult.Failure(result.Errors.ToArray());

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(new OperationError("AbilityTemplate.Delete.Failed", "Failed to delete ability template.", ex.Message));
            }
        }

        #endregion

    }
}