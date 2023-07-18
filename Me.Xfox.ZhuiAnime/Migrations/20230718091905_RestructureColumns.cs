using System;
using Me.Xfox.ZhuiAnime.Modules.PikPak;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    /// <inheritdoc />
    public partial class RestructureColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "pik_pak_anime",
                newName: "pikpak_job");

            migrationBuilder.RenameIndex(
                name: "pk_pik_pak_anime",
                table: "pikpak_job",
                newName: "pk_pikpak_job");

            migrationBuilder.AddColumn<long>(
                name: "match_group_ep",
                table: "pikpak_job",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql("""
                UPDATE pikpak_job
                SET match_group_ep = (match_group->'Ep')::bigint;
            """);

            migrationBuilder.DropColumn(
                name: "match_group",
                table: "pikpak_job");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "pikpak_job",
                newName: "pik_pak_anime");

            migrationBuilder.RenameIndex(
                name: "pk_pikpak_job",
                table: "pik_pak_anime",
                newName: "pk_pik_pak_anime");

            migrationBuilder.AddColumn<PikPakJob.MatchGroups>(
                name: "match_group",
                table: "pik_pak_anime",
                type: "jsonb",
                nullable: false,
                defaultValue: new PikPakJob.MatchGroups());

            migrationBuilder.Sql("""
                UPDATE pik_pak_anime
                SET match_group = json_build_object('Ep', match_group_ep);
            """);

            migrationBuilder.DropColumn(
                name: "match_group_ep",
                table: "pik_pak_anime");
        }
    }
}
