using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageForAzarab.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Letters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "Letters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Attachments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "Attachments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "AttachmentRevisions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "AttachmentRevisions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber2",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDate",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Letters_ReceiverId",
                table: "Letters",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Letters_SenderId",
                table: "Letters",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ReceiverId",
                table: "Attachments",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_SenderId",
                table: "Attachments",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentRevisions_ReceiverId",
                table: "AttachmentRevisions",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentRevisions_SenderId",
                table: "AttachmentRevisions",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentRevisions_AspNetUsers_ReceiverId",
                table: "AttachmentRevisions",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentRevisions_AspNetUsers_SenderId",
                table: "AttachmentRevisions",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_ReceiverId",
                table: "Attachments",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_SenderId",
                table: "Attachments",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Letters_AspNetUsers_ReceiverId",
                table: "Letters",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Letters_AspNetUsers_SenderId",
                table: "Letters",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentRevisions_AspNetUsers_ReceiverId",
                table: "AttachmentRevisions");

            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentRevisions_AspNetUsers_SenderId",
                table: "AttachmentRevisions");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_ReceiverId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_SenderId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Letters_AspNetUsers_ReceiverId",
                table: "Letters");

            migrationBuilder.DropForeignKey(
                name: "FK_Letters_AspNetUsers_SenderId",
                table: "Letters");

            migrationBuilder.DropIndex(
                name: "IX_Letters_ReceiverId",
                table: "Letters");

            migrationBuilder.DropIndex(
                name: "IX_Letters_SenderId",
                table: "Letters");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ReceiverId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_SenderId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_AttachmentRevisions_ReceiverId",
                table: "AttachmentRevisions");

            migrationBuilder.DropIndex(
                name: "IX_AttachmentRevisions_SenderId",
                table: "AttachmentRevisions");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "AttachmentRevisions");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "AttachmentRevisions");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber2",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegisterDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "AspNetUsers");
        }
    }
}
