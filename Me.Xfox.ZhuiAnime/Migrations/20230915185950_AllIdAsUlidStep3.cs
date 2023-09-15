using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    /// <inheritdoc />
    public partial class AllIdAsUlidStep3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_item_category_category_id",
                table: "item");

            migrationBuilder.DropForeignKey(
                name: "fk_item_item_parent_item_id",
                table: "item");

            migrationBuilder.DropForeignKey(
                name: "fk_link_item_item_id",
                table: "link");

            migrationBuilder.DropForeignKey(
                name: "fk_link_link_parent_link_id",
                table: "link");

            migrationBuilder.DropForeignKey(
                name: "fk_session_user_user_id",
                table: "session");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_torrent",
                table: "torrent");

            migrationBuilder.DropIndex(
                name: "ix_session_user_id",
                table: "session");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pikpak_job",
                table: "pikpak_job");

            migrationBuilder.DropPrimaryKey(
                name: "pk_link",
                table: "link");

            migrationBuilder.DropIndex(
                name: "ix_link_item_id",
                table: "link");

            migrationBuilder.DropIndex(
                name: "ix_link_parent_link_id",
                table: "link");

            migrationBuilder.DropPrimaryKey(
                name: "pk_item",
                table: "item");

            migrationBuilder.DropIndex(
                name: "ix_item_category_id",
                table: "item");

            migrationBuilder.DropIndex(
                name: "ix_item_parent_item_id",
                table: "item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_category",
                table: "category");

            migrationBuilder.DropColumn(
                name: "id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "id",
                table: "torrent");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "session");

            migrationBuilder.DropColumn(
                name: "id",
                table: "pikpak_job");

            migrationBuilder.DropColumn(
                name: "id",
                table: "link");

            migrationBuilder.DropColumn(
                name: "item_id",
                table: "link");

            migrationBuilder.DropColumn(
                name: "parent_link_id",
                table: "link");

            migrationBuilder.DropColumn(
                name: "id",
                table: "item");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "item");

            migrationBuilder.DropColumn(
                name: "parent_item_id",
                table: "item");

            migrationBuilder.DropColumn(
                name: "id",
                table: "category");

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "user",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "torrent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id_v2",
                table: "session",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "pikpak_job",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id_v2",
                table: "link",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "link",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "item",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "category_id_v2",
                table: "item",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "category",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                table: "user",
                column: "id_v2");

            migrationBuilder.AddPrimaryKey(
                name: "pk_torrent",
                table: "torrent",
                column: "id_v2");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pikpak_job",
                table: "pikpak_job",
                column: "id_v2");

            migrationBuilder.AddPrimaryKey(
                name: "pk_link",
                table: "link",
                column: "id_v2");

            migrationBuilder.AddPrimaryKey(
                name: "pk_item",
                table: "item",
                column: "id_v2");

            migrationBuilder.AddPrimaryKey(
                name: "pk_category",
                table: "category",
                column: "id_v2");

            migrationBuilder.CreateIndex(
                name: "ix_session_user_id_v2",
                table: "session",
                column: "user_id_v2");

            migrationBuilder.CreateIndex(
                name: "ix_link_item_id_v2",
                table: "link",
                column: "item_id_v2");

            migrationBuilder.CreateIndex(
                name: "ix_link_parent_link_id_v2",
                table: "link",
                column: "parent_link_id_v2");

            migrationBuilder.CreateIndex(
                name: "ix_item_category_id_v2",
                table: "item",
                column: "category_id_v2");

            migrationBuilder.CreateIndex(
                name: "ix_item_parent_item_id_v2",
                table: "item",
                column: "parent_item_id_v2");

            migrationBuilder.AddForeignKey(
                name: "fk_item_category_category_temp_id",
                table: "item",
                column: "category_id_v2",
                principalTable: "category",
                principalColumn: "id_v2",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_item_item_parent_item_temp_id",
                table: "item",
                column: "parent_item_id_v2",
                principalTable: "item",
                principalColumn: "id_v2");

            migrationBuilder.AddForeignKey(
                name: "fk_link_item_item_temp_id1",
                table: "link",
                column: "item_id_v2",
                principalTable: "item",
                principalColumn: "id_v2",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_link_link_parent_link_temp_id",
                table: "link",
                column: "parent_link_id_v2",
                principalTable: "link",
                principalColumn: "id_v2");

            migrationBuilder.AddForeignKey(
                name: "fk_session_user_user_temp_id",
                table: "session",
                column: "user_id_v2",
                principalTable: "user",
                principalColumn: "id_v2",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_item_category_category_temp_id",
                table: "item");

            migrationBuilder.DropForeignKey(
                name: "fk_item_item_parent_item_temp_id",
                table: "item");

            migrationBuilder.DropForeignKey(
                name: "fk_link_item_item_temp_id1",
                table: "link");

            migrationBuilder.DropForeignKey(
                name: "fk_link_link_parent_link_temp_id",
                table: "link");

            migrationBuilder.DropForeignKey(
                name: "fk_session_user_user_temp_id",
                table: "session");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_torrent",
                table: "torrent");

            migrationBuilder.DropIndex(
                name: "ix_session_user_id_v2",
                table: "session");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pikpak_job",
                table: "pikpak_job");

            migrationBuilder.DropPrimaryKey(
                name: "pk_link",
                table: "link");

            migrationBuilder.DropIndex(
                name: "ix_link_item_id_v2",
                table: "link");

            migrationBuilder.DropIndex(
                name: "ix_link_parent_link_id_v2",
                table: "link");

            migrationBuilder.DropPrimaryKey(
                name: "pk_item",
                table: "item");

            migrationBuilder.DropIndex(
                name: "ix_item_category_id_v2",
                table: "item");

            migrationBuilder.DropIndex(
                name: "ix_item_parent_item_id_v2",
                table: "item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_category",
                table: "category");

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "user",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "user",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "torrent",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "torrent",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id_v2",
                table: "session",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "session",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "pikpak_job",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "pikpak_job",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id_v2",
                table: "link",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "link",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "link",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "item_id",
                table: "link",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "parent_link_id",
                table: "link",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "category_id_v2",
                table: "item",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "item",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "item",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "category_id",
                table: "item",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "parent_item_id",
                table: "item",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id_v2",
                table: "category",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "category",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_torrent",
                table: "torrent",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pikpak_job",
                table: "pikpak_job",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_link",
                table: "link",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_item",
                table: "item",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_category",
                table: "category",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_session_user_id",
                table: "session",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_link_item_id",
                table: "link",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_link_parent_link_id",
                table: "link",
                column: "parent_link_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_category_id",
                table: "item",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_parent_item_id",
                table: "item",
                column: "parent_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_item_category_category_id",
                table: "item",
                column: "category_id",
                principalTable: "category",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_item_item_parent_item_id",
                table: "item",
                column: "parent_item_id",
                principalTable: "item",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_link_item_item_id",
                table: "link",
                column: "item_id",
                principalTable: "item",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_link_link_parent_link_id",
                table: "link",
                column: "parent_link_id",
                principalTable: "link",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_session_user_user_id",
                table: "session",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
