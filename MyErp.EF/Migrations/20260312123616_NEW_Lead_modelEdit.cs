using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyErp.EF.Migrations
{
    /// <inheritdoc />
    public partial class NEW_Lead_modelEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Website",
                table: "Leads");
        }
    }
}
