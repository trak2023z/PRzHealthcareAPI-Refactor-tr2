using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPIRefactor.Migrations
{
    /// <inheritdoc />
    public partial class binarydata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BinData",
                columns: table => new
                {
                    Bin_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bin_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Bin_Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bin_InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Bin_InsertedAccId = table.Column<int>(type: "int", nullable: false),
                    Bin_ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Bin_ModifiedAccId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinData", x => x.Bin_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BinData");
        }
    }
}
