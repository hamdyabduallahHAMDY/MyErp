using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyErp.EF.Migrations
{
    /// <inheritdoc />
    public partial class todoEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "ToDos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ToDos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ToDos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "ToDos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ToDos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ToDos");
        }
    }
}
