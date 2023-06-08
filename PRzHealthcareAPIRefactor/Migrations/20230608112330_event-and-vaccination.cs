using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPIRefactor.Migrations
{
    /// <inheritdoc />
    public partial class eventandvaccination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Ety_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ety_Name = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Ety_Id);
                });

            migrationBuilder.CreateTable(
                name: "Vaccinations",
                columns: table => new
                {
                    Vac_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vac_Name = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Vac_Description = table.Column<string>(type: "nvarchar(2047)", maxLength: 2047, nullable: false),
                    Vac_PhotoId = table.Column<int>(type: "int", nullable: true),
                    Vac_DaysBetweenVacs = table.Column<int>(type: "int", nullable: false),
                    Vac_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Vac_InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Vac_InsertedAccId = table.Column<int>(type: "int", nullable: false),
                    Vac_ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Vac_ModifiedAccId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccinations", x => x.Vac_Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Eve_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Eve_AccId = table.Column<int>(type: "int", nullable: true),
                    Eve_TimeFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Eve_TimeTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Eve_Type = table.Column<int>(type: "int", nullable: false),
                    Eve_DoctorId = table.Column<int>(type: "int", nullable: true),
                    Eve_VacId = table.Column<int>(type: "int", nullable: true),
                    Eve_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Eve_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Eve_InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Eve_InsertedAccId = table.Column<int>(type: "int", nullable: false),
                    Eve_ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Eve_ModifiedAccId = table.Column<int>(type: "int", nullable: false),
                    Eve_SerialNumber = table.Column<string>(type: "nvarchar(23)", maxLength: 23, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Eve_Id);
                    table.ForeignKey(
                        name: "FK_Events_Accounts_Eve_AccId",
                        column: x => x.Eve_AccId,
                        principalTable: "Accounts",
                        principalColumn: "Acc_Id");
                    table.ForeignKey(
                        name: "FK_Events_Vaccinations_Eve_VacId",
                        column: x => x.Eve_VacId,
                        principalTable: "Vaccinations",
                        principalColumn: "Vac_Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_Eve_AccId",
                table: "Events",
                column: "Eve_AccId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Eve_VacId",
                table: "Events",
                column: "Eve_VacId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Vaccinations");
        }
    }
}
