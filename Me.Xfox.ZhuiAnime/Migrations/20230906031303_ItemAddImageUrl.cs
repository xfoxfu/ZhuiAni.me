using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    /// <inheritdoc />
    public partial class ItemAddImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "item",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                table: "item");
        }
    }
}
