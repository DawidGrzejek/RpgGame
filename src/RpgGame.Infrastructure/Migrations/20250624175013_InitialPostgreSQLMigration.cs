using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgGame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQLMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "RpgGame");

            migrationBuilder.CreateTable(
                name: "events",
                schema: "RpgGame",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aggregate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    aggregate_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    event_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    event_data = table.Column<string>(type: "jsonb", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    user_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "game_saves",
                schema: "RpgGame",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    save_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    player_character_json = table.Column<string>(type: "jsonb", nullable: false),
                    current_location_name = table.Column<string>(type: "text", nullable: false),
                    play_time = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_saves", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_events_aggregate_id",
                schema: "RpgGame",
                table: "events",
                column: "aggregate_id");

            migrationBuilder.CreateIndex(
                name: "ix_events_aggregate_id_timestamp",
                schema: "RpgGame",
                table: "events",
                columns: new[] { "aggregate_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_events_aggregate_id_version",
                schema: "RpgGame",
                table: "events",
                columns: new[] { "aggregate_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_events_event_type",
                schema: "RpgGame",
                table: "events",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "ix_events_timestamp",
                schema: "RpgGame",
                table: "events",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "ix_game_saves_save_name",
                schema: "RpgGame",
                table: "game_saves",
                column: "save_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events",
                schema: "RpgGame");

            migrationBuilder.DropTable(
                name: "game_saves",
                schema: "RpgGame");
        }
    }
}
