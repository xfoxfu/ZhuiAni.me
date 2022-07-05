using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "anime",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    image = table.Column<byte[]>(type: "bytea", nullable: true),
                    bangumi_link = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_anime", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "episode",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    anime_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    bangumi_link = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_episode", x => x.id);
                    table.ForeignKey(
                        name: "fk_episode_anime_anime_id",
                        column: x => x.anime_id,
                        principalTable: "anime",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "link",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address = table.Column<string>(type: "text", nullable: false),
                    annotation = table.Column<string>(type: "text", nullable: false),
                    discriminator = table.Column<string>(type: "text", nullable: false),
                    anime_id = table.Column<long>(type: "bigint", nullable: true),
                    episode_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_link", x => x.id);
                    table.ForeignKey(
                        name: "fk_link_anime_anime_id",
                        column: x => x.anime_id,
                        principalTable: "anime",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_link_episode_episode_id",
                        column: x => x.episode_id,
                        principalTable: "episode",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_anime_bangumi_link",
                table: "anime",
                column: "bangumi_link",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_episode_anime_id",
                table: "episode",
                column: "anime_id");

            migrationBuilder.CreateIndex(
                name: "ix_episode_bangumi_link",
                table: "episode",
                column: "bangumi_link",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_link_anime_id",
                table: "link",
                column: "anime_id");

            migrationBuilder.CreateIndex(
                name: "ix_link_episode_id",
                table: "link",
                column: "episode_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "link");

            migrationBuilder.DropTable(
                name: "episode");

            migrationBuilder.DropTable(
                name: "anime");
        }
    }
}
