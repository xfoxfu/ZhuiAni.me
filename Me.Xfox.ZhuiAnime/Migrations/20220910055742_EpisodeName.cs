using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    public partial class EpisodeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "episode",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "episode");
        }
    }
}
