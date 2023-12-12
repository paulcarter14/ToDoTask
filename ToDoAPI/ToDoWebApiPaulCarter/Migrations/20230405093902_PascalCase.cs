using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoWebApiPaulCarter.Migrations
{
    /// <inheritdoc />
    public partial class PascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDone",
                table: "Notes",
                newName: "IsDone");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Notes",
                newName: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDone",
                table: "Notes",
                newName: "isDone");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Notes",
                newName: "id");
        }
    }
}
