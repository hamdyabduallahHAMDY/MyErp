using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyErp.EF.Migrations
{
    /// <inheritdoc />
    public partial class RowStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "UserSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "ToDos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Leads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "FAQs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Emails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowStatus",
                table: "CalenderTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "ToDos");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "FAQs");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "RowStatus",
                table: "CalenderTasks");
        }
    }
}
