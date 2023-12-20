using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoWebApiPaulCarter.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleAndDescriptionToNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Notes",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notes",
                newName: "Text");
        }
    }
}
