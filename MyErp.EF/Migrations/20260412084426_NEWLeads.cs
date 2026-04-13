using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyErp.EF.Migrations
{
    /// <inheritdoc />
    public partial class NEWLeads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstValue",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FounderAcc",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextFollowUp",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PiplineStage",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Probability",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocType",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "EstValue",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "FounderAcc",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "NextFollowUp",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "PiplineStage",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Probability",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "DocType",
                table: "Documents");
        }
    }
}
