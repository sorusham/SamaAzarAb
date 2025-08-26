using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageForAzarab.Migrations
{
    /// <inheritdoc />
    public partial class DocumentAddDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    OptionId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocCode = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    IssueStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentRevision = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckerId = table.Column<string>(type: "TEXT", nullable: true),
                    ApproverId = table.Column<string>(type: "TEXT", nullable: true),
                    DateChecker = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateApprover = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReviewStage = table.Column<int>(type: "INTEGER", nullable: false),
                    ForecastDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Hold = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseDocuments_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BaseDocuments_AspNetUsers_CheckerId",
                        column: x => x.CheckerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BaseDocuments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BaseDocuments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BaseDocumentId = table.Column<int>(type: "INTEGER", nullable: false),
                    RevisionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    RevisionCode = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousVersionId = table.Column<int>(type: "INTEGER", nullable: true),
                    SecurityLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatorId = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateSend = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Progress = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedToId = table.Column<string>(type: "TEXT", nullable: true),
                    Hidden = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentVersions_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentVersions_BaseDocuments_BaseDocumentId",
                        column: x => x.BaseDocumentId,
                        principalTable: "BaseDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_DocumentVersions_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalTable: "DocumentVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentVersionId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UploaderId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAttachments_AspNetUsers_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentAttachments_DocumentVersions_DocumentVersionId",
                        column: x => x.DocumentVersionId,
                        principalTable: "DocumentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentVersionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExternalTransactionNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    ReplyDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CommentSheet = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseToComments = table.Column<string>(type: "TEXT", nullable: true),
                    LetterNumber = table.Column<string>(type: "TEXT", nullable: true),
                    RecapNumber = table.Column<string>(type: "TEXT", nullable: true),
                    AzarabStatus = table.Column<string>(type: "TEXT", nullable: true),
                    DataDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTransactions_DocumentVersions_DocumentVersionId",
                        column: x => x.DocumentVersionId,
                        principalTable: "DocumentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentTransactions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseDocuments_ApproverId",
                table: "BaseDocuments",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseDocuments_CheckerId",
                table: "BaseDocuments",
                column: "CheckerId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseDocuments_DepartmentId",
                table: "BaseDocuments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseDocuments_ProjectId",
                table: "BaseDocuments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachments_DocumentVersionId",
                table: "DocumentAttachments",
                column: "DocumentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachments_UploaderId",
                table: "DocumentAttachments",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTransactions_DocumentVersionId",
                table: "DocumentTransactions",
                column: "DocumentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTransactions_ProjectId",
                table: "DocumentTransactions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_AssignedToId",
                table: "DocumentVersions",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_BaseDocumentId",
                table: "DocumentVersions",
                column: "BaseDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_CreatorId",
                table: "DocumentVersions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_PreviousVersionId",
                table: "DocumentVersions",
                column: "PreviousVersionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAttachments");

            migrationBuilder.DropTable(
                name: "DocumentTransactions");

            migrationBuilder.DropTable(
                name: "DocumentVersions");

            migrationBuilder.DropTable(
                name: "BaseDocuments");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
