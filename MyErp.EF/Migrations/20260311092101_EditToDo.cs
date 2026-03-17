using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyErp.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditToDo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Daily",
                table: "ToDos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Daily",
                table: "ToDos");
        }
    }
}
