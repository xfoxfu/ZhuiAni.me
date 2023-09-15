using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    /// <inheritdoc />
    public partial class AllIdAsUlidStep1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "id_v2",
                table: "user",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id_v2",
                table: "torrent",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id_v2",
                table: "session",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id_v2",
                table: "pikpak_job",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id_v2",
                table: "link",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "item_id_v2",
                table: "link",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "parent_link_id_v2",
                table: "link",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "category_id_v2",
                table: "item",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id_v2",
                table: "item",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id_v2",
                table: "category",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_v2",
                table: "user");

            migrationBuilder.DropColumn(
                name: "id_v2",
                table: "torrent");

            migrationBuilder.DropColumn(
                name: "user_id_v2",
                table: "session");

            migrationBuilder.DropColumn(
                name: "id_v2",
                table: "pikpak_job");

            migrationBuilder.DropColumn(
                name: "id_v2",
                table: "link");

            migrationBuilder.DropColumn(
                name: "item_id_v2",
                table: "link");

            migrationBuilder.DropColumn(
                name: "parent_link_id_v2",
                table: "link");

            migrationBuilder.DropColumn(
                name: "category_id_v2",
                table: "item");

            migrationBuilder.DropColumn(
                name: "id_v2",
                table: "item");

            migrationBuilder.DropColumn(
                name: "id_v2",
                table: "category");
        }
    }
}
