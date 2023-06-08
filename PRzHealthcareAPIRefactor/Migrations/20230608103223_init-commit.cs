using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPIRefactor.Migrations
{
    /// <inheritdoc />
    public partial class initcommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    Aty_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aty_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Aty_Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Acc_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Acc_AtyId = table.Column<int>(type: "int", nullable: false),
                    Acc_Login = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Acc_Password = table.Column<string>(type: "nvarchar(2047)", maxLength: 2047, nullable: false),
                    Acc_PhotoId = table.Column<int>(type: "int", nullable: true),
                    Acc_Firstname = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Acc_Secondname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acc_Lastname = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Acc_DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Acc_Pesel = table.Column<long>(type: "bigint", nullable: false),
                    Acc_Email = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Acc_ContactNumber = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Acc_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Acc_InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Acc_ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Acc_RegistrationHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Acc_ReminderHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Acc_ReminderExpire = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Acc_Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_Acc_AtyId",
                        column: x => x.Acc_AtyId,
                        principalTable: "AccountTypes",
                        principalColumn: "Aty_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Acc_AtyId",
                table: "Accounts",
                column: "Acc_AtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "AccountTypes");
        }
    }
}
