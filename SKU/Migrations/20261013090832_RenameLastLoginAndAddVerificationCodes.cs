using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKU.Migrations
{
    /// <inheritdoc />
    public partial class RenameLastLoginAndAddVerificationCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sec_Login_Sec_User_UserId",
                table: "Sec_Login");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sec_Login",
                table: "Sec_Login");

            migrationBuilder.RenameTable(
                name: "Sec_Login",
                newName: "Sec_UserLastLogin");

            migrationBuilder.RenameIndex(
                name: "IX_Sec_Login_UserId",
                table: "Sec_UserLastLogin",
                newName: "IX_Sec_UserLastLogin_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sec_UserLastLogin",
                table: "Sec_UserLastLogin",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Sec_VerificationCode",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sec_VerificationCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sec_VerificationCode_Sec_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Sec_User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sec_VerificationCode_UserId_Purpose",
                table: "Sec_VerificationCode",
                columns: new[] { "UserId", "Purpose" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sec_UserLastLogin_Sec_User_UserId",
                table: "Sec_UserLastLogin",
                column: "UserId",
                principalTable: "Sec_User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sec_UserLastLogin_Sec_User_UserId",
                table: "Sec_UserLastLogin");

            migrationBuilder.DropTable(
                name: "Sec_VerificationCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sec_UserLastLogin",
                table: "Sec_UserLastLogin");

            migrationBuilder.RenameTable(
                name: "Sec_UserLastLogin",
                newName: "Sec_Login");

            migrationBuilder.RenameIndex(
                name: "IX_Sec_UserLastLogin_UserId",
                table: "Sec_Login",
                newName: "IX_Sec_Login_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sec_Login",
                table: "Sec_Login",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sec_Login_Sec_User_UserId",
                table: "Sec_Login",
                column: "UserId",
                principalTable: "Sec_User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
