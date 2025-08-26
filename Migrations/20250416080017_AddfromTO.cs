using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageForAzarab.Migrations
{
    /// <inheritdoc />
    public partial class AddfromTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "Attachments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "To",
                table: "Attachments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "AttachmentRevisions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "To",
                table: "AttachmentRevisions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "From",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "To",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "From",
                table: "AttachmentRevisions");

            migrationBuilder.DropColumn(
                name: "To",
                table: "AttachmentRevisions");
        }
    }
}
