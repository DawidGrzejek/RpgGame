using Newtonsoft.Json;
using RpgGame.Application.Serialization.DTOs;
using RpgGame.Application.Serialization.Mappers;
using RpgGame.Domain.Entities.Characters.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Infrastructure.Persistence.EFCore
{
    /// <summary>
    /// Represents a game save that holds all necessary data to resume a game
    /// </summary>
    public class GameSave
    {
        public int Id { get; set; }
        public string SaveName { get; set; }
        public DateTime SaveDate { get; set; }
        public string PlayerCharacterJson { get; set; }
        public string CurrentLocationName { get; set; }
        public int PlayTime { get; set; }

        [NotMapped]
        public PlayerCharacterDto PlayerCharacterData
        {
            get => JsonConvert.DeserializeObject<PlayerCharacterDto>(PlayerCharacterJson);
            set => PlayerCharacterJson = JsonConvert.SerializeObject(value);
        }

        [NotMapped]
        public Character PlayerCharacter
        {
            get => PlayerMapper.FromDto(PlayerCharacterData);
            set => PlayerCharacterData = PlayerMapper.ToDto(value as PlayerCharacter);
        }
    }
}
