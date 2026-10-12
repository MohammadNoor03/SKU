using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKU.Migrations
{
    /// <inheritdoc />
    public partial class user_usertype_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserTypeId",
                table: "Sec_User",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Sec_User_UserTypeId",
                table: "Sec_User",
                column: "UserTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sec_User_Sec_UserType_UserTypeId",
                table: "Sec_User",
                column: "UserTypeId",
                principalTable: "Sec_UserType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sec_User_Sec_UserType_UserTypeId",
                table: "Sec_User");

            migrationBuilder.DropIndex(
                name: "IX_Sec_User_UserTypeId",
                table: "Sec_User");

            migrationBuilder.DropColumn(
                name: "UserTypeId",
                table: "Sec_User");
        }
    }
}
