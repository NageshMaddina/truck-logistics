using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLogistics.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carriers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MCNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DOTNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ContactName = table.Column<string>(type: "TEXT", nullable: true),
                    ContactEmail = table.Column<string>(type: "TEXT", nullable: true),
                    ContactPhone = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    ZipCode = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", precision: 3, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carriers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LicenseNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Loads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoadNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CarrierId = table.Column<int>(type: "INTEGER", nullable: true),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EquipmentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    Commodity = table.Column<string>(type: "TEXT", nullable: true),
                    PickupAddress = table.Column<string>(type: "TEXT", nullable: false),
                    PickupCity = table.Column<string>(type: "TEXT", nullable: false),
                    PickupState = table.Column<string>(type: "TEXT", nullable: false),
                    PickupZip = table.Column<string>(type: "TEXT", nullable: true),
                    PickupDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeliveryAddress = table.Column<string>(type: "TEXT", nullable: false),
                    DeliveryCity = table.Column<string>(type: "TEXT", nullable: false),
                    DeliveryState = table.Column<string>(type: "TEXT", nullable: false),
                    DeliveryZip = table.Column<string>(type: "TEXT", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Miles = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    Rate = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loads_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Loads_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TrackingEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoadId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Latitude = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    EventTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackingEvents_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carriers_MCNumber",
                table: "Carriers",
                column: "MCNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CarrierId",
                table: "Drivers",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Loads_CarrierId",
                table: "Loads",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Loads_DriverId",
                table: "Loads",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Loads_LoadNumber",
                table: "Loads",
                column: "LoadNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackingEvents_LoadId",
                table: "TrackingEvents",
                column: "LoadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackingEvents");

            migrationBuilder.DropTable(
                name: "Loads");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Carriers");
        }
    }
}
