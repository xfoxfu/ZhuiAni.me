using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    public partial class Catalog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_link_anime_anime_id",
                table: "link");

            migrationBuilder.DropForeignKey(
                name: "fk_link_episode_episode_id",
                table: "link");

            migrationBuilder.AddColumn<long>(
                name: "catalog_id",
                table: "link",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "catalog",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_link_catalog_id",
                table: "link",
                column: "catalog_id");

            migrationBuilder.AddForeignKey(
                name: "fk_link_anime_anime_id",
                table: "link",
                column: "anime_id",
                principalTable: "anime",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_link_catalog_catalog_id",
                table: "link",
                column: "catalog_id",
                principalTable: "catalog",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_link_episode_episode_id",
                table: "link",
                column: "episode_id",
                principalTable: "episode",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_link_anime_anime_id",
                table: "link");

            migrationBuilder.DropForeignKey(
                name: "fk_link_catalog_catalog_id",
                table: "link");

            migrationBuilder.DropForeignKey(
                name: "fk_link_episode_episode_id",
                table: "link");

            migrationBuilder.DropTable(
                name: "catalog");

            migrationBuilder.DropIndex(
                name: "ix_link_catalog_id",
                table: "link");

            migrationBuilder.DropColumn(
                name: "catalog_id",
                table: "link");

            migrationBuilder.AddForeignKey(
                name: "fk_link_anime_anime_id",
                table: "link",
                column: "anime_id",
                principalTable: "anime",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_link_episode_episode_id",
                table: "link",
                column: "episode_id",
                principalTable: "episode",
                principalColumn: "id");
        }
    }
}
