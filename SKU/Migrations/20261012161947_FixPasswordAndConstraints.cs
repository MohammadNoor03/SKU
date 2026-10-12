using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SKU.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswordAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sec_User_Sec_UserType_UserTypeId",
                table: "Sec_User");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Sec_User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
                table: "Sec_UserType",
                columns: new[] { "Id", "Description", "MajorCode", "MinorCode", "Name" },
                values: new object[,]
                {
                    { 1L, "Human, buys from a store", 1, 1, "Customer" },
                    { 2L, "System administrator", 1, 2, "Admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sec_User_Email",
                table: "Sec_User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sec_User_Username",
                table: "Sec_User",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sec_User_Sec_UserType_UserTypeId",
                table: "Sec_User",
                column: "UserTypeId",
                principalTable: "Sec_UserType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sec_User_Sec_UserType_UserTypeId",
                table: "Sec_User");

            migrationBuilder.DropIndex(
                name: "IX_Sec_User_Email",
                table: "Sec_User");

            migrationBuilder.DropIndex(
                name: "IX_Sec_User_Username",
                table: "Sec_User");

            migrationBuilder.DeleteData(
                table: "Sec_UserType",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Sec_UserType",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Sec_User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Sec_User_Sec_UserType_UserTypeId",
                table: "Sec_User",
                column: "UserTypeId",
                principalTable: "Sec_UserType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
