using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyErp.EF.Migrations
{
    /// <inheritdoc />
    public partial class newDocumnetField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "subject",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "subject",
                table: "Documents");
        }
    }
}
