using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKU.Migrations
{
    /// <inheritdoc />
    public partial class user_login_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Sec_Login",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Sec_Login_UserId",
                table: "Sec_Login",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sec_Login_Sec_User_UserId",
                table: "Sec_Login",
                column: "UserId",
                principalTable: "Sec_User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sec_Login_Sec_User_UserId",
                table: "Sec_Login");

            migrationBuilder.DropIndex(
                name: "IX_Sec_Login_UserId",
                table: "Sec_Login");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Sec_Login");
        }
    }
}
