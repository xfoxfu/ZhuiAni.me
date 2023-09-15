using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    /// <inheritdoc />
    public partial class AllIdAsUlidStep4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "id_v2",
                table: "user",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "id_v2",
                table: "torrent",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "user_id_v2",
                table: "session",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_session_user_id_v2",
                table: "session",
                newName: "ix_session_user_id");

            migrationBuilder.RenameColumn(
                name: "id_v2",
                table: "pikpak_job",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "parent_link_id_v2",
                table: "link",
                newName: "parent_link_id");

            migrationBuilder.RenameColumn(
                name: "item_id_v2",
                table: "link",
                newName: "item_id");

            migrationBuilder.RenameColumn(
                name: "id_v2",
                table: "link",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "ix_link_parent_link_id_v2",
                table: "link",
                newName: "ix_link_parent_link_id");

            migrationBuilder.RenameIndex(
                name: "ix_link_item_id_v2",
                table: "link",
                newName: "ix_link_item_id");

            migrationBuilder.RenameColumn(
                name: "parent_item_id_v2",
                table: "item",
                newName: "parent_item_id");

            migrationBuilder.RenameColumn(
                name: "category_id_v2",
                table: "item",
                newName: "category_id");

            migrationBuilder.RenameColumn(
                name: "id_v2",
                table: "item",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "ix_item_parent_item_id_v2",
                table: "item",
                newName: "ix_item_parent_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_item_category_id_v2",
                table: "item",
                newName: "ix_item_category_id");

            migrationBuilder.RenameColumn(
                name: "id_v2",
                table: "category",
                newName: "id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user",
                newName: "id_v2");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "torrent",
                newName: "id_v2");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "session",
                newName: "user_id_v2");

            migrationBuilder.RenameIndex(
                name: "ix_session_user_id",
                table: "session",
                newName: "ix_session_user_id_v2");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "pikpak_job",
                newName: "id_v2");

            migrationBuilder.RenameColumn(
                name: "parent_link_id",
                table: "link",
                newName: "parent_link_id_v2");

            migrationBuilder.RenameColumn(
                name: "item_id",
                table: "link",
                newName: "item_id_v2");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "link",
                newName: "id_v2");

            migrationBuilder.RenameIndex(
                name: "ix_link_parent_link_id",
                table: "link",
                newName: "ix_link_parent_link_id_v2");

            migrationBuilder.RenameIndex(
                name: "ix_link_item_id",
                table: "link",
                newName: "ix_link_item_id_v2");

            migrationBuilder.RenameColumn(
                name: "parent_item_id",
                table: "item",
                newName: "parent_item_id_v2");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "item",
                newName: "category_id_v2");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "item",
                newName: "id_v2");

            migrationBuilder.RenameIndex(
                name: "ix_item_parent_item_id",
                table: "item",
                newName: "ix_item_parent_item_id_v2");

            migrationBuilder.RenameIndex(
                name: "ix_item_category_id",
                table: "item",
                newName: "ix_item_category_id_v2");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "category",
                newName: "id_v2");

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
    }
}
