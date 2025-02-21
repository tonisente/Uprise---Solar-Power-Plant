using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uprise___Solar_Power_Plant.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PowerPlants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    InstalationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationLongitude = table.Column<double>(type: "float", nullable: false),
                    LocationLatitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerPlants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerPlantsReading",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PowerOutput = table.Column<int>(type: "int", nullable: false),
                    PowerPlantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerPlantsReading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerPlantsReading_PowerPlants_PowerPlantId",
                        column: x => x.PowerPlantId,
                        principalTable: "PowerPlants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PowerPlantsReading_PowerPlantId",
                table: "PowerPlantsReading",
                column: "PowerPlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PowerPlantsReading");

            migrationBuilder.DropTable(
                name: "PowerPlants");
        }
    }
}
