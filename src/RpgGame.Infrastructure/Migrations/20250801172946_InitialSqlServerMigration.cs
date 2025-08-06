using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgGame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "RpgGame");

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "RpgGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameSaves",
                schema: "RpgGame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaveName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    player_character_json = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentLocationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "RpgGame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "RpgGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AspNetUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Roles = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    email_notifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    game_sound_enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    theme = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "dark"),
                    language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    Preferences_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharacterIds = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Statistics_TotalPlayTimeMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Statistics_CharactersCreated = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Statistics_TotalLogins = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Statistics_HighestCharacterLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Statistics_TotalEnemiesDefeated = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Statistics_TotalQuestsCompleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Statistics_TotalDeaths = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Statistics_FirstLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statistics_LastActiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statistics_HasCreatedFirstCharacter = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Statistics_HasReachedLevel10 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Statistics_HasReachedLevel50 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Statistics_HasCompleted10Quests = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Statistics_HasDefeated100Enemies = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Statistics_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_AggregateId",
                schema: "RpgGame",
                table: "Events",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AggregateId_Timestamp",
                schema: "RpgGame",
                table: "Events",
                columns: new[] { "AggregateId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_AggregateId_Version",
                schema: "RpgGame",
                table: "Events",
                columns: new[] { "AggregateId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventType",
                schema: "RpgGame",
                table: "Events",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Timestamp",
                schema: "RpgGame",
                table: "Events",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_SaveName",
                schema: "RpgGame",
                table: "GameSaves",
                column: "SaveName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                schema: "RpgGame",
                table: "RefreshTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "RpgGame",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "RpgGame",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AspNetUserId",
                schema: "RpgGame",
                table: "Users",
                column: "AspNetUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                schema: "RpgGame",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "RpgGame",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                schema: "RpgGame",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "RpgGame",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "GameSaves",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "RpgGame");
        }
    }
}
