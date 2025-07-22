using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgGame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "RpgGame",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "RpgGame",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    asp_net_user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    roles = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    email_notifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    game_sound_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    theme = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "dark"),
                    language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    preferences_id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_ids = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    statistics_total_play_time_minutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_characters_created = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_total_logins = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_highest_character_level = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_total_enemies_defeated = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_total_quests_completed = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_total_deaths = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_first_login_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    statistics_last_active_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    statistics_has_created_first_character = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    statistics_has_reached_level10 = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    statistics_has_reached_level50 = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    statistics_has_completed10quests = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    statistics_has_defeated100enemies = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    statistics_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_expires_at",
                schema: "RpgGame",
                table: "refresh_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token",
                schema: "RpgGame",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "RpgGame",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_asp_net_user_id",
                schema: "RpgGame",
                table: "users",
                column: "asp_net_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_created_at",
                schema: "RpgGame",
                table: "users",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "RpgGame",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_is_active",
                schema: "RpgGame",
                table: "users",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                schema: "RpgGame",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "users",
                schema: "RpgGame");
        }
    }
}
