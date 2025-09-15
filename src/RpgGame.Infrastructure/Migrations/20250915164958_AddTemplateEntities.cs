using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgGame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbilityTemplates",
                schema: "RpgGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AbilityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManaCost = table.Column<int>(type: "int", nullable: false),
                    Cooldown = table.Column<int>(type: "int", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Range = table.Column<int>(type: "int", nullable: false),
                    AnimationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoundEffect = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilityTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSnapshots",
                schema: "RpgGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The Character ID this snapshot represents"),
                    EventVersion = table.Column<int>(type: "int", nullable: false, comment: "The event version this snapshot captures up to"),
                    TotalEventCount = table.Column<int>(type: "int", nullable: false, comment: "Total number of events used to create this snapshot"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "When this snapshot was created"),
                    SerializedState = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false, comment: "Serialized character state as JSON"),
                    CharacterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Character name for quick lookups"),
                    CharacterLevel = table.Column<int>(type: "int", nullable: false, comment: "Character level at time of snapshot"),
                    CharacterType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Character type (Player/NPC)"),
                    IsLatest = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Indicates if this is the latest snapshot for the character"),
                    StateSize = table.Column<int>(type: "int", nullable: false, comment: "Size of serialized state in bytes"),
                    CreationDuration = table.Column<TimeSpan>(type: "time", nullable: false, comment: "Time taken to create this snapshot")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterTemplates",
                schema: "RpgGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CharacterType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NPCBehavior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseStats_Level = table.Column<int>(type: "int", nullable: false),
                    BaseStats_CurrentHealth = table.Column<int>(type: "int", nullable: false),
                    BaseStats_MaxHealth = table.Column<int>(type: "int", nullable: false),
                    BaseStats_Strength = table.Column<int>(type: "int", nullable: false),
                    BaseStats_Defense = table.Column<int>(type: "int", nullable: false),
                    BaseStats_Speed = table.Column<int>(type: "int", nullable: false),
                    BaseStats_Magic = table.Column<int>(type: "int", nullable: false),
                    ConfigurationData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AbilityIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LootTableIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BehaviorData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemTemplates",
                schema: "RpgGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    StatModifiers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsConsumable = table.Column<bool>(type: "bit", nullable: false),
                    IsEquippable = table.Column<bool>(type: "bit", nullable: false),
                    EquipmentSlot = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbilityEffect",
                schema: "RpgGame",
                columns: table => new
                {
                    AbilityTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EffectType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasePower = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilityEffect", x => new { x.AbilityTemplateId, x.Id });
                    table.ForeignKey(
                        name: "FK_AbilityEffect_AbilityTemplates_AbilityTemplateId",
                        column: x => x.AbilityTemplateId,
                        principalSchema: "RpgGame",
                        principalTable: "AbilityTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbilityTemplates_Name",
                schema: "RpgGame",
                table: "AbilityTemplates",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSnapshots_CharacterId",
                schema: "RpgGame",
                table: "CharacterSnapshots",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSnapshots_CharacterId_IsLatest",
                schema: "RpgGame",
                table: "CharacterSnapshots",
                columns: new[] { "CharacterId", "IsLatest" },
                filter: "[IsLatest] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSnapshots_CreatedAt",
                schema: "RpgGame",
                table: "CharacterSnapshots",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSnapshots_StateSize",
                schema: "RpgGame",
                table: "CharacterSnapshots",
                column: "StateSize");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSnapshots_Type_Level",
                schema: "RpgGame",
                table: "CharacterSnapshots",
                columns: new[] { "CharacterType", "CharacterLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTemplates_Name",
                schema: "RpgGame",
                table: "CharacterTemplates",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemTemplates_Name",
                schema: "RpgGame",
                table: "ItemTemplates",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbilityEffect",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "CharacterSnapshots",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "CharacterTemplates",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "ItemTemplates",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "AbilityTemplates",
                schema: "RpgGame");
        }
    }
}
