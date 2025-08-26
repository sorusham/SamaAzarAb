using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageForAzarab.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_AspNetUsers_CreatorId",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "RevisionCode",
                table: "DocumentVersions");

            migrationBuilder.RenameColumn(
                name: "SecurityLevel",
                table: "DocumentVersions",
                newName: "IsSent");

            migrationBuilder.RenameColumn(
                name: "Hidden",
                table: "DocumentVersions",
                newName: "IsHidden");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "DocumentVersions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ClientDocCode",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AN",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AzarabCode",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CM",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocDate",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocNumber",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstSubmit",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Information",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationDate",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierId",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NC",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notification",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlanDate",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Progress",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reject",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Responsible",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BaseDocuments_CreatorId",
                table: "BaseDocuments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseDocuments_LastModifierId",
                table: "BaseDocuments",
                column: "LastModifierId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseDocuments_AspNetUsers_CreatorId",
                table: "BaseDocuments",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseDocuments_AspNetUsers_LastModifierId",
                table: "BaseDocuments",
                column: "LastModifierId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentVersions_AspNetUsers_CreatorId",
                table: "DocumentVersions",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseDocuments_AspNetUsers_CreatorId",
                table: "BaseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseDocuments_AspNetUsers_LastModifierId",
                table: "BaseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_AspNetUsers_CreatorId",
                table: "DocumentVersions");

            migrationBuilder.DropIndex(
                name: "IX_BaseDocuments_CreatorId",
                table: "BaseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_BaseDocuments_LastModifierId",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "AN",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "AzarabCode",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "CM",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "DocDate",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "DocNumber",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "FirstSubmit",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "Information",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "LastModificationDate",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "NC",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "Notification",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "PlanDate",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "Reject",
                table: "BaseDocuments");

            migrationBuilder.DropColumn(
                name: "Responsible",
                table: "BaseDocuments");

            migrationBuilder.RenameColumn(
                name: "IsSent",
                table: "DocumentVersions",
                newName: "SecurityLevel");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "DocumentVersions",
                newName: "Hidden");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "DocumentVersions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "RevisionCode",
                table: "DocumentVersions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientDocCode",
                table: "BaseDocuments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentVersions_AspNetUsers_CreatorId",
                table: "DocumentVersions",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
