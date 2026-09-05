using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic_Mangment.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "VisitAttachments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "VisitAttachments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "UploadedByUserId",
                table: "VisitAttachments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VisitAttachments_UploadedByUserId",
                table: "VisitAttachments",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitAttachments_Users_UploadedByUserId",
                table: "VisitAttachments",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitAttachments_Users_UploadedByUserId",
                table: "VisitAttachments");

            migrationBuilder.DropIndex(
                name: "IX_VisitAttachments_UploadedByUserId",
                table: "VisitAttachments");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "VisitAttachments");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "VisitAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "VisitAttachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
